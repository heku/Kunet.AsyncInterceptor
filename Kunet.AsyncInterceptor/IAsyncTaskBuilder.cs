using System;
using System.Runtime.CompilerServices;

namespace Kunet.AsyncInterceptor;

public interface IAsyncTaskBuilder
{
    object Task { get; }

    void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine;

    void SetResult(object result);

    void SetException(Exception ex);

    void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine;
}