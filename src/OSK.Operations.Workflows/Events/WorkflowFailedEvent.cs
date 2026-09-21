using OSK.Operations.Workflows.Ports;

namespace OSK.Operations.Workflows.Events;

/// <summary>
/// Represents a workflow that finished due to failure
/// </summary>
/// <param name="runner">The runner that triggered the event</param>
public class WorkflowFailedEvent(IWorkflowRunner runner): WorkflowFinishedEvent(runner)
{
}
