namespace OSK.Operations.Workflows.Models;

public class TaskRunSettings
{
    #region Static

    public static TaskRunSettings Default() => new()
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
