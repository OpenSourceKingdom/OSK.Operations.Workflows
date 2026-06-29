using System;
using System.Collections.Generic;

namespace OSK.Operations.Workflows.Models;

public class WorkflowStep
{
    #region Api

    public string Id { get; set; } = "_default";

    public string? Description { get; set; }

    public WorkflowRunSettings RunSettings { get; set; } = new WorkflowRunSettings();

    public required Func<WorkflowOutputContext, IReadOnlyCollection<TaskOperationDetails>> TaskOperationFactory { get; set; }

    #endregion
}