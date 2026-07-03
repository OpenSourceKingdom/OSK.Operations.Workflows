using System;

namespace OSK.Operations.Workflows.Models;

/// <summary>
/// Represents an operation that is ran over multiple iterations within some workflow
/// </summary>
public interface IIterativeOperation
{
    /// <summary>
    /// The total number of work items that must be completed in order to be <see cref="OperationState.Complete"/>
    /// </summary>
    int TotalWorkItems { get; }

    /// <summary>
    /// Whether the operation has completed successfuly.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
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
    OperationState Iterate(TimeSpan deltaTime);
}
