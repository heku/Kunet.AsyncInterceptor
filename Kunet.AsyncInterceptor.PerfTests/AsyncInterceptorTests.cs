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
//| StakxRunningTaskT      | ShortRun-.NET 8.0             | .NET 8.0             | RunningTask<T>      | 2,123.04 ns | 0.1221 |    2104 B |
//| KunetRunningTaskT      | ShortRun-.NET 8.0             | .NET 8.0             | RunningTask<T>      |   429.42 ns | 0.0677 |    1136 B |
//| StakxRunningTaskT      | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningTask<T>      | 6,877.11 ns | 0.5798 |    3684 B |
//| KunetRunningTaskT      | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningTask<T>      | 1,499.10 ns | 0.3014 |    1902 B |
//| StakxRunningValueTaskT | ShortRun-.NET 8.0             | .NET 8.0             | RunningValueTask<T> | 2,877.30 ns | 0.1450 |    2440 B |
//| KunetRunningValueTaskT | ShortRun-.NET 8.0             | .NET 8.0             | RunningValueTask<T> |   436.08 ns | 0.0725 |    1216 B |
//| StakxRunningValueTaskT | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningValueTask<T> | 7,132.29 ns | 0.5951 |    3755 B |
//| KunetRunningValueTaskT | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningValueTask<T> | 1,564.72 ns | 0.3147 |    1982 B |
//| StakxTask              | ShortRun-.NET 8.0             | .NET 8.0             | Task                |   940.71 ns | 0.0448 |     752 B |
//| KunetTask              | ShortRun-.NET 8.0             | .NET 8.0             | Task                |    77.63 ns | 0.0196 |     328 B |
//| StakxTask              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task                | 3,320.81 ns | 0.1793 |    1140 B |
//| KunetTask              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task                |   166.57 ns | 0.0560 |     353 B |
//| StakxTaskT             | ShortRun-.NET 8.0             | .NET 8.0             | Task<T>             | 1,352.91 ns | 0.0668 |    1144 B |
//| KunetTaskT             | ShortRun-.NET 8.0             | .NET 8.0             | Task<T>             |   168.32 ns | 0.0348 |     584 B |
//| StakxTaskT             | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task<T>             | 4,475.84 ns | 0.3281 |    2071 B |
//| KunetTaskT             | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task<T>             |   601.14 ns | 0.1516 |     955 B |
//| StakxValueTask         | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask           | 1,832.72 ns | 0.0629 |    1080 B |
//| KunetValueTask         | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask           |    86.66 ns | 0.0234 |     392 B |
//| StakxValueTask         | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask           | 3,652.26 ns | 0.2022 |    1276 B |
//| KunetValueTask         | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask           |   162.22 ns | 0.0675 |     425 B |
//| StakxValueTaskT        | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask<T>        | 2,110.97 ns | 0.0877 |    1472 B |
//| KunetValueTaskT        | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask<T>        |   187.48 ns | 0.0391 |     656 B |
//| StakxValueTaskT        | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask<T>        | 4,636.58 ns | 0.3204 |    2055 B |
//| KunetValueTaskT        | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask<T>        |   613.35 ns | 0.1497 |     947 B |
//| StakxVoid              | ShortRun-.NET 8.0             | .NET 8.0             | Void                |    76.13 ns | 0.0062 |     104 B |
//| KunetVoid              | ShortRun-.NET 8.0             | .NET 8.0             | Void                |   141.13 ns | 0.0062 |     104 B |
//| StakxVoid              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Void                | 1,107.38 ns | 0.0515 |     329 B |
//| KunetVoid              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Void                | 1,451.33 ns | 0.0629 |     401 B |