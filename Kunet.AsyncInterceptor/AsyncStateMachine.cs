using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

internal struct AsyncStateMachine : IAsyncStateMachine
{
    private readonly AsyncAdapter _adapter;
    private readonly Func<IAsyncInvocation, ValueTask> _interceptAsync;
    private ValueTask? _interceptingTask;

    public AsyncStateMachine(AsyncAdapter adapter, Func<IAsyncInvocation, ValueTask> interceptAsync)
    {
        _adapter = adapter;
        _interceptAsync = interceptAsync;
        _interceptingTask = null;
    }

    public void MoveNext()
    {
        try
        {
            _interceptingTask ??= _interceptAsync(_adapter);
            var awaiter = _interceptingTask.Value.GetAwaiter();
            if (awaiter.IsCompleted)
            {
                awaiter.GetResult(); // throw exception if there is.
                _adapter.SetResult(_adapter.AsyncResult);
            }
            else
            {
                _adapter.AwaitUnsafeOnCompleted(ref awaiter, ref this);
            }
        }
        catch (Exception ex)
        {
            _adapter.SetException(ex);
        }
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        // SetStateMachine was originally needed in order to store the boxed state machine reference into the boxed copy.
        // Now that a normal box is no longer used, SetStateMachine is also legacy. We need not do anything here.
        // https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Runtime/CompilerServices/AsyncMethodBuilderCore.cs,70528b49b7e9916f
    }
}