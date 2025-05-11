using Castle.DynamicProxy;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal sealed class AsyncAdapterOfTask<T>(IInvocation invocation) : AsyncAdapter(invocation, SetAsyncResult)
{
    private static async ValueTask SetAsyncResult(IAsyncInvocation i) => i.AsyncResult = await ((Task<T>)i.Invocation.ReturnValue).ConfigureAwait(false);

    public override object ConvertToReturnTask(ValueTask interceptingTask) => ConvertToTask(interceptingTask);

    public async Task<T> ConvertToTask(ValueTask interceptingTask)
    {
        await interceptingTask.ConfigureAwait(false);
        return (T)(AsyncResult ?? default(T));
    }
}