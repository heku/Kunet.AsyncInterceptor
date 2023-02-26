using Castle.DynamicProxy;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal sealed class AsyncInvocationOfValueTask<T> : AsyncInvocation
{
    public AsyncInvocationOfValueTask(IInvocation invocation) : base(invocation)
    {
    }

    protected override async ValueTask SetAsyncResult()
    {
        Debug.Assert(Invocation.ReturnValue is ValueTask<T>);
        var valueTask = (ValueTask<T>)Invocation.ReturnValue;
        AsyncResult = await valueTask.ConfigureAwait(false);
    }
}