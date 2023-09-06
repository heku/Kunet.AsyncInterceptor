using Castle.DynamicProxy;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal sealed class AsyncAdapterOfTask<T> : AsyncAdapter
{
    private readonly AsyncTaskMethodBuilder<T> _builder = AsyncTaskMethodBuilder<T>.Create();

    public AsyncAdapterOfTask(IInvocation invocation) : base(invocation) => Task = _builder.Task;

    protected override async ValueTask SetAsyncResult() => AsyncResult = await ((Task<T>)Invocation.ReturnValue).ConfigureAwait(false);

    public override object Task { get; }

    public override void Start<TStateMachine>(ref TStateMachine stateMachine) => _builder.Start(ref stateMachine);

    public override void SetResult(object result) => _builder.SetResult((T)result);

    public override void SetException(Exception exception) => _builder.SetException(exception);

    public override void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
}