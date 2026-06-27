using System;

namespace OSK.Operations.Workflows.Models;

/// <summary>
/// Represents an operation that executes and manages a collection of <see cref="IWorkflowOperation"/>
/// </summary>
public interface IOperationExecutor: IWorkflowOperation
{
    /// <summary>
    /// Triggered when a operation within an executor has finished, either successfully or not
    /// </summary>
    event Action<ITaskOperation>? OnOperationFinished;
}
