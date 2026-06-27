using OSK.Operations.Workflows.Models;
using System;

namespace OSK.Operations.Workflows.Executors.Sequential;

/// <summary>
/// Executes a sequence of iterative operations, advancing to the next operation when the current one completes. This provides extra contextual data for reference to earlier results
/// in the sequence of operations through a <see cref="SequentialExecutionContext"/>
/// </summary>
/// <param name="operationFactories">The factories to create the sequential operations</param>
public class SequentialExecutor(params Func<SequentialExecutionContext, ITaskOperation>[] operationFactories) : WorkflowOperation, IOperationExecutor
{
    #region Variables

    protected SequentialExecutionContext Context = new();
    protected ITaskOperation? CurrentOperation;
    private int _currentOperationIndex = 0;

    #endregion

    #region IOperationExecutor

    public event Action<ITaskOperation>? OnOperationFinished;
    
    #endregion

    #region IterativeOperation Overrides

    protected override OperationStatus RunIteration(TimeSpan deltaTime)
    {
        if (_currentOperationIndex >= operationFactories.Length)
        {
            return OperationStatus.Complete;
        }
        if (CurrentOperation is null)
        {
            CurrentOperation = operationFactories[_currentOperationIndex].Invoke(Context);
        }

        var state = CurrentOperation.Run(deltaTime);
        var message = CurrentOperation.Status.Message;
        var progressPercentage = (CurrentOperation.Status.Progress + _currentOperationIndex) / operationFactories.Length;

        switch (state)
        {
            case OperationState.Complete:
                Context.AddOperation(_currentOperationIndex, CurrentOperation);
                _currentOperationIndex++;

                OnOperationFinished?.Invoke(CurrentOperation);

                CurrentOperation = null;
                if (_currentOperationIndex >= operationFactories.Length)
                {
                    return OperationStatus.Complete;
                }

                break;
            case OperationState.Aborted:
                return OperationStatus.Aborted($"Operation {_currentOperationIndex} failed: {CurrentOperation.Status.Message}");
            case OperationState.Failed:
                return OperationStatus.Failed($"Operation {_currentOperationIndex} failed: {CurrentOperation.Status.Message}", CurrentOperation.Status.Exception);
        }

        return OperationStatus.ProgressUpdate(progressPercentage, message);
    }

    #endregion
}
