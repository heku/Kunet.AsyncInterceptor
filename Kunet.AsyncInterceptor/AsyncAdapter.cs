using Castle.DynamicProxy;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

public abstract partial class AsyncAdapter : IAsyncAdapter
{
    #region IAsyncInvocation

    private readonly IInvocationProceedInfo _proceed;

    protected AsyncAdapter(IInvocation invocation)
    {
        Invocation = invocation;
        _proceed = invocation.CaptureProceedInfo();
    }

    public IInvocation Invocation { get; }

    public object AsyncResult { get; set; }

    public async ValueTask ProceedAsync()
    {
        _proceed.Invoke();                                  // Invocation.ReturnValue = NEXT()
        if (Invocation.ReturnValue is not null)
        {
            await SetAsyncResult().ConfigureAwait(false);   // AsyncResult = await Invocation.ReturnValue
        }
    }

    /// <summary>
    /// <c>AsyncResult = <see langword="await"/> Invocation.ReturnValue</c>
    /// </summary>
    protected abstract ValueTask SetAsyncResult();

    #endregion IAsyncInvocation

    public abstract object Task { get; }

    public abstract void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine;

    public abstract void SetResult(object result);

    public abstract void SetException(Exception exception);

    public abstract void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine;
}