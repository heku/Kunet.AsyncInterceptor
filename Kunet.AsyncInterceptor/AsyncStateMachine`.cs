using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Kunet.AsyncInterceptor;

[StructLayout(LayoutKind.Auto)]
internal struct AsyncStateMachine<T> : IAsyncStateMachine where T : IAsyncAdapter
{
    private readonly T _adapter;
    private ValueTaskAwaiter _awaiter;

    public AsyncStateMachine(in T adapter, in ValueTaskAwaiter awaiter)
    {
        _adapter = adapter;
        _awaiter = awaiter;
    }

    public void MoveNext()
    {
        try
        {
            if (_awaiter.IsCompleted)
            {
                _awaiter.GetResult(); // throw exception if there is.
                _adapter.SetResult(_adapter.AsyncResult);
            }
            else
            {
                _adapter.AwaitUnsafeOnCompleted(ref _awaiter, ref this);
            }
        }
        catch (Exception ex)
        {
            _adapter.SetException(ex);
        }
    }

    [Obsolete]
    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        // SetStateMachine was originally needed in order to store the boxed state machine reference into the boxed copy.
        // Now that a normal box is no longer used, SetStateMachine is also legacy. We need not do anything here.
        // https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Runtime/CompilerServices/AsyncMethodBuilderCore.cs,70528b49b7e9916f
    }
}