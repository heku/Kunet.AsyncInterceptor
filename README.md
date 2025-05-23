## Kunet.AsyncInterceptor

[![Build on Push](https://github.com/heku/Kunet.AsyncInterceptor/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/heku/Kunet.AsyncInterceptor/actions/workflows/dotnet.yml)
[![Nuget Version](https://img.shields.io/nuget/v/Kunet.AsyncInterceptor)](https://www.nuget.org/packages/Kunet.AsyncInterceptor)

Inspired by [stakx.DynamicProxy.AsyncInterceptor](https://github.com/stakx/DynamicProxy.AsyncInterceptor), but rewrite with less reflection to get better performance.

This library allows you to smoothly connect [Castle.DynamicProxy](https://github.com/castleproject/Core) with your `async/await` code,
it provides an abstract `AsyncInterceptor` class, your interceptor inherited from this class can get the ability to intercept method
which return value is `Task`, `Task<T>`, `ValueTask` or `ValueTask<T>`.


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

#### Benchmark

##### .NET 8.0

| Method                 | Categories          | Mean        | Ratio | Gen0   | Allocated | Alloc Ratio |
|----------------------- |-------------------- |------------:|------:|-------:|----------:|------------:|
| StakxRunningTaskT      | RunningTask&lt;T&gt;      | 2,076.89 ns |  6.64 | 0.1221 |    2104 B |        2.33 |
| KunetRunningTaskT      | RunningTask&lt;T&gt;      |   312.92 ns |  1.00 | 0.0539 |     904 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxRunningValueTaskT | RunningValueTask&lt;T&gt; | 2,908.91 ns |  8.54 | 0.1450 |    2440 B |        2.48 |
| KunetRunningValueTaskT | RunningValueTask&lt;T&gt; |   340.72 ns |  1.00 | 0.0587 |     984 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxTask              | Task                |   916.48 ns | 31.27 | 0.0448 |     752 B |        4.09 |
| KunetTask              | Task                |    29.31 ns |  1.00 | 0.0110 |     184 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxTaskT             | Task&lt;T&gt;             | 1,303.35 ns |  9.44 | 0.0648 |    1144 B |        2.60 |
| KunetTaskT             | Task&lt;T&gt;             |   138.14 ns |  1.00 | 0.0262 |     440 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxValueTask         | ValueTask           | 1,713.29 ns | 45.55 | 0.0629 |    1080 B |        4.35 |
| KunetValueTask         | ValueTask           |    37.62 ns |  1.00 | 0.0148 |     248 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxValueTaskT        | ValueTask&lt;T&gt;        | 2,118.77 ns | 14.53 | 0.0877 |    1472 B |        2.92 |
| KunetValueTaskT        | ValueTask&lt;T&gt;        |   145.85 ns |  1.00 | 0.0300 |     504 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxVoid              | Void                |    77.71 ns |  4.14 | 0.0062 |     104 B |        1.00 |
| KunetVoid              | Void                |    18.78 ns |  1.00 | 0.0062 |     104 B |        1.00 |

##### .NET Framework 4.8.1

| Method                 | Categories          | Mean        | Ratio | Gen0   | Allocated | Alloc Ratio |
|----------------------- |-------------------- |------------:|------:|-------:|----------:|------------:|
| StakxRunningTaskT      | RunningTask&lt;T&gt;      | 6,797.09 ns |  5.59 | 0.5798 |    3684 B |        2.31 |
| KunetRunningTaskT      | RunningTask&lt;T&gt;      | 1,215.56 ns |  1.00 | 0.2537 |    1597 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxRunningValueTaskT | RunningValueTask&lt;T&gt; | 7,117.17 ns |  5.67 | 0.5951 |    3755 B |        2.24 |
| KunetRunningValueTaskT | RunningValueTask&lt;T&gt; | 1,254.33 ns |  1.00 | 0.2651 |    1677 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxTask              | Task                | 3,158.44 ns | 65.02 | 0.1793 |    1140 B |        6.16 |
| KunetTask              | Task                |    48.58 ns |  1.00 | 0.0293 |     185 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxTaskT             | Task&lt;T&gt;             | 4,347.93 ns |  8.71 | 0.3281 |    2071 B |        2.63 |
| KunetTaskT             | Task&lt;T&gt;             |   499.03 ns |  1.00 | 0.1249 |     786 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxValueTask         | ValueTask           | 3,612.61 ns | 67.68 | 0.2022 |    1276 B |        5.12 |
| KunetValueTask         | ValueTask           |    53.37 ns |  1.00 | 0.0395 |     249 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxValueTaskT        | ValueTask&lt;T&gt;        | 4,596.25 ns |  8.52 | 0.3204 |    2055 B |        2.67 |
| KunetValueTaskT        | ValueTask&lt;T&gt;        |   539.78 ns |  1.00 | 0.1221 |     770 B |        1.00 |
|                        |                     |             |       |        |           |             |
| StakxVoid              | Void                | 1,111.00 ns | 32.64 | 0.0515 |     329 B |        3.16 |
| KunetVoid              | Void                |    34.04 ns |  1.00 | 0.0166 |     104 B |        1.00 |