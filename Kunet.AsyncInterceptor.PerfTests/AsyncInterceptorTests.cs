using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Castle.DynamicProxy;

namespace Kunet.AsyncInterceptor.PerfTests;

[ShortRunJob(RuntimeMoniker.Net481)]
[ShortRunJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser, CategoriesColumn]
[HideColumns("Error", "StdDev", "Medium", "RatioSD", "Job")]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByJob, BenchmarkLogicalGroupRule.ByCategory)]
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

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Void")]
    public void KunetVoid() => KunetProxy.VoidMethod();

    [Benchmark]
    [BenchmarkCategory("Task")]
    public Task StakxTask() => StakxProxy.TaskMethod();

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Task")]
    public Task KunetTask() => KunetProxy.TaskMethod();

    [Benchmark]
    [BenchmarkCategory("Task<T>")]
    public Task<int> StakxTaskT() => StakxProxy.TaskMethod<int>();

    [Benchmark(Baseline = true)]
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

    [Benchmark(Baseline = true)]
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

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("ValueTask")]
    public ValueTask KunetValueTask() => KunetProxy.ValueTaskMethod();

    [Benchmark]
    [BenchmarkCategory("ValueTask<T>")]
    public ValueTask<int> StakxValueTaskT() => StakxProxy.ValueTaskMethod<int>();

    [Benchmark(Baseline = true)]
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

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("RunningValueTask<T>")]
    public ValueTask<int> KunetRunningValueTaskT()
    {
        var tcs = new TaskCompletionSource<object>();
        var task = KunetProxy.RunningValueTaskMethod<int>(tcs.Task);
        tcs.SetResult(null);
        return task;
    }
}