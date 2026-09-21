using OSK.Operations.Workflows.Models;
using System;

namespace OSK.Operations.Workflows.Tasks.Sequential;

/// <summary>
/// Represents a single step in a sequential operation
/// </summary>
/// <param name="taskOperationFactory">The factory to create an operation</param>
public class SequentialStep(Func<ITaskOperation, ITaskOperation> taskOperationFactory)
{
    internal Func<ITaskOperation, ITaskOperation> TaskFactory => taskOperationFactory;
}
