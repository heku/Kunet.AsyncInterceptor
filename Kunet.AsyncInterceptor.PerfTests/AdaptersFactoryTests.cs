using BenchmarkDotNet.Attributes;
using Moq;
using IInvocation = Castle.DynamicProxy.IInvocation;

namespace Kunet.AsyncInterceptor.PerfTests;

[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net462)]
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class AdaptersFactoryTests
{
    public IInvocation Invocation { get; } = Mock.Of<IInvocation>(x => x.Method.ReturnType == typeof(Task<string>));

    [Benchmark]
    public void NewFactory() => AdaptersFactory.TryCreate(Invocation, out _, out _);

    [Benchmark]
    public void OldFactory() => LegacyFactory.TryCreate(Invocation, out _, out _);

    internal static class LegacyFactory
    {
        public static bool TryCreate(IInvocation invocation, out IAsyncTaskBuilder builder, out IAsyncInvocation asyncInvocation)
        {
            var returnType = invocation.Method.ReturnType;
            if (returnType == typeof(Task))
            {
                builder = new AsyncTaskBuilderOfTask();
                asyncInvocation = new AsyncInvocationOfTask(invocation);
                return true;
            }
            if (returnType == typeof(ValueTask))
            {
                builder = new AsyncTaskBuilderOfValueTask();
                asyncInvocation = new AsyncInvocationOfValueTask(invocation);
                return true;
            }
            if (returnType.IsGenericType && returnType.GenericTypeArguments.Length == 1)
            {
                var genericType = returnType.GetGenericTypeDefinition();
                var argumentType = returnType.GenericTypeArguments[0];
                if (genericType == typeof(Task<>))
                {
                    builder = CreateAsyncTaskBuilder(typeof(AsyncTaskBuilderOfTask<>), argumentType);
                    asyncInvocation = CreateAsyncInvocation(typeof(AsyncInvocationOfTask<>), argumentType, invocation);
                    return true;
                }
                if (genericType == typeof(ValueTask<>))
                {
                    builder = CreateAsyncTaskBuilder(typeof(AsyncTaskBuilderOfValueTask<>), argumentType);
                    asyncInvocation = CreateAsyncInvocation(typeof(AsyncInvocationOfValueTask<>), argumentType, invocation);
                    return true;
                }
            }
            builder = null;
            asyncInvocation = null;
            return false;
        }

        private static IAsyncTaskBuilder CreateAsyncTaskBuilder(Type genericType, Type argumentType)
        {
            var closedType = genericType.MakeGenericType(argumentType);
            return (IAsyncTaskBuilder)Activator.CreateInstance(closedType);
        }

        private static IAsyncInvocation CreateAsyncInvocation(Type genericType, Type argumentType, IInvocation invocation)
        {
            var closedType = genericType.MakeGenericType(argumentType);
            return (IAsyncInvocation)Activator.CreateInstance(closedType, invocation);
        }
    }
}