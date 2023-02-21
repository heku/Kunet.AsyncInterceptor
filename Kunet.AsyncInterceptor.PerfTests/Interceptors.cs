using Castle.DynamicProxy;

namespace Kunet.AsyncInterceptor.PerfTests;

public sealed class KunetInterceptor : AsyncInterceptor
{
    protected override void Intercept(IInvocation invocation) => invocation.Proceed();

    protected override ValueTask InterceptAsync(IAsyncInvocation invocation) => invocation.ProceedAsync();
}

public sealed class StakxInterceptor : stakx.DynamicProxy.AsyncInterceptor
{
    protected override void Intercept(IInvocation invocation) => invocation.Proceed();

    protected override ValueTask InterceptAsync(stakx.DynamicProxy.IAsyncInvocation invocation) => invocation.ProceedAsync();
}