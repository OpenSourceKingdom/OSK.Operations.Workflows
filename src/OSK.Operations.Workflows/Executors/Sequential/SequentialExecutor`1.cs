using System;
using OSK.Operations.Workflows.Models;


namespace OSK.Operations.Workflows.Executors.Sequential;

/// <summary>
/// A <see cref="SequentialExecutor"/> that will return a final result of type <see cref="{TResult}"/>
/// </summary>
/// <typeparam name="TResult">The type of result</typeparam>
/// <param name="operationFactories">The factories to create the sequential operations</param>
/// <param name="finalFactory">The factory that is used to return the final result</param>
public class SequentialExecutor<TResult>(Func<SequentialExecutionContext, ITaskOperation>[] operationFactories,
    Func<SequentialExecutionContext, ITaskOperation<TResult>> finalFactory) : SequentialExecutor([.. operationFactories, finalFactory]), ITaskOperation<TResult>
{
    #region IIterativeOperation<TResult>

    /// <inheritdoc/>
    public TResult? Result { get; private set; }

    #endregion

    #region SequentialOperation Overrides

    protected override OperationStatus RunIteration(TimeSpan deltaTime)
    {
        var status = base.RunIteration(deltaTime);
        if (status.State is OperationState.Complete && status.Progress >= 1.0f)
        {
            var finalOperation = Context.GetOperationAs<ITaskOperation<TResult>>(operationFactories.Length);
            Result = finalOperation is null
                ? default
                : finalOperation.Result;
        }

        return status;
    }

    #endregion
}
