using System;
using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows;

/// <summary>
/// A simple workflow operation that represents an iterative task to help with implementing other <see cref="IIterativeOperation"/>
/// </summary>
public abstract class IterativeOperation : IIterativeOperation
{
    #region Variables

    private bool _initialized;

    #endregion

    #region IWorkflowOperation

    /// <inheritdoc/>
    public abstract int TotalWorkItems { get; }

    /// <inheritdoc/>
    public bool IsSuccessful => Status.State == OperationState.Complete;

    /// <inheritdoc/>
    public bool IsFinished => Status.State != OperationState.InProgress && Status.State != OperationState.NotStarted;

    /// <inheritdoc/>
    public OperationStatus Status { get; private set; } = OperationStatus.NotStarted;

    /// <inheritdoc/>
    public OperationState Iterate(TimeSpan deltaTime)
    {
        if (IsFinished)
        {
            return Status.State;
        }
        if (_initialized is false)
        {
            Initialize();
            _initialized = true;
        }

        Status = RunIteration(deltaTime);
        return Status.State;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Ran once, during the first iteration, to allow the operation to perform any required setup separate from the normal iterations
    /// </summary>
    protected virtual void Initialize()
    {
    }

    /// <summary>
    /// Runs every iteration and should handle the core requirements of the operation logic. 
    /// </summary>
    /// <param name="delta">The time delta since the last iteration</param>
    /// <returns>The status of the operation after iterating</returns>
    protected abstract OperationStatus RunIteration(TimeSpan delta);

    #endregion
}
