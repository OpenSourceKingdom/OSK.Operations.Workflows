namespace OSK.Operations.Workflows.Executors.Concurrent;

public class ConcurrentExecutionOptions
{
    #region Static

    public static ConcurrentExecutionOptions Default() => new()
    {
        IgnoreFailedOperations = false,
        MaxConcurrenOperations = 1
    };

    #endregion

    #region Variables

    /// <summary>
    /// Determines if failed operations within a workpool should exit the entire operation or be ignored. Ignoring will allow accumulation of failed operations and 
    /// continuation of the work pool.
    /// </summary>
    public bool IgnoreFailedOperations { get; set; }

    private int _concurrentOperations = 1;
    /// <summary>
    /// Specifies the total number of operations that can be run concurrently. If this is set to 1, operations will be run sequentially.
    /// A value less than or equal to 0 will be treated as 1.
    /// </summary>
    public int MaxConcurrenOperations
    {
        get => _concurrentOperations;
        set
        {
            _concurrentOperations = value < 1 ? 1 : value;
        }
    }

    #endregion
}
