namespace OSK.Operations.Workflows.Models;

public class TaskOperationDetails(ITaskOperation operation)
{
    public string? Name { get; set; }

    public TaskRunSettings RunSettings { get; set; } = TaskRunSettings.Default();

    public ITaskOperation Operation { get; } = operation;
}
