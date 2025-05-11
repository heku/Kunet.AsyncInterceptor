using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal abstract partial class AsyncAdapter(IInvocation invocation, Func<IAsyncInvocation, ValueTask> asyncResultSetter) : AsyncInvocation(invocation, asyncResultSetter)
{
    /// <summary>Convert <see cref="ValueTask"/> to original return task.</summary>
    public abstract object ConvertToReturnTask(ValueTask interceptingTask);
}