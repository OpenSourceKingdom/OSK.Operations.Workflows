using OSK.Hexagonal.MetaData;
using OSK.Operations.Workflows.Events;
using OSK.Operations.Workflows.Models;
using System;

namespace OSK.Operations.Workflows.Ports;

/// <summary>
/// A runner that is capable of handling a workflow sequence, executing each step in order and managing the state of the workflow.
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IWorkflowRunner: IIterativeOperation
{
    /// <summary>
    /// Triggered when a workflow step operation has finished, either successfully or not
    /// </summary>
    event Action<WorkflowStepOperationFinishedEvent>? OnOperationFinished;

    /// <summary>
    /// Triggered when an event occurs within the workflow, such as a step finishing or an error occurring.
    /// </summary>
    event Action<WorkflowEvent>? OnWorkflowEvent;

    /// <summary>
    /// The current workflow step number.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>This is not guaranteed to be a zero-indexed number</item>
    /// </list>
    /// </remarks>
    int CurrentWorkflowStep { get; }

    /// <summary>
    /// Gets a snapshot of the current state of the workflow run, including its status and outputs.
    /// </summary>
    /// <returns></returns>
    WorkflowRunSnapshot GetSnapshot();
}
