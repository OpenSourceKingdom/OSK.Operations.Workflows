using System.Diagnostics.CodeAnalysis;

namespace OSK.Operations.Workflows.Models;

/// <summary>
/// Represents an <see cref="ITaskOperation"/> that also returns a result when it has finished successfully
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface ITaskOperation<TResult> : ITaskOperation
{
    /// <summary>
    /// Whether the operation has completed successfuly.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>An operation may not be successful because it is still running. To know its full state, check the <see cref="Status"/></item>
    /// </list>
    /// </remarks>
    [MemberNotNullWhen(true, nameof(Result))]
    new bool IsSuccessful => Status.State == OperationState.Complete;

    /// <summary>
    /// The operation's output of type <see cref="{TResult}"/>. This will only be set if the operation has finished successfully.
    /// </summary>
    TResult? Result { get; }
}
