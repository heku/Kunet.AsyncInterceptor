using BenchmarkDotNet.Attributes;
using Moq;
using IInvocation = Castle.DynamicProxy.IInvocation;

namespace Kunet.AsyncInterceptor.PerfTests;

[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net481)]
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[HideColumns("Error", "StdDev", "Medium")]
public class AdaptersFactoryTests
{
    public IInvocation Invocation { get; } = Mock.Of<IInvocation>(x => x.Method.ReturnType == typeof(Task<string>));

    [Benchmark]
    public void NewFactory() => AsyncAdapter.TryCreate(Invocation, out _);

    [Benchmark]
    public void OldFactory() => LegacyFactory.TryCreate(Invocation, out _);

    internal static class LegacyFactory
    {
        public static bool TryCreate(IInvocation invocation, out IAsyncAdapter adapter)
        {
            var returnType = invocation.Method.ReturnType;
            if (returnType == typeof(Task))
            {
                adapter = new AsyncAdapterOfTask(invocation);
                return true;
            }
            if (returnType == typeof(ValueTask))
            {
                adapter = new AsyncAdapterOfValueTask(invocation);
                return true;
            }
            if (returnType.IsGenericType && returnType.GenericTypeArguments.Length == 1)
            {
                var genericType = returnType.GetGenericTypeDefinition();
                var argumentType = returnType.GenericTypeArguments[0];
                if (genericType == typeof(Task<>))
                {
                    adapter = CreateAsyncAdapter(typeof(AsyncAdapterOfTask<>), argumentType, invocation);
                    return true;
                }
                if (genericType == typeof(ValueTask<>))
                {
                    adapter = CreateAsyncAdapter(typeof(AsyncAdapterOfValueTask<>), argumentType, invocation);
                    return true;
                }
            }
            adapter = null;
            return false;
        }

        private static IAsyncAdapter CreateAsyncAdapter(Type genericType, Type argumentType, IInvocation invocation)
        {
            var closedType = genericType.MakeGenericType(argumentType);
            return (IAsyncAdapter)Activator.CreateInstance(closedType, invocation);
        }
    }
}