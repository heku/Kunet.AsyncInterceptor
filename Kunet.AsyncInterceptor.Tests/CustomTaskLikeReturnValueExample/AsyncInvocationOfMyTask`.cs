using Castle.DynamicProxy;

namespace Kunet.AsyncInterceptor.Tests;

// You must provide this to set AsyncResult from Invocation.ReturnValue

internal sealed class AsyncInvocationOfMyTask<T> : AsyncInvocation
{
    public AsyncInvocationOfMyTask(IInvocation invocation) : base(invocation)
    {
    }

    protected override async ValueTask SetAsyncResult()
    {
        AsyncResult = await (MyTask<T>)Invocation.ReturnValue;
    }
}