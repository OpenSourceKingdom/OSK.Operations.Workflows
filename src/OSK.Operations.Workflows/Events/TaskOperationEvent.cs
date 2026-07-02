using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Events;

public abstract class TaskOperationEvent(ITaskOperation operation)
{
    public ITaskOperation Operation { get; } = operation;
}
