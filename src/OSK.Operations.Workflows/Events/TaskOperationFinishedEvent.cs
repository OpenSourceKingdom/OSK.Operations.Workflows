using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Events;

public class TaskOperationFinishedEvent(ITaskOperation operation, string? taskId = null) : TaskOperationEvent(operation)
{
    public string? TaskId { get; } = taskId;
}
