using OSK.Operations.Workflows.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace OSK.Operations.Workflows.Tasks.Sequential;

/// <summary>
/// An operation that completes of a series sequential operation steps
/// </summary>
public class SequentialOperation: IterativeOperation, ITaskOperation
{
    #region Variables

    private ITaskOperation _currentOperation;
    private SequentialStep[] _steps;

    private int _currentStepIndex = -1;

    #endregion

    #region Constructors

    /// <summary>
    /// An operation that completes of a series sequential operation steps
    /// </summary>
    /// <param name="initialOperation">The initital operation to run</param>
    /// <param name="steps">Subsequent steps in the flow that have access to the prior step result</param>
    public SequentialOperation(ITaskOperation initialOperation, params SequentialStep[] steps)
        : this(initialOperation, (IEnumerable<SequentialStep>)steps)
    {
    }

    /// <summary>
    /// An operation that completes of a series sequential operation steps
    /// </summary>
    /// <param name="initialOperation">The initital operation to run</param>
    /// <param name="steps">Subsequent steps in the flow that have access to the prior step result</param>
    public SequentialOperation(ITaskOperation initialOperation, IEnumerable<SequentialStep> steps)
    {
        _currentOperation = initialOperation ?? throw new InvalidOperationException("Serquential steps requires an initial operation to run.");
        _steps = steps?.ToArray() ?? [];

        TotalWorkItems = 1 + _steps.Length;
    }

    #endregion

    #region IterativeOperation Overrides

    /// <inheritdoc/>
    public override int TotalWorkItems { get; }

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
            if (_currentStepIndex >= _steps.Length)
            {
                return _currentOperation.Status;
            }

            _currentOperation = _steps[_currentStepIndex].TaskFactory(_currentOperation);
            if (_currentOperation is null)
            {
                return OperationStatus.Complete;
            }

            return new OperationStatus(OperationState.InProgress, (1 + _currentStepIndex - 1) / TotalWorkItems);
        }

        return _currentOperation.Status;
    }

    #endregion
}
