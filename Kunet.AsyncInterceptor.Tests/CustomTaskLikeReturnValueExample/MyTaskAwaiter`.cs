using System.Runtime.CompilerServices;

namespace Kunet.AsyncInterceptor.Tests;

// No need to care about this implementation
public class MyTaskAwaiter<T> : INotifyCompletion
{
    private readonly MyTask<T> _task;

    public MyTaskAwaiter(MyTask<T> task) => _task = task;

    public bool IsCompleted => _task.Result is not null;

    public T GetResult() => _task.Result;

    public void OnCompleted(Action continuation) => _task.OnCompleted += continuation;
}