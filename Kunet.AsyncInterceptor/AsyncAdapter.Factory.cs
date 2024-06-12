using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

public partial class AsyncAdapter
{
    internal static readonly Dictionary<Type, Func<IInvocation, AsyncAdapter>> FactoryCache = new();    // Task    -> (invocation => new AsyncAdapterOfTask(invocation))
    internal static readonly Dictionary<Type, Type> OpenGenericTypesRegistration = new();               // Task<T> -> AsyncAdapterOfTask<T>

    static AsyncAdapter()
    {
        Register(typeof(Task<>), typeof(AsyncAdapterOfTask<>));             // Task<T>
        Register(typeof(ValueTask<>), typeof(AsyncAdapterOfValueTask<>));   // ValueTask<T>
    }

    public static void Register<T>(Func<IInvocation, AsyncAdapter> factory) => FactoryCache[typeof(T)] = factory;

    public static void Register(Type openGenericReturnType, Type openGenericAsyncAdapterType) => OpenGenericTypesRegistration[openGenericReturnType] = openGenericAsyncAdapterType;

    internal static bool TryCreate(IInvocation invocation, out AsyncAdapter adapter)
    {
        var returnType = invocation.Method.ReturnType;
        if (FactoryCache.TryGetValue(returnType, out var factory))
        {
            adapter = factory.Invoke(invocation);
            return true;
        }
        if (returnType.IsGenericType && returnType.GenericTypeArguments.Length == 1)
        {
            var openGenericReturnType = returnType.GetGenericTypeDefinition();
            if (OpenGenericTypesRegistration.TryGetValue(openGenericReturnType, out var openGenericAsyncAdapterType))
            {
                factory = CreateFactory(openGenericAsyncAdapterType, returnType.GenericTypeArguments);
                FactoryCache[returnType] = factory;
                adapter = factory.Invoke(invocation);
                return true;
            }
        }
        var builderType =
            (Attribute.GetCustomAttribute(invocation.Method, typeof(AsyncMethodBuilderAttribute), false) as AsyncMethodBuilderAttribute)?.BuilderType ??
            (Attribute.GetCustomAttribute(invocation.Method.ReturnType, typeof(AsyncMethodBuilderAttribute), false) as AsyncMethodBuilderAttribute)?.BuilderType;
        if (builderType is not null)
        {
            if (builderType.IsGenericType is false)
            {
                adapter = new AsyncAdapterFallback(builderType, invocation);
                return true;
            }
            if (builderType.IsGenericType && returnType.IsGenericType && returnType.GenericTypeArguments.Length == 1)
            {
                builderType = builderType.MakeGenericType(returnType.GenericTypeArguments);
                adapter = new AsyncAdapterFallback(builderType, invocation);
                return true;
            }
        }
        adapter = null;
        return false;
    }

    private static Func<IInvocation, AsyncAdapter> CreateFactory(Type openGenericReturnType, Type[] typeArguments)
    {
        var adapterType = openGenericReturnType.MakeGenericType(typeArguments);
        var adapterCtor = adapterType.GetConstructors().Single();
        var parameter = Expression.Parameter(typeof(IInvocation));
        var newInvocation = Expression.New(adapterCtor, parameter);

        return (Func<IInvocation, AsyncAdapter>)Expression.Lambda(newInvocation, parameter).Compile();
    }
}