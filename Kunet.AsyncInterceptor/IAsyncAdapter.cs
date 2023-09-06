using System;
using System.Runtime.CompilerServices;

namespace Kunet.AsyncInterceptor;

internal interface IAsyncAdapter : IAsyncInvocation
{
    /// <summary>
    /// Encapsulate the <see cref="AsyncInterceptor.InterceptAsync(IAsyncInvocation)"/> to be a same TASK type as the TARGET's.
    /// <code>
    /// Task = {
    ///            await InterceptAsync()
    ///            {
    ///                // ...
    ///                await ProceedAsync() { AsyncResult = await NEXT Task }
    ///                // ...
    ///            }
    ///            return AsyncResult;
    ///        }
    /// </code>
    /// </summary>
    object Task { get; }

    /// <summary>
    /// Start the <paramref name="stateMachine"/>, i.e. start the <see cref="Task"/>.
    /// </summary>
    /// <remarks>
    /// The <paramref name="stateMachine"/> will invoke <see cref="AsyncInterceptor.InterceptAsync(IAsyncInvocation)"/>.
    /// </remarks>
    void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine;

    /// <summary>
    /// Set the <see cref="Task"/> succeeded with <paramref name="result"/>.
    /// </summary>
    void SetResult(object result);

    /// <summary>
    /// Set the <see cref="Task"/> failed with <paramref name="exception"/>.
    /// </summary>
    void SetException(Exception exception);

    /// <summary>
    /// Link the <paramref name="stateMachine"/> and <paramref name="awaiter"/> together, so that when <paramref name="awaiter"/>
    /// completed, the <paramref name="stateMachine"/>.<see cref="IAsyncStateMachine.MoveNext"/> will be invoked.
    /// </summary>
    void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine;
}