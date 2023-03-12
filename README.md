## Kunet.AsyncInterceptor

[![Build on Push](https://github.com/heku/Kunet.AsyncInterceptor/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/heku/Kunet.AsyncInterceptor/actions/workflows/dotnet.yml)
[![Nuget Version](https://img.shields.io/nuget/v/Kunet.AsyncInterceptor)](https://www.nuget.org/packages/Kunet.AsyncInterceptor)

Inspired by [stakx.DynamicProxy.AsyncInterceptor](https://github.com/stakx/DynamicProxy.AsyncInterceptor), but rewrite with less reflection to get better performance.

This library allows you to smoothly connect [Castle.DynamicProxy](https://github.com/castleproject/Core) with your `async/await` code,
it provides an abstract `AsyncInterceptor` class, your interceptor inherited from this class can get the ability to intercept method
which return value is `Task`, `Task<T>`, `ValueTask` or `ValueTask<T>`.
You are also able to extend the library to support custom [task-like types](https://github.com/dotnet/roslyn/blob/main/docs/features/task-types.md).


```csharp
class Example : AsyncInterceptor
{
    // This gets called when a non-awaitable method is intercepted:
    protected override void Intercept(IInvocation invocation)
    {
        invocation.Proceed();
        invocation.ReturnValue = ... ;
    }

    // Or this gets called when an awaitable method is intercepted:
    protected override async ValueTask InterceptAsync(IAsyncInvocation invocation)
    {
        await invocation.ProceedAsync();
        invocation.AsyncResult = ... ;
    }
}
```