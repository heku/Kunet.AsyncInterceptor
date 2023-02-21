using System.Runtime.CompilerServices;

namespace Kunet.AsyncInterceptor.Tests;

// You must provide this to adapt your custom TaskMethodBuilder to IAsyncTaskBuilder

public sealed class AsyncTaskBuilderOfMyTask<T> : IAsyncTaskBuilder
{
    private readonly MyTaskMethodBuilder<T> _builder = MyTaskMethodBuilder<T>.Create();

    public AsyncTaskBuilderOfMyTask() => Task = _builder.Task;

    public object Task { get; }

    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine => _builder.Start(ref stateMachine);

    public void SetException(Exception ex) => _builder.SetException(ex);

    public void SetResult(object result) => _builder.SetResult((T)result);

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
}