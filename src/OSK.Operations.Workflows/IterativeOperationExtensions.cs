using OSK.Operations.Workflows.Models;
using System;

namespace OSK.Operations.Workflows;

public static class IterativeOperationExtensions
{
    /// <summary>
    /// Runs a single iteration of the operation, using zero as the time from the previous iteration of the operation
    /// </summary>
    /// <param name="operation">The operation to iterate</param>
    /// <returns>The state of the operation</returns>
    public static OperationState Iterate(this IIterativeOperation operation)
        => operation.Iterate(TimeSpan.Zero);

    /// <summary>
    /// Runs a single iteration of the operation, using the specified double as the time from the previous iteration of the operation
    /// </summary>
    /// <param name="operation">The operation to iterate</param>
    /// <returns>The state of the operation</returns>
    public static OperationState IterateSeconds(this IIterativeOperation operation, double deltaTimeInSeconds)
        => operation.Iterate(TimeSpan.FromSeconds(deltaTimeInSeconds));
}
