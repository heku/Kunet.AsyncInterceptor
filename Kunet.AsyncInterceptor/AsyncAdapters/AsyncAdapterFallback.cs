using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal sealed class AsyncAdapterFallback : AsyncAdapter
{
    private readonly Type _builderType;
    private readonly object _builder;

    public AsyncAdapterFallback(Type builderType, IInvocation invocation) : base(invocation)
    {
        _builderType = builderType;
        _builder = builderType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
        Task = _builderType.GetProperty("Task", BindingFlags.Public | BindingFlags.Instance).GetValue(_builder);
    }

    protected override ValueTask SetAsyncResult()
    {
        var awaiter = Invocation.ReturnValue.GetType().GetMethod("GetAwaiter", BindingFlags.Public | BindingFlags.Instance).Invoke(Invocation.ReturnValue, null);
        AsyncResult = awaiter.GetType().GetMethod("GetResult", BindingFlags.Public | BindingFlags.Instance).Invoke(awaiter, null);
        return default;
    }

    public override object Task { get; }

    public override void Start<TStateMachine>(ref TStateMachine stateMachine)
    {
        var method = _builderType.GetMethod("Start", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(stateMachine.GetType());
        method.Invoke(_builder, new object[] { stateMachine });
    }

    public override void SetResult(object result)
    {
        var method = _builderType.GetMethod("SetResult", BindingFlags.Public | BindingFlags.Instance);
        method.Invoke(_builder, new[] { result });
    }

    public override void SetException(Exception exception)
    {
        _builderType.GetMethod("SetException", BindingFlags.Public | BindingFlags.Instance).Invoke(_builder, new object[] { exception });
    }

    public override void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
    {
        var method = _builderType.GetMethod("AwaitUnsafeOnCompleted", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(awaiter.GetType(), stateMachine.GetType());
        method.Invoke(_builder, new object[] { awaiter, stateMachine });
    }
}