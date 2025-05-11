using Castle.DynamicProxy;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

[StructLayout(LayoutKind.Auto)]
internal class AsyncInvocation(IInvocation invocation, Func<IAsyncInvocation, ValueTask> asyncResultSetter) : IAsyncInvocation
{
    private readonly IInvocationProceedInfo _proceed = invocation.CaptureProceedInfo();

    public IInvocation Invocation => invocation;
    public object AsyncResult { get; set; }

    // AsyncResult = await Invocation.ReturnValue  = NEXT()
    public ValueTask ProceedAsync()
    {
        _proceed.Invoke();
        return asyncResultSetter(this);
    }
}