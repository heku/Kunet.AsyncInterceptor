using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kunet.AsyncInterceptor;

/// <summary>
/// <inheritdoc cref="IInvocation"/>
/// </summary>
public interface IAsyncInvocation
{
    /// <summary>
    /// <inheritdoc cref="IInvocation"/>
    /// </summary>
    /// <remarks>
    /// Do not use <see cref="IInvocation.ReturnValue"/>, use <see cref="AsyncResult"/> instead.<br/>
    /// Do not use <see cref="IInvocation.Proceed"/>, use <see cref="ProceedAsync"/> instead.
    /// </remarks>
    IInvocation Invocation { get; }

    /// <summary>
    /// <para>Replacement of <see cref="IInvocation.ReturnValue"/> in asynchronous mode.</para>
    /// <code>AsyncResult = <see langword="await"/> Invocation.ReturnValue</code>
    /// <inheritdoc cref="IInvocation.ReturnValue"/>
    /// </summary>
    object AsyncResult { get; set; }

    /// <summary>
    /// <para>Replacement of <see cref="IInvocation.Proceed"/> in asynchronous mode.</para>
    /// <code>Invocation.ReturnValue = NEXT()</code>
    /// <code>AsyncResult = <see langword="await"/> Invocation.ReturnValue</code>
    /// <inheritdoc cref="IInvocation.Proceed"/>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="IInvocation.Proceed"/>
    /// </remarks>
    ValueTask ProceedAsync();
}