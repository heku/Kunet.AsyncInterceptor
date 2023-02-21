using System.Runtime.CompilerServices;

namespace Kunet.AsyncInterceptor.Tests;

// https://github.com/dotnet/roslyn/blob/main/docs/features/task-types.md

// No need to care about this implementation
public class MyTaskMethodBuilder<T>
{
    public static MyTaskMethodBuilder<T> Create() => new() { Task = new() };

    public MyTask<T> Task { get; private set; }

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine => stateMachine.MoveNext();

#pragma warning disable IDE0060 // Remove unused parameter
    public void SetStateMachine(IAsyncStateMachine stateMachine)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    public void SetResult(T result) => Task.Complete(result);

    public void SetException(Exception exception) => throw new NotImplementedException();

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine => awaiter.OnCompleted(stateMachine.MoveNext);

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine => awaiter.OnCompleted(stateMachine.MoveNext);
}