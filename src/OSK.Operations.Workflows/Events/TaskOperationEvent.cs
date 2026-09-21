using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Events;

/// <summary>
/// An event for a task operation
/// </summary>
/// <param name="operation"></param>
public abstract class TaskOperationEvent(ITaskOperation operation)
{
    /// <summary>
    /// The operation that caused the event
    /// </summary>
    public ITaskOperation Operation { get; } = operation;
}
