using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

public static class AdaptersFactory
{
    private static readonly MethodInfo CreateValueTupleMethod;
    private static readonly Dictionary<Type, Func<IInvocation, (IAsyncTaskBuilder, IAsyncInvocation)>> Cache = new();
    private static readonly Dictionary<Type, (Type, Type)> GenericTypesRegistration = new();

    static AdaptersFactory()
    {
        CreateValueTupleMethod = typeof(ValueTuple).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                   .First(m => m.GetParameters().Length == 2)
                                                   .MakeGenericMethod(typeof(IAsyncTaskBuilder), typeof(IAsyncInvocation));

        Register<Task>(x => (new AsyncTaskBuilderOfTask(), new AsyncInvocationOfTask(x)));
        Register<ValueTask>(x => (new AsyncTaskBuilderOfValueTask(), new AsyncInvocationOfValueTask(x)));
        Register(typeof(Task<>), typeof(AsyncTaskBuilderOfTask<>), typeof(AsyncInvocationOfTask<>));
        Register(typeof(ValueTask<>), typeof(AsyncTaskBuilderOfValueTask<>), typeof(AsyncInvocationOfValueTask<>));
    }

    public static void Register<T>(Func<IInvocation, (IAsyncTaskBuilder, IAsyncInvocation)> factory)
        => Cache[typeof(T)] = factory;

    public static void Register(Type genericReturnType, Type genericAsyncTaskBuilderType, Type genericAsyncInvocationType)
        => GenericTypesRegistration[genericReturnType] = (genericAsyncTaskBuilderType, genericAsyncInvocationType);

    public static bool TryCreate(IInvocation invocation, out IAsyncTaskBuilder builder, out IAsyncInvocation asyncInvocation)
    {
        var returnType = invocation.Method.ReturnType;
        if (Cache.TryGetValue(returnType, out var factory))
        {
            (builder, asyncInvocation) = factory.Invoke(invocation);
            return true;
        }
        if (returnType.IsGenericType && returnType.GenericTypeArguments.Length == 1)
        {
            var genericType = returnType.GetGenericTypeDefinition();
            if (GenericTypesRegistration.TryGetValue(genericType, out var types))
            {
                var (genericAsyncTaskBuilderType, genericAsyncInvocationType) = types;
                factory = CreateFactory(genericAsyncTaskBuilderType, genericAsyncInvocationType, returnType.GenericTypeArguments);
                Cache[returnType] = factory;
                (builder, asyncInvocation) = factory.Invoke(invocation);
                return true;
            }
        }
        (builder, asyncInvocation) = (null, null);
        return false;
    }

    private static Func<IInvocation, (IAsyncTaskBuilder, IAsyncInvocation)> CreateFactory(Type builderGenericType, Type invocationGenericType, Type[] typeArguments)
    {
        var builderType = builderGenericType.MakeGenericType(typeArguments);
        var builderCtor = builderType.GetConstructor(Type.EmptyTypes);
        var newBuilder = Expression.New(builderCtor);

        var invocationType = invocationGenericType.MakeGenericType(typeArguments);
        var invocationCtor = invocationType.GetConstructors().Single();
        var argument = Expression.Parameter(typeof(IInvocation));
        var newInvocation = Expression.New(invocationCtor, argument);

        var body = Expression.Call(CreateValueTupleMethod, newBuilder, newInvocation);

        return (Func<IInvocation, (IAsyncTaskBuilder, IAsyncInvocation)>)Expression.Lambda(body, argument).Compile();
    }
}