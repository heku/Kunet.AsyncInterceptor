﻿using Castle.DynamicProxy;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

// Inspired by https://github.com/stakx/DynamicProxy.AsyncInterceptor

public abstract class AsyncInterceptor : IInterceptor
{
    void IInterceptor.Intercept(IInvocation invocation)
    {
        if (AsyncAdapter.TryCreate(invocation, out var adapter))
        {
            AsyncStateMachine stateMachine = new(adapter, InterceptAsync);
            adapter.Start(ref stateMachine);
            Debug.Assert(adapter.Task is not null);
            invocation.ReturnValue = adapter.Task;
        }
        else
        {
            Intercept(invocation);
        }
    }

    protected abstract void Intercept(IInvocation invocation);

    protected abstract ValueTask InterceptAsync(IAsyncInvocation invocation);
}