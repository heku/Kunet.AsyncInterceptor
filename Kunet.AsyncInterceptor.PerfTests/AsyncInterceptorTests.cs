using BenchmarkDotNet.Attributes;
using Castle.DynamicProxy;

namespace Kunet.AsyncInterceptor.PerfTests;

[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net481)]
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
[MemoryDiagnoser, CategoriesColumn]
[HideColumns("Error", "StdDev", "Medium")]
public class AsyncInterceptorTests
{
    public IFoo StakxProxy { get; set; }
    public IFoo KunetProxy { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var generator = new ProxyGenerator();
        var target = new Foo();
        StakxProxy = generator.CreateInterfaceProxyWithTarget<IFoo>(target, new StakxInterceptor());
        KunetProxy = generator.CreateInterfaceProxyWithTarget<IFoo>(target, new KunetInterceptor());
    }

    [Benchmark]
    [BenchmarkCategory("Void")]
    public void StakxVoid() => StakxProxy.VoidMethod();

    [Benchmark]
    [BenchmarkCategory("Void")]
    public void KunetVoid() => KunetProxy.VoidMethod();

    [Benchmark]
    [BenchmarkCategory("Task")]
    public Task StakxTask() => StakxProxy.TaskMethod();

    [Benchmark]
    [BenchmarkCategory("Task")]
    public Task KunetTask() => KunetProxy.TaskMethod();

    [Benchmark]
    [BenchmarkCategory("Task<T>")]
    public Task<int> StakxTaskT() => StakxProxy.TaskMethod<int>();

    [Benchmark]
    [BenchmarkCategory("Task<T>")]
    public Task<int> KunetTaskT() => KunetProxy.TaskMethod<int>();

    [Benchmark]
    [BenchmarkCategory("RunningTask<T>")]
    public Task<int> StakxRunningTaskT()
    {
        var tcs = new TaskCompletionSource<object>();
        var task = StakxProxy.RunningTaskMethod<int>(tcs.Task);
        tcs.SetResult(null);
        return task;
    }

    [Benchmark]
    [BenchmarkCategory("RunningTask<T>")]
    public Task<int> KunetRunningTaskT()
    {
        var tcs = new TaskCompletionSource<object>();
        var task = KunetProxy.RunningTaskMethod<int>(tcs.Task);
        tcs.SetResult(null);
        return task;
    }

    [Benchmark]
    [BenchmarkCategory("ValueTask")]
    public ValueTask StakxValueTask() => StakxProxy.ValueTaskMethod();

    [Benchmark]
    [BenchmarkCategory("ValueTask")]
    public ValueTask KunetValueTask() => KunetProxy.ValueTaskMethod();

    [Benchmark]
    [BenchmarkCategory("ValueTask<T>")]
    public ValueTask<int> StakxValueTaskT() => StakxProxy.ValueTaskMethod<int>();

    [Benchmark]
    [BenchmarkCategory("ValueTask<T>")]
    public ValueTask<int> KunetValueTaskT() => KunetProxy.ValueTaskMethod<int>();

    [Benchmark]
    [BenchmarkCategory("RunningValueTask<T>")]
    public ValueTask<int> StakxRunningValueTaskT()
    {
        var tcs = new TaskCompletionSource<object>();
        var task = StakxProxy.RunningValueTaskMethod<int>(tcs.Task);
        tcs.SetResult(null);
        return task;
    }

    [Benchmark]
    [BenchmarkCategory("RunningValueTask<T>")]
    public ValueTask<int> KunetRunningValueTaskT()
    {
        var tcs = new TaskCompletionSource<object>();
        var task = KunetProxy.RunningValueTaskMethod<int>(tcs.Task);
        tcs.SetResult(null);
        return task;
    }
}



//| Method                 | Job                           | Runtime              | Categories          | Mean        | Gen0   | Allocated |
//|----------------------- |------------------------------ |--------------------- |-------------------- |------------:|-------:|----------:|
//| StakxRunningTaskT      | ShortRun-.NET 8.0             | .NET 8.0             | RunningTask<T>      | 1,988.02 ns | 0.1221 |    2104 B |
//| KunetRunningTaskT      | ShortRun-.NET 8.0             | .NET 8.0             | RunningTask<T>      |   304.62 ns | 0.0539 |     904 B |
//| StakxRunningTaskT      | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningTask<T>      | 6,746.47 ns | 0.5493 |    3490 B |
//| KunetRunningTaskT      | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningTask<T>      | 1,191.40 ns | 0.2537 |    1597 B |
//| StakxRunningValueTaskT | ShortRun-.NET 8.0             | .NET 8.0             | RunningValueTask<T> | 2,847.12 ns | 0.1450 |    2440 B |
//| KunetRunningValueTaskT | ShortRun-.NET 8.0             | .NET 8.0             | RunningValueTask<T> |   314.22 ns | 0.0587 |     984 B |
//| StakxRunningValueTaskT | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningValueTask<T> | 7,112.27 ns | 0.5951 |    3755 B |
//| KunetRunningValueTaskT | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningValueTask<T> | 1,230.46 ns | 0.2651 |    1677 B |
//| StakxTask              | ShortRun-.NET 8.0             | .NET 8.0             | Task                |   880.12 ns | 0.0448 |     752 B |
//| KunetTask              | ShortRun-.NET 8.0             | .NET 8.0             | Task                |    28.58 ns | 0.0110 |     184 B |
//| StakxTask              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task                | 3,242.71 ns | 0.1793 |    1140 B |
//| KunetTask              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task                |    45.88 ns | 0.0293 |     185 B |
//| StakxTaskT             | ShortRun-.NET 8.0             | .NET 8.0             | Task<T>             | 1,304.30 ns | 0.0668 |    1144 B |
//| KunetTaskT             | ShortRun-.NET 8.0             | .NET 8.0             | Task<T>             |   133.39 ns | 0.0262 |     440 B |
//| StakxTaskT             | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task<T>             | 4,425.31 ns | 0.3281 |    2071 B |
//| KunetTaskT             | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task<T>             |   490.25 ns | 0.1249 |     786 B |
//| StakxValueTask         | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask           | 1,671.27 ns | 0.0629 |    1080 B |
//| KunetValueTask         | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask           |    37.36 ns | 0.0148 |     248 B |
//| StakxValueTask         | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask           | 3,627.59 ns | 0.2022 |    1276 B |
//| KunetValueTask         | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask           |    53.79 ns | 0.0395 |     249 B |
//| StakxValueTaskT        | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask<T>        | 2,096.30 ns | 0.0877 |    1472 B |
//| KunetValueTaskT        | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask<T>        |   144.43 ns | 0.0300 |     504 B |
//| StakxValueTaskT        | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask<T>        | 4,605.92 ns | 0.3204 |    2055 B |
//| KunetValueTaskT        | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask<T>        |   499.61 ns | 0.1221 |     770 B |
//| StakxVoid              | ShortRun-.NET 8.0             | .NET 8.0             | Void                |    75.74 ns | 0.0062 |     104 B |
//| KunetVoid              | ShortRun-.NET 8.0             | .NET 8.0             | Void                |    18.77 ns | 0.0062 |     104 B |
//| StakxVoid              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Void                | 1,089.48 ns | 0.0515 |     329 B |
//| KunetVoid              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Void                |    33.64 ns | 0.0166 |     104 B |