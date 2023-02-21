## Kunet.AsyncInterceptor

Inspired by [stakx.DynamicProxy.AsyncInterceptor](https://github.com/stakx/DynamicProxy.AsyncInterceptor), but rewrite with less reflection.

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

## Performance

|                 Method |         Runtime |         Mean |         Error |       StdDev |   Gen0 | Allocated |
|----------------------- |---------------- |-------------:|--------------:|-------------:|-------:|----------:|
|      StakxRunningTaskT |        .NET 6.0 |  6,529.53 ns | 19,472.076 ns | 1,067.330 ns | 0.3357 |    2865 B |
|      KunetRunningTaskT |        .NET 6.0 |  1,219.14 ns |    469.659 ns |    25.744 ns | 0.1736 |    1464 B |
|      StakxRunningTaskT | Framework 4.7.2 | 14,345.88 ns | 23,519.098 ns | 1,289.161 ns | 0.5798 |    3685 B |
|      KunetRunningTaskT | Framework 4.7.2 |  3,126.01 ns |  3,032.443 ns |   166.218 ns | 0.3052 |    1934 B |
| StakxRunningValueTaskT |        .NET 6.0 |  8,419.00 ns |  6,195.064 ns |   339.572 ns | 0.3510 |    3025 B |
| KunetRunningValueTaskT |        .NET 6.0 |  1,348.40 ns |    874.297 ns |    47.923 ns | 0.1831 |    1544 B |
| StakxRunningValueTaskT | Framework 4.7.2 | 14,897.44 ns |  4,276.208 ns |   234.393 ns | 0.5951 |    3756 B |
| KunetRunningValueTaskT | Framework 4.7.2 |  2,640.64 ns |  3,287.681 ns |   180.209 ns | 0.3166 |    2014 B |
|              StakxTask |        .NET 6.0 |  2,753.29 ns |    213.686 ns |    11.713 ns | 0.1183 |     992 B |
|              KunetTask |        .NET 6.0 |    195.84 ns |    138.319 ns |     7.582 ns | 0.0410 |     344 B |
|              StakxTask | Framework 4.7.2 |  5,840.52 ns |  1,697.348 ns |    93.037 ns | 0.1755 |    1140 B |
|              KunetTask | Framework 4.7.2 |    340.55 ns |     66.936 ns |     3.669 ns | 0.0587 |     369 B |
|             StakxTaskT |        .NET 6.0 |  4,450.06 ns |  2,917.884 ns |   159.939 ns | 0.1907 |    1617 B |
|             KunetTaskT |        .NET 6.0 |    618.92 ns |    243.618 ns |    13.354 ns | 0.0992 |     832 B |
|             StakxTaskT | Framework 4.7.2 |  8,537.17 ns |  6,192.413 ns |   339.427 ns | 0.3204 |    2071 B |
|             KunetTaskT | Framework 4.7.2 |  1,102.44 ns |    599.739 ns |    32.874 ns | 0.1526 |     971 B |
|         StakxValueTask |        .NET 6.0 |  4,333.59 ns |  4,807.079 ns |   263.492 ns | 0.1297 |    1144 B |
|         KunetValueTask |        .NET 6.0 |    198.82 ns |      6.866 ns |     0.376 ns | 0.0486 |     408 B |
|         StakxValueTask | Framework 4.7.2 |  7,505.78 ns | 11,502.047 ns |   630.466 ns | 0.1984 |    1276 B |
|         KunetValueTask | Framework 4.7.2 |    332.45 ns |    138.024 ns |     7.566 ns | 0.0701 |     441 B |
|        StakxValueTaskT |        .NET 6.0 |  5,526.53 ns |  2,792.015 ns |   153.040 ns | 0.2289 |    1944 B |
|        KunetValueTaskT |        .NET 6.0 |    705.00 ns |    634.545 ns |    34.782 ns | 0.1078 |     904 B |
|        StakxValueTaskT | Framework 4.7.2 |  9,285.49 ns |  7,286.716 ns |   399.410 ns | 0.3204 |    2055 B |
|        KunetValueTaskT | Framework 4.7.2 |  1,172.79 ns |    513.807 ns |    28.163 ns | 0.1526 |     963 B |
|              StakxVoid |        .NET 6.0 |    288.92 ns |     83.023 ns |     4.551 ns | 0.0124 |     104 B |
|              KunetVoid |        .NET 6.0 |     56.54 ns |     44.619 ns |     2.446 ns | 0.0124 |     104 B |
|              StakxVoid | Framework 4.7.2 |  2,114.39 ns |  3,188.952 ns |   174.797 ns | 0.0496 |     329 B |
|              KunetVoid | Framework 4.7.2 |     85.95 ns |     71.254 ns |     3.906 ns | 0.0165 |     104 B |