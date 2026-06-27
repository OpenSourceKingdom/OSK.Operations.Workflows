using OSK.Operations.Workflows.Models;
using System;

namespace OSK.Operations.Workflows;

public static class WorkflowOperationExtensions
{
    /// <summary>
    /// Runs a single iteration of the operation, using zero as the time from the previous iteration of the operation
    /// </summary>
    /// <param name="operation">The operation to iterate</param>
    /// <returns>The state of the operation</returns>
    public static OperationState Iterate(this IWorkflowOperation operation)
        => operation.Run(TimeSpan.Zero);
}
