using OSK.Hexagonal.MetaData;
using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Options;
using System;

namespace OSK.Operations.Workflows.Ports;

/// <summary>
/// A manager that handles the execution and management of <see cref="ITaskOperation"/> instances.
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface ITaskOperationManager
{
    /// <summary>
    /// An event that is raised when an operation has finished executing, either successfully or not.
    /// </summary>
    event Action<ITaskOperation>? OnOperationFinished;

    /// <summary>
    /// Configures the <see cref="ITaskOperationManager"/> with the specified settings.
    /// </summary>
    /// <param name="configurator">The option configurator</param>
    void Configure(Action<TaskOperationManagerSettings> configurator);

    /// <summary>
    /// Adds a new <see cref="ITaskOperation"/> to the manager for execution.
    /// </summary>
    /// <param name="operation">The managed task associated with the operation</param>
    /// <param name="options">The options for the managed task</param>
    /// <returns>The managed operation, or null if the operation could not be added</returns>
    ManagedOperation? AddOperation(ITaskOperation operation, ManagedTaskOptions? options = null);

    /// <summary>
    /// Updates the state of the <see cref="ITaskOperationManager"/> and its managed operations.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update</param>
    void Update(TimeSpan deltaTime);
}
