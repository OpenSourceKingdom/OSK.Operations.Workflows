namespace OSK.Operations.Workflows.Models;

/// <summary>
/// Represents the running state of the operation
/// </summary>
public enum OperationState
{
    /// <summary>
    /// The operation has not been iterated and may require initialization
    /// </summary>
    NotStarted,

    /// <summary>
    /// The operation has begun iteration and has initialized
    /// </summary>
    InProgress,
    
    /// <summary>
    /// The operation has finished iterating successfuly
    /// </summary>
    Complete,
    
    /// <summary>
    /// The operation was aborted
    /// </summary>
    Aborted,

    /// <summary>
    /// The operation ended because of an error that resulted in a failure
    /// </summary>
    Failed
}
