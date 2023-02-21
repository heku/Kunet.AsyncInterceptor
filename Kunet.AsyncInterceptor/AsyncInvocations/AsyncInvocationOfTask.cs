using Castle.DynamicProxy;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal sealed class AsyncInvocationOfTask : AsyncInvocation
{
    public AsyncInvocationOfTask(IInvocation invocation) : base(invocation)
    {
    }

    protected override ValueTask SetAsyncResult()
    {
        Debug.Assert(Invocation.ReturnValue is Task);
        return new ValueTask((Task)Invocation.ReturnValue);
    }
}