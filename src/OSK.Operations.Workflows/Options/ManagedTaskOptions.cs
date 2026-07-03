using OSK.Operations.Workflows.Ports;

namespace OSK.Operations.Workflows.Options;

/// <summary>
/// A set of options for adding a managed task to an <see cref="ITaskOperationManager"/>
/// </summary>
public class ManagedTaskOptions
{
    /// <summary>
    /// An optional id that can be used to specify a unique identifier for the managed task. If not provided, a new id will be generate using the type of task.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The task id is used to group operations, if multiple operations are associated with the same task id.</item>
    /// </list>
    /// </remarks>
    public string? TaskId { get; set; }
}
