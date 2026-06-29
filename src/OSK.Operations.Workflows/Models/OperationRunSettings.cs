namespace OSK.Operations.Workflows.Models;

public class OperationRunSettings
{
    #region Static

    public static OperationRunSettings Default() => new()
    {
        MaxConcurrenOperations = 1
    };

    #endregion

    #region Variables

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
