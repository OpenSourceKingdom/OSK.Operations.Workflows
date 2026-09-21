using OSK.Operations.Workflows.Ports;

namespace OSK.Operations.Workflows.Events;

/// <summary>
/// Represents an even that is triggered when a workflow has been finished. This can be a successful or failed workflow
/// </summary>
/// <param name="workflowRunner">The runner that triggered the event</param>
public abstract class WorkflowFinishedEvent(IWorkflowRunner workflowRunner): WorkflowEvent
{
    /// <summary>
    /// The runner that just finished the workflow
    /// </summary>
    public IWorkflowRunner Runner => workflowRunner;
}
