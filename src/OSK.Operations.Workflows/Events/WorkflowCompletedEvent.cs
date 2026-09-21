using OSK.Operations.Workflows.Ports;

namespace OSK.Operations.Workflows.Events;

/// <summary>
/// Represents a workflow finished event that successfully completed
/// </summary>
/// <param name="runner">The runner that triggered the event</param>
public class WorkflowCompletedEvent(IWorkflowRunner runner): WorkflowFinishedEvent(runner)
{
}
