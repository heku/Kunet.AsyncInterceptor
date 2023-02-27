using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Kunet.AsyncInterceptor;

[DebuggerStepThrough]
internal sealed class AsyncTaskBuilderOfValueTask<T> : IAsyncTaskBuilder
{
    private readonly AsyncValueTaskMethodBuilder<T> _builder = AsyncValueTaskMethodBuilder<T>.Create();

    public AsyncTaskBuilderOfValueTask() => Task = _builder.Task;

    public object Task { get; }

    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine => _builder.Start(ref stateMachine);

    public void SetResult(object result) => _builder.SetResult((T)result);

    public void SetException(Exception exception) => _builder.SetException(exception);

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
}