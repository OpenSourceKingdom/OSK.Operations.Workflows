using System;
using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows;

/// <summary>
/// A simple workflow operation that represents an iterative task to help with implementing other <see cref="IWorkflowOperation"/>
/// </summary>
public abstract class WorkflowOperation : IWorkflowOperation
{
    #region Variables

    private bool _initialized;

    #endregion

    #region IIterativeOperation

    /// <inheritdoc/>
    public bool IsSuccessful => Status.State == OperationState.Complete;

    /// <inheritdoc/>
    public bool IsFinished => Status.State != OperationState.InProgress || Status.State != OperationState.NotStarted;

    /// <inheritdoc/>
    public OperationStatus Status { get; private set; } = OperationStatus.NotStarted;

    /// <inheritdoc/>
    public OperationState Run(TimeSpan deltaTime)
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

    protected virtual void Initialize()
    {
    }

    protected abstract OperationStatus RunIteration(TimeSpan delta);

    #endregion
}
