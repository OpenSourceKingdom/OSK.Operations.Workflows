using OSK.Hexagonal.MetaData;
using OSK.Operations.Workflows.Events;
using OSK.Operations.Workflows.Models;
using System;

namespace OSK.Operations.Workflows.Ports;

/// <summary>
/// Represents an operation that executes and manages a collection of <see cref="ITaskOperation"/>
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IOperationRunner: IIterativeOperation
{
    /// <summary>
    /// Triggered when a operation within an executor has finished, either successfully or not
    /// </summary>
    event Action<TaskOperationFinishedEvent>? OnOperationFinished;
}
