using OSK.Hexagonal.MetaData;
using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Options;
using System;

namespace OSK.Operations.Workflows.Ports;

[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface ITaskOperationManager
{
    event Action<ITaskOperation>? OnOperationFinished;

    void Configure(Action<TaskOperationManagerSettings> options);

    ManagedOperation? AddOperation(ITaskOperation operation, ManagedTaskOptions? options = null);

    void Update(TimeSpan deltaTime);
}
