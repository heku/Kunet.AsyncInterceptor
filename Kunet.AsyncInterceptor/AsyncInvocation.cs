using Castle.DynamicProxy;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

public abstract class AsyncInvocation : IAsyncInvocation
{
    private readonly IInvocationProceedInfo _proceed;

    public AsyncInvocation(IInvocation invocation)
    {
        Invocation = invocation;
        _proceed = invocation.CaptureProceedInfo();
    }

    public IInvocation Invocation { get; }

    public object AsyncResult { get; set; }

    public async ValueTask ProceedAsync()
    {
        var lastReturnValue = Invocation.ReturnValue;
        try
        {
            _proceed.Invoke();
            Debug.Assert(Invocation.ReturnValue is not null);
            await SetAsyncResult().ConfigureAwait(false);
        }
        finally
        {
            Invocation.ReturnValue = lastReturnValue;
        }
    }

    protected abstract ValueTask SetAsyncResult();
}