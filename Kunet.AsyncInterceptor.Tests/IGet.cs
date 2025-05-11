namespace Kunet.AsyncInterceptor.Tests;

public interface IGet
{
    T Get<T>();

    Task<T> GetTask<T>();

    ValueTask<T> GetValueTask<T>();
}