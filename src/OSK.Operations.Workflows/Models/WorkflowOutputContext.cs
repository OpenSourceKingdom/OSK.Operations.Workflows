using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Operations.Workflows.Models;

/// <summary>
/// A context that contains information about the overall operations and reuslts of a given workflow run
/// </summary>
public class WorkflowOutputContext
{
    #region Variables

    private readonly Dictionary<string, Dictionary<string, ITaskOperation>> _completedTaskLookup = [];

    #endregion

    #region Public

    /// <summary>
    /// Gets a previously completed operation from the context as the given type of <see cref="IIterativeOperation"/>.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The operation is 0-indexed.</item>
    /// <item>Returns null if the key is not present or the cast fails.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TOperation">The type of operation that is expected.</typeparam>
    /// <param name="stepId">The id for the workflow step the outputs are associated with</param>
    /// <param name="resultId">The key for the specific operation result</param>
    /// <returns>The operation converted to the provided type.</returns>
    public TOperation? GetOperationAs<TOperation>(string stepId, string resultId)
        where TOperation : class, IIterativeOperation
    {
        return _completedTaskLookup.TryGetValue(stepId, out var stepOperationResultLookup) && stepOperationResultLookup.TryGetValue(resultId, out var operation) && operation is TOperation typedOperation 
            ? typedOperation 
            : null;
    }

    /// <summary>
    /// Gets the most recent operation that was ran and attempts to convert it to a given operation type
    /// </summary>
    /// <typeparam name="TOperation">The type of operation to convert to</typeparam>
    /// <returns>The operation converted to the provided type.</returns>
    public TOperation? GetLastOperationAs<TOperation>()
        where TOperation: class, IIterativeOperation
    {
        var recentStep = _completedTaskLookup.Values.LastOrDefault();
        var recentOperation = recentStep?.Values.LastOrDefault();

        return recentOperation is TOperation typedOperation
            ? typedOperation
            : null;
    }

    /// <summary>
    /// Tries to get a previously completed operation's result as the provided type
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The operation is 0-indexed.</item>
    /// <item>Returns null if the key is not present or the cast fails.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TResult">The type of result the operation returns</typeparam>
    /// <param name="stepId">The id for the workflow step the outputs are associated with</param>
    /// <param name="resultId">The key for the specific operation result</param>
    /// <param name="result">The result of the operation if it was successfully retrieved</param>
    /// <returns>whether the result was successfully retrieved</returns>
    /// 
    public bool TryGetResultAs<TResult>(string stepId, string resultId, out TResult? result)
    {
        var operation = GetOperationAs<IIterativeOperation>(stepId, resultId);
        if (operation is null || operation is not ITaskOperation<TResult> typedOperation)
        {
            result = default;
            return false;
        }

        result = typedOperation.Result;
        return true;
    }

    /// <summary>
    /// Tries to get the most recent operation and returns the result as the given type
    /// </summary>
    /// <typeparam name="TResult">The type of result the operation returned</typeparam>
    /// <param name="result">The result of the operation if it was successfully retrieved</param>
    /// <returns>whether the result was successfully retrieved</returns>
    public bool TryGetLastResultAs<TResult>(out TResult? result)
    {
        var lastOperation = GetLastOperationAs<IIterativeOperation>();
        if (lastOperation is null || lastOperation is not ITaskOperation<TResult> typedOperation)
        {
            result = default;
            return false;
        }

        result = typedOperation.Result;
        return true;
    }

    #endregion

    #region Helpers

    internal void AddOperation(string workflowStepId, string resultId, ITaskOperation operation)
    {
        if (string.IsNullOrEmpty(workflowStepId))
        {
            throw new ArgumentNullException(new(workflowStepId));
        }
        if (string.IsNullOrEmpty(resultId))
        {
            throw new ArgumentNullException(new(resultId));
        }
        if (operation is null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        if (!_completedTaskLookup.TryGetValue(workflowStepId, out var workflowResultLookup))
        {
            workflowResultLookup = [];
            _completedTaskLookup[workflowStepId] = workflowResultLookup;
        }

        workflowResultLookup[resultId] = operation;
    }

    #endregion
}
