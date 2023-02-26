using Castle.DynamicProxy;

namespace Kunet.AsyncInterceptor.Tests;

public sealed class ReturnValueFormatter : AsyncInterceptor
{
    private readonly string _format;

    public ReturnValueFormatter(string format) => _format = format;

    protected override void Intercept(IInvocation invocation)
    {
        invocation.Proceed();
        invocation.ReturnValue = string.Format(_format, invocation.ReturnValue);
    }

    protected override async ValueTask InterceptAsync(IAsyncInvocation invocation)
    {
        await invocation.ProceedAsync().ConfigureAwait(false);
        invocation.AsyncResult = string.Format(_format, invocation.AsyncResult);
    }
}