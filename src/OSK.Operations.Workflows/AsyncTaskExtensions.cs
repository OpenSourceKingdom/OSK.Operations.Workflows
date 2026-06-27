using System.Threading.Tasks;
using OSK.Operations.Workflows.Tasks.Async;

namespace OSK.Operations.Workflows;

public static class AsyncTaskExtensions
{
    /// <summary>
    /// Converts the task into an equvialent <see cref="AsynchronousOperation"/>
    /// </summary>
    /// <param name="task">The task to convert to an operation</param>
    /// <returns>An <see cref="AsynchronousOperation"/> that references the task</returns>
    public static AsynchronousOperation ToAsyncOperation(this Task task)
        => new(task);

    /// <summary>
    /// Converts the value task into an equvialent <see cref="AsynchronousOperation"/>
    /// </summary>
    /// <param name="task">The value task to convert to an operation</param>
    /// <returns>An <see cref="AsynchronousOperation"/> that references the value task</returns>
    public static AsynchronousOperation ToAsyncOperation(this ValueTask task)
        => new(task);

    /// <summary>
    /// Converts the task into an equvialent <see cref="AsynchronousOperation{TResult}"/>
    /// </summary>
    /// <param name="task">The task to convert to an operation</param>
    /// <returns>An <see cref="AsynchronousOperation{TResult}"/> that references the task</returns>
    public static AsynchronousOperation<TResult> ToAsyncOperation<TResult>(this Task<TResult> task)
        => new(task);

    /// <summary>
    /// Converts the value task into an equvialent <see cref="AsynchronousOperation{TResult}"/>
    /// </summary>
    /// <param name="task">The value task to convert to an operation</param>
    /// <returns>An <see cref="AsynchronousOperation{TResult}"/> that references the value task</returns>
    public static AsynchronousOperation<TResult> ToAsyncOperation<TResult>(this ValueTask<TResult> task)
        => new(task);
}
