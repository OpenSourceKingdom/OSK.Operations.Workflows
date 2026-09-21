using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Operations.Workflows.Events;
using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Options;
using OSK.Operations.Workflows.Ports;

namespace OSK.Operations.Workflows.Runners;

/// <summary>
/// Executes a collection of operations in a concurrent manner.
/// </summary>
public class OperationRunner: IterativeOperation, IOperationRunner
{
    #region Variables

    private readonly Queue<WorkflowTaskOperationDetails> _operations;

    private readonly OperationRunOptions _settings;
    private readonly List<WorkflowTaskOperationDetails> _inProgressOperations = []; 
    private readonly List<ITaskOperation> _completedOperations = [];

    private int _totalFailedOperations = 0;

    #endregion

    #region Constructors

    /// <summary>
    /// Create a runner that uses the provided operations with default
    /// </summary>
    /// <param name="operations"></param>
    public OperationRunner(params ITaskOperation[] operations)
        : this(OperationRunOptions.Default(), operations)
    {
    }

    /// <summary>
    /// Create a runner that utilizes the provided operation details using the default run settings
    /// </summary>
    /// <param name="operations"></param>
    public OperationRunner(params WorkflowTaskOperationDetails[] operations)
        : this(OperationRunOptions.Default(), operations)
    {
    }

    /// <summary>
    /// Create a runner that utilizes the provided run settings and operations with default operation details
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="operations"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OperationRunner(OperationRunOptions settings, params ITaskOperation[] operations)
        : this(settings, operations is null ? [] : [.. operations.Select(operation => new WorkflowTaskOperationDetails(operation))])
    {
    }

    /// <summary>
    /// Create a runner that utilizes the specified run settings and operation details
    /// </summary>
    /// <param name="settings">The settings to use with the runner</param>
    /// <param name="operations">The operation details</param>
    /// <exception cref="ArgumentNullException"></exception>
    public OperationRunner(OperationRunOptions settings, params WorkflowTaskOperationDetails[] operations)
    {
        _operations = operations is null
            ? []
            : new(operations);
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        TotalWorkItems = operations?.Length ?? 0;
    }

    #endregion

    #region IOperationExecutor

    /// <inheritdoc/>
    public event Action<TaskOperationFinishedEvent>? OnOperationFinished;

    #endregion

    #region IterativeOperation Overrides

    /// <inheritdoc/>
    public override int TotalWorkItems { get; }

    /// <inheritdoc/>
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

        if (_completedOperations.Count + _totalFailedOperations >= TotalWorkItems)
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

                OnOperationFinished?.Invoke(new TaskOperationFinishedEvent(finishedOperation.Operation, finishedOperation.Id));

                if (state is not OperationState.Complete)
                {
                    _totalFailedOperations++;
                    
                    if (!finishedOperation.RunSettings.IgnoreFailure)
                    {
                        return (0, finishedOperation.Operation.Status);
                    }
                    continue;
                }

                _completedOperations.Add(finishedOperation.Operation);
                i--;
            }
            else
            {
                inProgressAggregatedPercentageChange += _inProgressOperations[i].Operation.Status.Progress;
            }
        }

        return (_inProgressOperations.Count is 0 ? 0 : inProgressAggregatedPercentageChange / _inProgressOperations.Count, null);
    } 

    #endregion
}
