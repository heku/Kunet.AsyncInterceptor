using Castle.DynamicProxy;
using Moq;
using Xunit;

namespace Kunet.AsyncInterceptor.Tests;

public class AsyncInterceptorTests
{
    [Fact]
    public async Task InterceptByReturnValueFormatterTest()
    {
        var target = Mock.Of<IGet>(x =>
            x.Get<string>() == "value" &&
            x.GetTask<string>() == Task.FromResult("value") &&
            x.GetValueTask<string>() == new ValueTask<string>("value")
        );
        var proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(target, new ReturnValueFormatter("1 {0} 1"));

        Assert.Equal("1 value 1", proxy.Get<string>());
        Assert.Equal("1 value 1", await proxy.GetTask<string>());
        Assert.Equal("1 value 1", await proxy.GetValueTask<string>());
    }

    [Fact]
    public async Task InterceptByManyReturnValueFormattersTest()
    {
        var target = Mock.Of<IGet>(x =>
            x.Get<string>() == "value" &&
            x.GetTask<string>() == Task.FromResult("value") &&
            x.GetValueTask<string>() == new ValueTask<string>("value")
        );
        var proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(target, new ReturnValueFormatter("1 {0} 1"),
                                                                                new ReturnValueFormatter("2 {0} 2"),
                                                                                new ReturnValueFormatter("3 {0} 3"));

        Assert.Equal("1 2 3 value 3 2 1", proxy.Get<string>());
        Assert.Equal("1 2 3 value 3 2 1", await proxy.GetTask<string>());
        Assert.Equal("1 2 3 value 3 2 1", await proxy.GetValueTask<string>());
    }
}