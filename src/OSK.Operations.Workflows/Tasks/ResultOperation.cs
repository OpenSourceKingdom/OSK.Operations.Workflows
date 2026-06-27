using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Tasks;

/// <summary>
/// A <see cref="FinishedOperation"/> that contains a given result. Useful for when an immediate response needs to be provided.
/// </summary>
/// <typeparam name="TResult">The type of result the operation returns</typeparam>
/// <param name="result">The result to return</param>
public class ResultOperation<TResult>(TResult? result, string message = "") 
    : FinishedOperation(result is null ? OperationState.Failed : OperationState.Complete, message), ITaskOperation<TResult>
{
    #region IIterativeOperation

    public TResult? Result => result;

    #endregion
}
