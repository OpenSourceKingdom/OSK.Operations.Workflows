using System;

namespace OSK.Operations.Workflows.Models;

/// <summary>
/// Represents an operation that is ran over multiple iterations within some workflow
/// </summary>
public interface IWorkflowOperation
{
    /// <summary>
    /// Whether the operation has completed successfuly.
    /// </summary>
    /// <remarks>
    /// 💡Notes:.ma
    /// <list type="bullet">
    /// <item>An operation may not be successful because it is still running. To know its full state, check the <see cref="Status"/></item>
    /// </list>
    /// </remarks>
    bool IsSuccessful => Status.State == OperationState.Complete;

    /// <summary>
    /// Whether the operation has finished running through all required iterations.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>An operation may be finished running, but it could be in one of a variety states. To know its full state, check the <see cref="Status"/></item>
    /// </list>
    /// </remarks>
    bool IsFinished => Status.State != OperationState.InProgress || Status.State != OperationState.NotStarted;

    /// <summary>
    /// The current status of the operation. Contains extra information that may be helpful for more context on the operation
    /// </summary>
    OperationStatus Status { get; }

    /// <summary>
    /// Runs a single iteration of the operation.
    /// </summary>
    /// <param name="deltaTime">The time since the last iteration of the operation</param>
    /// <returns>The current <see cref="OperationState"/> of the operation</returns>
    OperationState Run(TimeSpan deltaTime);
}
