using System.Diagnostics.CodeAnalysis;
using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows;

/// <summary>
/// A base class for iterative operations to help with implementing other <see cref="ITaskOperation{TResult}"/>
/// </summary>
public abstract class TaskOperation<TResult> : IterativeOperation, ITaskOperation<TResult>
{
    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Result))]
    public new bool IsSuccessful => Status.State == OperationState.Complete;

    /// <inheritdoc/>
    public abstract TResult? Result { get; protected set; }
}
