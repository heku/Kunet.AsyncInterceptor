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
//| StakxRunningTaskT      | ShortRun-.NET 8.0             | .NET 8.0             | RunningTask<T>      | 2,090.08 ns | 0.1221 |    2104 B |
//| KunetRunningTaskT      | ShortRun-.NET 8.0             | .NET 8.0             | RunningTask<T>      |   401.26 ns | 0.0639 |    1072 B |
//| StakxRunningTaskT      | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningTask<T>      | 6,799.46 ns | 0.5493 |    3490 B |
//| KunetRunningTaskT      | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningTask<T>      | 1,513.07 ns | 0.2918 |    1837 B |
//| StakxRunningValueTaskT | ShortRun-.NET 8.0             | .NET 8.0             | RunningValueTask<T> | 2,897.58 ns | 0.1450 |    2440 B |
//| KunetRunningValueTaskT | ShortRun-.NET 8.0             | .NET 8.0             | RunningValueTask<T> |   432.29 ns | 0.0687 |    1152 B |
//| StakxRunningValueTaskT | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningValueTask<T> | 7,157.10 ns | 0.5951 |    3755 B |
//| KunetRunningValueTaskT | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | RunningValueTask<T> | 1,564.16 ns | 0.3033 |    1918 B |
//| StakxTask              | ShortRun-.NET 8.0             | .NET 8.0             | Task                |   922.91 ns | 0.0448 |     752 B |
//| KunetTask              | ShortRun-.NET 8.0             | .NET 8.0             | Task                |    58.15 ns | 0.0157 |     264 B |
//| StakxTask              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task                | 3,306.79 ns | 0.1793 |    1140 B |
//| KunetTask              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task                |   114.68 ns | 0.0459 |     289 B |
//| StakxTaskT             | ShortRun-.NET 8.0             | .NET 8.0             | Task<T>             | 1,360.26 ns | 0.0668 |    1144 B |
//| KunetTaskT             | ShortRun-.NET 8.0             | .NET 8.0             | Task<T>             |   166.35 ns | 0.0310 |     520 B |
//| StakxTaskT             | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task<T>             | 4,612.61 ns | 0.3586 |    2263 B |
//| KunetTaskT             | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Task<T>             |   592.44 ns | 0.1411 |     891 B |
//| StakxValueTask         | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask           | 1,764.23 ns | 0.0629 |    1080 B |
//| KunetValueTask         | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask           |    64.17 ns | 0.0196 |     328 B |
//| StakxValueTask         | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask           | 3,736.56 ns | 0.2022 |    1276 B |
//| KunetValueTask         | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask           |   125.07 ns | 0.0572 |     361 B |
//| StakxValueTaskT        | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask<T>        | 2,162.70 ns | 0.0877 |    1472 B |
//| KunetValueTaskT        | ShortRun-.NET 8.0             | .NET 8.0             | ValueTask<T>        |   184.36 ns | 0.0353 |     592 B |
//| StakxValueTaskT        | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask<T>        | 4,751.55 ns | 0.3204 |    2055 B |
//| KunetValueTaskT        | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | ValueTask<T>        |   632.87 ns | 0.1402 |     883 B |
//| StakxVoid              | ShortRun-.NET 8.0             | .NET 8.0             | Void                |    78.18 ns | 0.0062 |     104 B |
//| KunetVoid              | ShortRun-.NET 8.0             | .NET 8.0             | Void                |   152.02 ns | 0.0062 |     104 B |
//| StakxVoid              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Void                | 1,115.67 ns | 0.0515 |     329 B |
//| KunetVoid              | ShortRun-.NET Framework 4.8.1 | .NET Framework 4.8.1 | Void                | 1,511.98 ns | 0.0629 |     401 B |