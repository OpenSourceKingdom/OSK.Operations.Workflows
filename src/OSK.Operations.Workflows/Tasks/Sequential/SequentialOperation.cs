using OSK.Operations.Workflows.Models;
using System;

namespace OSK.Operations.Workflows.Tasks.Sequential;

/// <summary>
/// An operation that completes of a series sequential operation steps
/// </summary>
/// <param name="initialOperation">The initital operation to run</param>
/// <param name="steps">Subsequent steps in the flow that have access to the prior step result</param>
public class SequentialOperation(ITaskOperation initialOperation, params SequentialStep[] steps): IterativeOperation, ITaskOperation
{
    #region Variables

    private ITaskOperation _currentOperation = initialOperation ?? throw new InvalidOperationException("Serquential steps requires an initial operation to run.");
    private int _currentStepIndex = -1;

    #endregion

    #region IterativeOperation Overrides

    /// <inheritdoc/>
    public override int TotalWorkItems { get; } = 1 + steps.Length;

    /// <inheritdoc/>
    protected override OperationStatus RunIteration(TimeSpan delta)
    {
        if (_currentOperation is null)
        {
            return OperationStatus.Started;
        }

        _currentOperation.Iterate(delta);
        if (_currentOperation.IsFinished)
        {
            _currentStepIndex++;
            if (_currentStepIndex >= steps.Length)
            {
                return _currentOperation.Status;
            }

            _currentOperation = steps[_currentStepIndex].TaskFactory(_currentOperation);
            if (_currentOperation is null)
            {
                return OperationStatus.Complete;
            }
        }

        return _currentOperation.Status;
    }

    #endregion
}
