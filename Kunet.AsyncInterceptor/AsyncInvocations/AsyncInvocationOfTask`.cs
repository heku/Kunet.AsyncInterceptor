using Castle.DynamicProxy;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal sealed class AsyncInvocationOfTask<T> : AsyncInvocation
{
    public AsyncInvocationOfTask(IInvocation invocation) : base(invocation)
    {
    }

    protected override async ValueTask SetAsyncResult()
    {
        Debug.Assert(Invocation.ReturnValue is Task<T>);
        var task = (Task<T>)Invocation.ReturnValue;
        AsyncResult = await task.ConfigureAwait(false);
    }
}