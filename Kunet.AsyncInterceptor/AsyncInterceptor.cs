using Castle.DynamicProxy;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

// Inspired by https://github.com/stakx/DynamicProxy.AsyncInterceptor

public abstract class AsyncInterceptor : IInterceptor
{
    void IInterceptor.Intercept(IInvocation invocation)
    {
        var returnType = invocation.Method.ReturnType;
        if (returnType == typeof(Task))
        {
            var adapter = new AsyncAdapterOfTask(invocation);
            var stateMachine = new AsyncStateMachine<AsyncAdapterOfTask>(in adapter, this);
            adapter.Start(ref stateMachine);
            Debug.Assert(adapter.Task is not null);
            invocation.ReturnValue = adapter.Task;
        }
        else if (returnType == typeof(ValueTask))
        {
            var adapter = new AsyncAdapterOfValueTask(invocation);
            var stateMachine = new AsyncStateMachine<AsyncAdapterOfValueTask>(in adapter, this);
            adapter.Start(ref stateMachine);
            Debug.Assert(adapter.Task is not null);
            invocation.ReturnValue = adapter.Task;
        }
        else if (AsyncAdapter.TryCreate(invocation, out var adapter))
        {
            var stateMachine = new AsyncStateMachine<AsyncAdapter>(in adapter, this);
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

    // Compat.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ValueTask InternalInterceptAsync(IAsyncInvocation invocation) => InterceptAsync(invocation);
}