using Castle.DynamicProxy;
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
            invocation.ReturnValue = InterceptAsync(new AsyncInvocation(invocation, static ai => ai.Invocation.ReturnValue is Task task ? new(task) : default)).AsTask();
        }
        else if (returnType == typeof(ValueTask))
        {
            invocation.ReturnValue = InterceptAsync(new AsyncInvocation(invocation, static ai => ai.Invocation.ReturnValue is ValueTask valueTask ? valueTask : default));
        }
        else if (AsyncAdapter.TryCreate(invocation, out var adapter))
        {
            invocation.ReturnValue = adapter.ConvertToReturnTask(InterceptAsync(adapter));
        }
        else
        {
            Intercept(invocation);
        }
    }

    protected abstract void Intercept(IInvocation invocation);

    protected abstract ValueTask InterceptAsync(IAsyncInvocation invocation);
}