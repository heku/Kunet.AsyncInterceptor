using Castle.DynamicProxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal partial class AsyncAdapter
{
    internal static readonly ConcurrentDictionary<Type, Func<IInvocation, AsyncAdapter>> FactoryCache = []; // Task    -> (invocation => new AsyncAdapterOfTask(invocation))
    internal static readonly Dictionary<Type, Type> OpenGenericTypesRegistration = [];                      // Task<T> -> AsyncAdapterOfTask<T>

    static AsyncAdapter()
    {
        Register(typeof(Task<>), typeof(AsyncAdapterOfTask<>));           // Task<T>
        Register(typeof(ValueTask<>), typeof(AsyncAdapterOfValueTask<>)); // ValueTask<T>
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