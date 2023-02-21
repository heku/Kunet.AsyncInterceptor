using Castle.DynamicProxy;
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
    /// Do NOT use <see cref="IInvocation.ReturnValue"/>, use <see cref="AsyncResult"/> instead.<br/>
    /// Do NOT use <see cref="IInvocation.Proceed"/>, use <see cref="ProceedAsync"/> instead.
    /// </remarks>
    IInvocation Invocation { get; }

    /// <summary>
    /// Replacement of <see cref="IInvocation.ReturnValue"/> in asynchronous mode.
    /// </summary>
    object AsyncResult { get; set; }

    /// <summary>
    /// Replacement of <see cref="IInvocation.Proceed"/> in asynchronous mode.
    /// </summary>
    ValueTask ProceedAsync();
}