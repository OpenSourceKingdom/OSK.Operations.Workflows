using OSK.Hexagonal.MetaData;
using OSK.Operations.Workflows.Events;
using OSK.Operations.Workflows.Models;
using System;

namespace OSK.Operations.Workflows.Ports;

[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IWorkflowRunner: IIterativeOperation
{
    event Action<WorkflowStepOperationFinishedEvent>? OnOperationFinished;
    event Action<WorkflowEvent>? OnWorkflowEvent;

    WorkflowRunSnapshot GetSnapshot();
}
