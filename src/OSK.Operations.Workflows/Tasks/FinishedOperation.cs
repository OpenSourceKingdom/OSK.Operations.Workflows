using System;
using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Tasks;

/// <summary>
/// An operation that begins in a finished state. Useful for when an immediate response needs to be provided.
/// </summary>
/// <param name="state">The state the operation should have</param>
/// <param name="message">A descriptive message for the current state of the operation</param>
public class FinishedOperation(OperationState state = OperationState.Complete, string message = "") : IWorkflowOperation
{
    #region Static

    /// <summary>
    /// Represents a finished operation that succeeded
    /// </summary>
    public static FinishedOperation Success = new();

    /// <summary>
    /// Represents a finished operation that failed
    /// </summary>
    public static FinishedOperation Failure = new(OperationState.Failed);

    /// <summary>
    /// Represents a finished operation that was aborted
    /// </summary>
    public static FinishedOperation Aborted = new(OperationState.Aborted);

    #endregion

    #region IIterativeOperation

    public OperationStatus Status { get; } = state switch
    {
        OperationState.Complete => OperationStatus.Complete,
        OperationState.Failed => OperationStatus.Failed(message),
        OperationState.Aborted => OperationStatus.Aborted(message),
        _ => throw new InvalidOperationException("Operation status must be in a final status for a completed operation.")
    };

    public OperationState Run(TimeSpan delta)
        => Status.State;

    #endregion
}
