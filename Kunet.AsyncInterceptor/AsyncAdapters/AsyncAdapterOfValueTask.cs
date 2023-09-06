using Castle.DynamicProxy;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal sealed class AsyncAdapterOfValueTask : AsyncAdapter
{
    private readonly AsyncValueTaskMethodBuilder _builder = AsyncValueTaskMethodBuilder.Create();

    public AsyncAdapterOfValueTask(IInvocation invocation) : base(invocation) => Task = _builder.Task;

    protected override ValueTask SetAsyncResult() => (ValueTask)Invocation.ReturnValue;

    public override object Task { get; }

    public override void Start<TStateMachine>(ref TStateMachine stateMachine) => _builder.Start(ref stateMachine);

    public override void SetResult(object result) => _builder.SetResult();

    public override void SetException(Exception exception) => _builder.SetException(exception);

    public override void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
}