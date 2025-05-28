using Castle.DynamicProxy;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal sealed class AsyncAdapterOfValueTask<T>(IInvocation invocation) : AsyncAdapter(invocation, SetAsyncResult)
{
    private static async ValueTask SetAsyncResult(IAsyncInvocation i) => i.AsyncResult = await ((ValueTask<T>)i.Invocation.ReturnValue).ConfigureAwait(false);

    public override object ConvertToReturnTask(ValueTask interceptingTask) => ConvertToValueTask(interceptingTask);

    private async ValueTask<T> ConvertToValueTask(ValueTask interceptingTask)
    {
        await interceptingTask.ConfigureAwait(false);
        return (T)(AsyncResult ?? default(T));
    }
}