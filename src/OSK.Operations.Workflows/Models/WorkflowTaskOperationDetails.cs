using OSK.Operations.Workflows.Options;

namespace OSK.Operations.Workflows.Models;

public class WorkflowTaskOperationDetails(ITaskOperation operation)
{
    public string? Id { get; set; }

    public TaskRunOptions RunSettings { get; set; } = TaskRunOptions.Default();

    public ITaskOperation Operation { get; } = operation;
}
