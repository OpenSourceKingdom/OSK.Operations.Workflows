namespace OSK.Operations.Workflows.Models;

public class ManagedOperation
{
    public required string Id { get; set; }

    public required ITaskOperation Task { get; set; }
}
