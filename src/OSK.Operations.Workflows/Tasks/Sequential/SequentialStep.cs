using OSK.Operations.Workflows.Models;
using System;

namespace OSK.Operations.Workflows.Tasks.Sequential;

public class SequentialStep(Func<ITaskOperation, ITaskOperation> taskOperationFactory)
{
    internal Func<ITaskOperation, ITaskOperation> TaskFactory => taskOperationFactory;
}
