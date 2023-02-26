namespace Kunet.AsyncInterceptor.PerfTests;

public interface IFoo
{
    void VoidMethod();

    Task TaskMethod();

    Task<T> TaskMethod<T>();

    Task<T> RunningTaskMethod<T>(Task running);

    ValueTask ValueTaskMethod();

    ValueTask<T> ValueTaskMethod<T>();

    ValueTask<T> RunningValueTaskMethod<T>(Task running);
}

public sealed class Foo : IFoo
{
    public void VoidMethod() { }

    public Task TaskMethod() => Task.CompletedTask;

    public Task<T> TaskMethod<T>() => Task.FromResult<T>(default);

    public async Task<T> RunningTaskMethod<T>(Task running)
    {
        await running.ConfigureAwait(false);
        return default;
    }

    public ValueTask ValueTaskMethod() => new();

    public ValueTask<T> ValueTaskMethod<T>() => new(default(T));

    public async ValueTask<T> RunningValueTaskMethod<T>(Task running)
    {
        await running.ConfigureAwait(false);
        return default;
    }
}