namespace OSK.Operations.Workflows.Models;

/// <summary>
/// Represents the data of a workflow run at a current instant
/// </summary>
public class WorkflowRunSnapshot
{
    /// <summary>
    /// The status of the current run
    /// </summary>
    public required OperationStatus Status { get; set; }

    /// <summary>
    /// Any outputs completed and stored up to the point the snapshot was taken
    /// </summary>
    public required WorkflowOutputContext Outputs { get; set; }
}
