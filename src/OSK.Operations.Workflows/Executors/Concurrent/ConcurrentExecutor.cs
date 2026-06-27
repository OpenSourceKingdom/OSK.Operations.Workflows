using System;
using System.Collections.Generic;
using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Executors.Concurrent;

/// <summary>
/// Executes a collection of operations in a concurrent manner.
/// </summary>
public class ConcurrentExecutor: WorkflowOperation, IOperationExecutor
{
    #region Variables

    private readonly Queue<Func<ITaskOperation>> _operationFactories;

    private int _totalOperations;

    private readonly ConcurrentExecutionOptions _options;
    private readonly List<ITaskOperation> _inProgressOperations = [];
    private readonly List<ITaskOperation> _completedOperations = [];

    private int _totalFailedOperationsSkipped = 0;

    #endregion

    #region Constructors

    /// <summary>
    /// Create a concurrent executor that utilizes the provided operations with the given delay operation factories
    /// </summary>
    /// <param name="options"></param>
    /// <param name="operationFactories"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ConcurrentExecutor(ConcurrentExecutionOptions options, params Func<ITaskOperation>[] operationFactories)
    {
        _operationFactories = operationFactories is null
            ? []
            : new(operationFactories);
        _options = options ?? throw new ArgumentNullException(nameof(options));

        _totalOperations = operationFactories?.Length ?? 0;
    }

    #endregion

    #region IOperationExecutor

    public event Action<ITaskOperation>? OnOperationFinished;

    #endregion

    #region IterativeOperation Overrides

    protected override OperationStatus RunIteration(TimeSpan deltaTime)
    {
        while (_inProgressOperations.Count < _operationFactories.Count && _operationFactories.Count > 0)
        {
            var nextOperation = _operationFactories.Dequeue().Invoke();
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

        if (_completedOperations.Count >= _totalOperations)
        {
            return OperationStatus.Complete;
        }

        var progressPercentage = (_completedOperations.Count + _totalFailedOperationsSkipped + progressUpdate.ProgressPercentage) / _totalOperations;
        return OperationStatus.ProgressUpdate(progressPercentage, string.Empty);
    }

    #endregion

    #region Helpers

    private (double ProgressPercentage, OperationStatus? ProgressError) UpdateInProgressOperations(TimeSpan deltaTime)
    {
        double inProgressAggregatedPercentageChange = 0;
        for (var i = 0; i < _inProgressOperations.Count; i++)
        {
            var state = _inProgressOperations[i].Run(deltaTime);
            if (_inProgressOperations[i].IsFinished)
            {
                var finishedOperation = _inProgressOperations[i];
                _inProgressOperations.RemoveAt(i);

                if (state is not OperationState.Complete)
                {
                    if (!_options.IgnoreFailedOperations)
                    {
                        return (0, finishedOperation.Status);
                    }

                    _totalFailedOperationsSkipped++;
                    continue;
                }

                _completedOperations.Add(finishedOperation);
                i--;

                OnOperationFinished?.Invoke(finishedOperation);
            }
            else
            {
                inProgressAggregatedPercentageChange += _inProgressOperations[i].Status.Progress;
            }
        }

        return (inProgressAggregatedPercentageChange, null);
    }

    #endregion
}
