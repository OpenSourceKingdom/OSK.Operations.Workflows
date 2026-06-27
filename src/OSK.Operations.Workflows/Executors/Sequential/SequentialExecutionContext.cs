using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Executors.Sequential;

/// <summary>
/// A context that contains information about the overall operations and reuslts of a given iteration for a sequential operation
/// </summary>
public class SequentialExecutionContext
{
    #region Variables

    private readonly Dictionary<int, IWorkflowOperation> _operationLookup = [];

    #endregion

    #region Public

    /// <summary>
    /// Gets a previously completed operation from the context as the given type of <see cref="IWorkflowOperation"/>.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The operation is 0-indexed.</item>
    /// <item>Returns null if the key is not present or the cast fails.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TOperation">The type of operation that is expected.</typeparam>
    /// <param name="index"></param>bb
    /// <returns>The operation converted to the provided type.</returns>
    public TOperation? GetOperationAs<TOperation>(int index)
        where TOperation : class, IWorkflowOperation
    {
        return _operationLookup.TryGetValue(index, out var operation) && operation is TOperation typedOperation 
            ? typedOperation 
            : null;
    }

    /// <summary>
    /// Gets the most recent operation that was ran and attempts to convert it to a given operation type
    /// </summary>
    /// <typeparam name="TOperation">The type of operation to convert to</typeparam>
    /// <returns>The operation converted to the provided type.</returns>
    public TOperation? GetLastOperationAs<TOperation>()
        where TOperation: class, IWorkflowOperation
    {
        var lastOperation = _operationLookup.Values.LastOrDefault();
        return lastOperation is TOperation typedOperation
            ? typedOperation
            : null;
    }

    /// <summary>
    /// Gets a previously completed operation's result as the provided type
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The operation is 0-indexed.</item>
    /// <item>Returns null if the key is not present or the cast fails.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TOperation">The type of operation to convert to</typeparam>
    /// <typeparam name="TResult">The type of result the operation returns</typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    /// 
    public TResult? GetResultAs<TOperation, TResult>(int key)
        where TOperation: class, ITaskOperation<TResult>
    {
        var operation = GetOperationAs<TOperation>(key);
        return operation is null
            ? default
            : operation.Result;
    }

    /// <summary>
    /// Gets the most recent operation and returns the result as the given type
    /// </summary>
    /// <typeparam name="TOperation">The type of operation to convert to</typeparam>
    /// <typeparam name="TResult">The type of result the operation returned</typeparam>
    /// <returns>The result of the operation</returns>
    public TResult? GetLastResult<TOperation, TResult>()
        where TOperation: class, ITaskOperation<TResult>
    {
        var lastOperation = GetLastOperationAs<TOperation>();
        return lastOperation is null
            ? default
            : lastOperation.Result;
    }

    #endregion

    #region Helpers

    internal void AddOperation(int key, IWorkflowOperation operation)
    {
        if (operation is null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        _operationLookup[key] = operation;
    }

    #endregion
}
