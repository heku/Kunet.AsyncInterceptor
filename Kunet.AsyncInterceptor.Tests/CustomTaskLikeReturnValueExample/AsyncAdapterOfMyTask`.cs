using Castle.DynamicProxy;

namespace Kunet.AsyncInterceptor.Tests;

// You must provide this to set AsyncResult from Invocation.ReturnValue
// You must provide this to adapt your custom TaskMethodBuilder to IAsyncTaskBuilder

internal sealed class AsyncAdapterOfMyTask<T> : AsyncAdapter
{
    private readonly MyTaskMethodBuilder<T> _builder = MyTaskMethodBuilder<T>.Create();

    public AsyncAdapterOfMyTask(IInvocation invocation) : base(invocation) => Task = _builder.Task;

    protected override async ValueTask SetAsyncResult() => AsyncResult = await (MyTask<T>)Invocation.ReturnValue;

    public override object Task { get; }

    public override void Start<TStateMachine>(ref TStateMachine stateMachine) => _builder.Start(ref stateMachine);

    public override void SetException(Exception ex) => _builder.SetException(ex);

    public override void SetResult(object result) => _builder.SetResult((T)result);

    public override void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
}