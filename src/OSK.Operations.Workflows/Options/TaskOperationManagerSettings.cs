using System.Collections.Generic;

namespace OSK.Operations.Workflows.Options;

public class TaskOperationManagerSettings
{
    #region Variables

    public ManagedOperationGroupOptions Defaults { get; set; } = new();

    public Dictionary<string, ManagedOperationGroupOptions> GroupOptions { get; set; } = [];

    #endregion
}
