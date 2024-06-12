using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

[StructLayout(LayoutKind.Auto)]
internal struct AsyncAdapterOfValueTask : IAsyncInvocation, IAsyncAdapter
{
    private readonly IInvocationProceedInfo _proceed;
    private readonly AsyncValueTaskMethodBuilder _builder = AsyncValueTaskMethodBuilder.Create();

    public AsyncAdapterOfValueTask(IInvocation invocation)
    {
        _proceed = invocation.CaptureProceedInfo();
        Invocation = invocation;
        Task = _builder.Task;
    }

    public IInvocation Invocation { get; }

    public object AsyncResult { get; set; }

    public object Task { get; }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);

    public ValueTask ProceedAsync()
    {
        _proceed.Invoke();                            // Invocation.ReturnValue = NEXT()
        if (Invocation.ReturnValue is not null)
        {
            Debug.Assert(Invocation.ReturnValue is ValueTask);
            return (ValueTask)Invocation.ReturnValue; // AsyncResult = await Invocation.ReturnValue
        }
        return default;
    }

    public void SetException(Exception exception) => _builder.SetException(exception);

    public void SetResult(object result) => _builder.SetResult();

    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine => _builder.Start(ref stateMachine);
}