using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Executors;

/// <summary>
/// Executes a collection of operations in a concurrent manner.
/// </summary>
public class OperationRunner: WorkflowOperation, IOperationRunner
{
    #region Variables

    private readonly Queue<TaskOperationDetails> _operations;

    private readonly OperationRunSettings _settings;
    private readonly List<TaskOperationDetails> _inProgressOperations = []; 
    private readonly List<ITaskOperation> _completedOperations = [];

    private int _totalFailedOperations = 0;

    #endregion

    #region Constructors

    /// <summary>
    /// Create a runner that uses the provided operations with default
    /// </summary>
    /// <param name="operations"></param>
    public OperationRunner(params ITaskOperation[] operations)
        : this(OperationRunSettings.Default(), operations)
    {
    }

    /// <summary>
    /// Create a runner that utilizes the provided operation details using the default run settings
    /// </summary>
    /// <param name="operations"></param>
    public OperationRunner(params TaskOperationDetails[] operations)
        : this(OperationRunSettings.Default(), operations)
    {
    }

    /// <summary>
    /// Create a runner that utilizes the provided run settings and operations with default operation details
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="operations"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OperationRunner(OperationRunSettings settings, params ITaskOperation[] operations)
        : this(settings, [.. operations.Select(operation => new TaskOperationDetails(operation))])
    {
    }

    /// <summary>
    /// Create a runner that utilizes the specified run settings and operation details
    /// </summary>
    /// <param name="settings">The settings to use with the runner</param>
    /// <param name="operations">The operation details</param>
    /// <exception cref="ArgumentNullException"></exception>
    public OperationRunner(OperationRunSettings settings, params TaskOperationDetails[] operations)
    {
        _operations = operations is null
            ? []
            : new(operations);
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        TotalWorkItems = operations?.Length ?? 0;
    }

    #endregion

    #region IOperationExecutor

    public event Action<TaskOperationDetails>? OnOperationFinished;

    #endregion

    #region IterativeOperation Overrides

    public override int TotalWorkItems { get; } 

    protected override OperationStatus RunIteration(TimeSpan deltaTime)
    {
        while (_inProgressOperations.Count < _settings.MaxConcurrenOperations && _operations.Count > 0)
        {
            var nextOperation = _operations.Dequeue();
            if (nextOperation is not null)
            {
                _inProgressOperations.Add(nextOperation);
            }
        }

        var progressUpdate = UpdateInProgressOperations(deltaTime);
        if (progressUpdate.ProgressError is not null)
        {
            return progressUpdate.ProgressError.Value;
        }

        if (_completedOperations.Count >= TotalWorkItems)
        {
            return OperationStatus.Complete;
        }

        var progressPercentage = (_completedOperations.Count + _totalFailedOperations + progressUpdate.ProgressPercentage) / TotalWorkItems;
        return OperationStatus.ProgressUpdate(progressPercentage, string.Empty);
    }

    #endregion

    #region Helpers

    private (double ProgressPercentage, OperationStatus? ProgressError) UpdateInProgressOperations(TimeSpan deltaTime)
    {
        double inProgressAggregatedPercentageChange = 0;
        for (var i = 0; i < _inProgressOperations.Count; i++)
        {
            var state = _inProgressOperations[i].Operation.Iterate(deltaTime);
            if (_inProgressOperations[i].Operation.IsFinished)
            {
                var finishedOperation = _inProgressOperations[i];
                _inProgressOperations.RemoveAt(i);

                if (state is not OperationState.Complete)
                {
                    _totalFailedOperations++;


                    continue;
                }

                _completedOperations.Add(finishedOperation.Operation);
                i--;

                OnOperationFinished?.Invoke(finishedOperation);
            }
            else
            {
                inProgressAggregatedPercentageChange += _inProgressOperations[i].Operation.Status.Progress;
            }
        }

        return (inProgressAggregatedPercentageChange, null);
    }

    #endregion
}
