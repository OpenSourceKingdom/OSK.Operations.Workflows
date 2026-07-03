using OSK.Operations.Workflows.Internal;

namespace OSK.Operations.Workflows.Options;

/// <summary>
/// Options to provide for configuring a <see cref="ManagedOperationGroup"/> instance.
/// </summary>
public class ManagedOperationGroupOptions
{
    /// <summary>
    /// The maximum number of operations that will be iterated concurrently
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>This will not prevent operations that are async and NOT created using a delayed factory from executing on their own</item>
    /// </list>
    /// </remarks>
    public int? MaxConcurrentOperations { get; set; }

    /// <summary>
    /// The maximum number of operations that can be queued for execution
    /// </summary>
    public int? MaxQueueSize { get; set; }
}
