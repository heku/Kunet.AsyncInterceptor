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
//| StakxRunningTaskT      | ShortRun-.NET 8.0             | .NET 8.0             | RunningTask<T>      | 2,011.65 ns | 0.1221 |    2104 B |
//| KunetRunningTaskT      | ShortRun-.NET 8.0             | .NET 8.0             | RunningTask<T>      |   401.71 ns | 0.0629 |    1056 B |
//| StakxRunningTaskT      | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningTask<T>      | 6,720.22 ns | 0.5493 |    3490 B |
//| KunetRunningTaskT      | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningTask<T>      | 1,494.21 ns | 0.2880 |    1821 B |
//| StakxRunningValueTaskT | ShortRun-.NET 8.0             | .NET 8.0             | RunningValueTask<T> | 2,881.59 ns | 0.1450 |    2440 B |
//| KunetRunningValueTaskT | ShortRun-.NET 8.0             | .NET 8.0             | RunningValueTask<T> |   432.41 ns | 0.0677 |    1136 B |
//| StakxRunningValueTaskT | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningValueTask<T> | 7,029.54 ns | 0.5951 |    3755 B |
//| KunetRunningValueTaskT | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningValueTask<T> | 1,659.73 ns | 0.3014 |    1902 B |
//| StakxTask              | ShortRun-.NET 8.0             | .NET 8.0             | Task                |   906.45 ns | 0.0448 |     752 B |
//| KunetTask              | ShortRun-.NET 8.0             | .NET 8.0             | Task                |    51.98 ns | 0.0157 |     264 B |
//| StakxTask              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task                | 3,237.15 ns | 0.1793 |    1140 B |
//| KunetTask              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task                |   108.91 ns | 0.0459 |     289 B |
//| StakxTaskT             | ShortRun-.NET 8.0             | .NET 8.0             | Task<T>             | 1,352.58 ns | 0.0648 |    1144 B |
//| KunetTaskT             | ShortRun-.NET 8.0             | .NET 8.0             | Task<T>             |   164.39 ns | 0.0310 |     520 B |
//| StakxTaskT             | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task<T>             | 4,407.14 ns | 0.3281 |    2071 B |
//| KunetTaskT             | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task<T>             |   585.02 ns | 0.1411 |     891 B |
//| StakxValueTask         | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask           | 1,722.44 ns | 0.0629 |    1080 B |
//| KunetValueTask         | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask           |    54.16 ns | 0.0196 |     328 B |
//| StakxValueTask         | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask           | 3,652.60 ns | 0.2022 |    1276 B |
//| KunetValueTask         | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask           |   121.72 ns | 0.0572 |     361 B |
//| StakxValueTaskT        | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask<T>        | 2,134.08 ns | 0.0877 |    1472 B |
//| KunetValueTaskT        | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask<T>        |   173.59 ns | 0.0353 |     592 B |
//| StakxValueTaskT        | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask<T>        | 4,612.15 ns | 0.3204 |    2055 B |
//| KunetValueTaskT        | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask<T>        |   609.95 ns | 0.1402 |     883 B |
//| StakxVoid              | ShortRun-.NET 8.0             | .NET 8.0             | Void                |    76.92 ns | 0.0062 |     104 B |
//| KunetVoid              | ShortRun-.NET 8.0             | .NET 8.0             | Void                |   144.37 ns | 0.0062 |     104 B |
//| StakxVoid              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Void                | 1,101.19 ns | 0.0515 |     329 B |
//| KunetVoid              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Void                | 1,488.42 ns | 0.0629 |     401 B |