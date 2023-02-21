using BenchmarkDotNet.Attributes;
using Castle.DynamicProxy;

namespace Kunet.AsyncInterceptor.PerfTests;

[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net462)]
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net60)]
[MemoryDiagnoser, CategoriesColumn]
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