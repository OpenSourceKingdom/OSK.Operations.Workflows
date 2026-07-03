using OSK.Operations.Workflows.Events;
using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Operations.Workflows.Runners;

/// <inheritdoc/>
public class WorkflowRunner(IEnumerable<WorkflowStep> steps) : IterativeOperation, IWorkflowRunner
{
    #region Variables

    private WorkflowStep? _currentStep;
    private OperationRunner? _operationRunner;
    private WorkflowOutputContext _outputContext = new();

    private readonly Queue<WorkflowStep> _stepQueue = new(steps);
    private bool _initialized;

    #endregion

    #region IWorkflowRunner

    public event Action<WorkflowStepOperationFinishedEvent>? OnOperationFinished;
    public event Action<WorkflowEvent>? OnWorkflowEvent;

    /// <summary>
    /// Gets a snapshot of the workflow run current state
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The snapshot of data returned may not be completed if the runner has not finished, please be sure to iterate as needed before assuming the data in the snapshot is complete</item>
    /// </list>
    /// </remarks>
    /// <returns>The snapshot at this time.</returns>
    public WorkflowRunSnapshot GetSnapshot()
        => new()
        {
            Status = _operationRunner is null
                ? _initialized ? OperationStatus.Complete : OperationStatus.NotStarted
                : _operationRunner.Status,
            Outputs = _outputContext
        };

    #endregion

    #region WorkflowOperation Overrides

    public override int TotalWorkItems { get; } = steps.Count();

    protected override void Initialize()
    {
        PrepareNextStep();
        _initialized = true;
    }

    protected override OperationStatus RunIteration(TimeSpan delta)
    {
        if (_operationRunner is null || _currentStep is null)
        {
            return OperationStatus.Complete;
        }

        var state = _operationRunner.Iterate(delta);
        return state switch
        {
            OperationState.InProgress => GetProgressUpdate(TotalWorkItems - _stepQueue.Count - 1),
            OperationState.Complete => ProcessStepCompletion(_currentStep),
            OperationState.Aborted or OperationState.Failed => _operationRunner.Status,
            _ => throw new InvalidOperationException($"An unexpected state occurred when iterating the Workflow runner: {state}"),
        };
    }

    #endregion

    #region Helpers

    private OperationStatus ProcessStepCompletion(WorkflowStep step)
    {
        OnWorkflowEvent?.Invoke(new WorkflowStepFinishedEvent(step));

        return PrepareNextStep()
            ? GetProgressUpdate(TotalWorkItems - _stepQueue.Count - 1)
            : OperationStatus.Complete;
    }

    private bool PrepareNextStep()
    {
        _currentStep = null;
        while (_stepQueue.Count > 0 && _currentStep is null)
        {
            _currentStep = _stepQueue.Dequeue();
        }
        if (_currentStep is null)
        {
            _operationRunner = null;
            return false;
        }

        _operationRunner = new OperationRunner(_currentStep.RunSettings.OperationSettings, [.. _currentStep.TaskOperationFactory(_outputContext)]);
        _operationRunner.OnOperationFinished += finishedEvent => ProcesssFinishedOperation(_currentStep, finishedEvent);
        return true;
    }

    private OperationStatus GetProgressUpdate(int stepIndex)
    {
        var stepProgress = _operationRunner?.Status.Progress ?? 1f;
        return OperationStatus.ProgressUpdate((stepIndex + stepProgress) / TotalWorkItems, _currentStep?.Description);
    }
    
    private void ProcesssFinishedOperation(WorkflowStep step, TaskOperationFinishedEvent finishedEvent)
    {
        if (finishedEvent.Operation.IsSuccessful && !string.IsNullOrWhiteSpace(finishedEvent.TaskId))
        {
            _outputContext.AddOperation(step.Id, finishedEvent.TaskId, finishedEvent.Operation);
        }

        OnOperationFinished?.Invoke(new WorkflowStepOperationFinishedEvent(step, finishedEvent.Operation));
    }

    #endregion
}
