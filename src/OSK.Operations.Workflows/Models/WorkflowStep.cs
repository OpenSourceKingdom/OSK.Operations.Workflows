using OSK.Operations.Workflows.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Operations.Workflows.Models;

/// <summary>
/// Describes a single step in a workflow
/// </summary>
public class WorkflowStep
{
    #region Api

    /// <summary>
    /// 
    /// </summary>
    public string Id { get; set; } = "_default";

    public string? Description { get; set; }

    public WorkflowRunOptions RunSettings { get; set; } = new WorkflowRunOptions();

    public Func<WorkflowOutputContext, IReadOnlyCollection<WorkflowTaskOperationDetails>> TaskOperationFactory { get; }

    #endregion

    #region Constructors

    public WorkflowStep(params ITaskOperation[] operations)
    {
        if (operations is null)
        {
            throw new ArgumentNullException(nameof(operations));
        }
        TaskOperationFactory = _ => [.. operations.Select(op => new WorkflowTaskOperationDetails(op))];
    }

    public WorkflowStep(params WorkflowTaskOperationDetails[] details)
    {
        if (details is null)
        {
            throw new ArgumentNullException(nameof(details));
        }
        TaskOperationFactory = _ => details;
    }

    public WorkflowStep(Func<WorkflowOutputContext, IReadOnlyCollection<ITaskOperation>> operationFactory)
    {
        if (operationFactory is null)
        {
            throw new ArgumentNullException(nameof(operationFactory));
        }
        TaskOperationFactory = context => [.. operationFactory(context).Select(op => new WorkflowTaskOperationDetails(op))];
    }

    public WorkflowStep(Func<WorkflowOutputContext, IReadOnlyCollection<WorkflowTaskOperationDetails>> operationFactory)
    {
        if (operationFactory is null)
        {
            throw new ArgumentNullException(nameof(operationFactory));
        }
        TaskOperationFactory = operationFactory;
    }

    #endregion
}