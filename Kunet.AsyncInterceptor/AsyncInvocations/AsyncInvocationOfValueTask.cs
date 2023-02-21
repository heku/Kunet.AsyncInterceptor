using Castle.DynamicProxy;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal sealed class AsyncInvocationOfValueTask : AsyncInvocation
{
    public AsyncInvocationOfValueTask(IInvocation invocation) : base(invocation)
    {
    }

    protected override ValueTask SetAsyncResult()
    {
        Debug.Assert(Invocation.ReturnValue is ValueTask);
        return (ValueTask)Invocation.ReturnValue;
    }
}