using OSK.Operations.Workflows.Executors;
using OSK.Operations.Workflows.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Operations.Workflows.Runners;

public class WorkflowRunner(IEnumerable<WorkflowStep> steps) : WorkflowOperation
{
    #region Variables

    private string? _currentMessage;
    private OperationRunner? _operationRunner;
    private WorkflowOutputContext _outputContext = new();

    private readonly Queue<WorkflowStep> _stepQueue = new(steps);

    #endregion

    #region WorkflowOperation Overrides

    public override int TotalWorkItems { get; } = steps.Count();

    protected override void Initialize()
    {
        PrepareNextStep();
    }

    protected override OperationStatus RunIteration(TimeSpan delta)
    {
        if (_operationRunner is null)
        {
            return OperationStatus.Complete;
        }

        var state = _operationRunner.Iterate(delta);
        return state switch
        {
            OperationState.InProgress => GetProgressUpdate(TotalWorkItems - _stepQueue.Count - 1),
            OperationState.Complete => ProcessRunnerCompletion(),
            OperationState.Aborted or OperationState.Failed => _operationRunner.Status,
            _ => throw new InvalidOperationException($"An unexpected state occurred when iterating the Workflow runner: {state}"),
        };
    }

    #endregion

    #region Helpers

    private OperationStatus ProcessRunnerCompletion()
        => PrepareNextStep()
            ? GetProgressUpdate(TotalWorkItems - _stepQueue.Count - 1)
            : OperationStatus.Complete;

    private bool PrepareNextStep()
    {
        WorkflowStep? step = null;
        while (_stepQueue.Count > 0 && step is null)
        {
            step = _stepQueue.Dequeue();
        }
        if (step is null)
        {
            _operationRunner = null;
            return false;
        }

        _currentMessage = step.Description;
        _operationRunner = new OperationRunner(step.RunSettings.OperationSettings, [.. step.TaskOperationFactory(_outputContext)]);
        _operationRunner.OnOperationFinished += details => ProcesssFinishedOperation(step.Id, details);
        return true;
    }

    private OperationStatus GetProgressUpdate(int stepIndex)
    {
        var stepProgress = _operationRunner?.Status.Progress ?? 1f;
        return OperationStatus.ProgressUpdate((stepIndex + stepProgress) / TotalWorkItems, _currentMessage);
    }
    
    private void ProcesssFinishedOperation(string stepId, TaskOperationDetails details)
    {
        if (details.Operation.IsSuccessful && !string.IsNullOrWhiteSpace(details.Name))
        {
            _outputContext.AddOperation(stepId, details.Name, details.Operation);
        }
    }

    #endregion
}
