using System;

namespace OSK.Operations.Workflows.Models;

/// <summary>
/// Represents an operation that executes and manages a collection of <see cref="ITaskOperation"/>
/// </summary>
public interface IOperationRunner: IIterativeOperation
{
    /// <summary>
    /// Triggered when a operation within an executor has finished, either successfully or not
    /// </summary>
    event Action<TaskOperationDetails>? OnOperationFinished;
}
