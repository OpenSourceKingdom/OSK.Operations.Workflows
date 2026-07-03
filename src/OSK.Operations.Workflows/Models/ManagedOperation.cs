using OSK.Operations.Workflows.Ports;

namespace OSK.Operations.Workflows.Models;

/// <summary>
/// An operation that is assigned to an <see cref="ITaskOperationManager"/>
/// </summary>
public class ManagedOperation
{
    /// <summary>
    /// The task id group for the operation and any similar operations that are managed together
    /// </summary>
    public required string TaskGroupId { get; set; }

    /// <summary>
    /// The task operation that is being managed
    /// </summary>
    public required ITaskOperation Task { get; set; }
}
