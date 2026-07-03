using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Events;

/// <summary>
/// Reoresents an event that is raised when a task operation has finished executing.
/// </summary>
/// <param name="operation">The operation that finished executing</param>
/// <param name="taskId">The ID of the task the operation is associated with</param>
public class TaskOperationFinishedEvent(ITaskOperation operation, string? taskId = null) : TaskOperationEvent(operation)
{
    /// <summary>
    /// The id for the operation
    /// </summary>
    public string? TaskId { get; } = taskId;
}
