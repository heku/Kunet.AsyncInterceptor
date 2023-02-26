using Castle.DynamicProxy;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

// Inspired by https://github.com/stakx/DynamicProxy.AsyncInterceptor

public abstract class AsyncInterceptor : IInterceptor
{
    void IInterceptor.Intercept(IInvocation invocation)
    {
        if (AdaptersFactory.TryCreate(invocation, out var builder, out var asyncInvocation))
        {
            AsyncStateMachine stateMachine = new(builder, asyncInvocation, InterceptAsync);
            builder.Start(ref stateMachine);
            Debug.Assert(builder.Task is not null);
            invocation.ReturnValue = builder.Task;
        }
        else
        {
            Intercept(invocation);
        }
    }

    protected abstract void Intercept(IInvocation invocation);

    protected abstract ValueTask InterceptAsync(IAsyncInvocation invocation);
}