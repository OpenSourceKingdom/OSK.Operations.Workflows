using OSK.Operations.Workflows.Models;
using System;

namespace OSK.Operations.Workflows.Tasks;

/// <summary>
/// An operation that executes a given action.
/// </summary>
/// <param name="action"></param>
public class ActionOperation(Action action) : IterativeOperation, ITaskOperation
{
    #region IterativeOperation Overrides

    public override int TotalWorkItems => 1;

    protected override OperationStatus RunIteration(TimeSpan delta)
    {
        action();
        return OperationStatus.Complete;
    }

    #endregion
}
