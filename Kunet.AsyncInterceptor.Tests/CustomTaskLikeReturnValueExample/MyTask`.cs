using System.Runtime.CompilerServices;

namespace Kunet.AsyncInterceptor.Tests;

// https://github.com/dotnet/roslyn/blob/main/docs/features/task-types.md

// No need to care about this implementation
[AsyncMethodBuilder(typeof(MyTaskMethodBuilder<>))]
public class MyTask<T>
{
    public event Action OnCompleted;

    public MyTask()
    {
    }

    public MyTask(T result)
    {
        Result = result;
    }

    public T Result { get; private set; }

    public void Complete(T result)
    {
        Result = result;
        OnCompleted?.Invoke();
    }

    public MyTaskAwaiter<T> GetAwaiter() => new(this);
}