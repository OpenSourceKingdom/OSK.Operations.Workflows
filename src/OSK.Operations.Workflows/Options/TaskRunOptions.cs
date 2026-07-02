namespace OSK.Operations.Workflows.Options;

public class TaskRunOptions
{
    #region Static

    public static TaskRunOptions Default() => new()
    {
        IgnoreFailure = false
    };

    #endregion

    #region Variables

    /// <summary>
    /// Determines if failed operations within a workpool should exit the entire operation or be ignored. Ignoring will allow accumulation of failed operations and 
    /// continuation of the work pool.
    /// </summary>
    public bool IgnoreFailure { get; set; }

    #endregion
}
