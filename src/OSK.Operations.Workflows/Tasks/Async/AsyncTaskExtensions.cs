using System.Threading.Tasks;

namespace OSK.Operations.Workflows.Tasks.Async;

public static class AsyncTaskExtensions
{
    /// <summary>
    /// Converts the task into an equvialent <see cref="AsynchronousOperation"/>
    /// </summary>
    /// <param name="task">The task to convert to an operation</param>
    /// <returns>An <see cref="AsynchronousOperation"/> that references the task</returns>
    public static AsynchronousOperation ToOperation(this Task task)
        => new(task);

    /// <summary>
    /// Converts the value task into an equvialent <see cref="AsynchronousOperation"/>
    /// </summary>
    /// <param name="task">The value task to convert to an operation</param>
    /// <returns>An <see cref="AsynchronousOperation"/> that references the value task</returns>
    public static AsynchronousOperation ToOperation(this ValueTask task)
        => new(task);

    /// <summary>
    /// Converts the task into an equvialent <see cref="AsynchronousOperation{TResult}"/>
    /// </summary>
    /// <param name="task">The task to convert to an operation</param>
    /// <returns>An <see cref="AsynchronousOperation{TResult}"/> that references the task</returns>
    public static AsynchronousOperation<TResult> ToOperation<TResult>(this Task<TResult> task)
        => new(task);

    /// <summary>
    /// Converts the value task into an equvialent <see cref="AsynchronousOperation{TResult}"/>
    /// </summary>
    /// <param name="task">The value task to convert to an operation</param>
    /// <returns>An <see cref="AsynchronousOperation{TResult}"/> that references the value task</returns>
    public static AsynchronousOperation<TResult> ToOperation<TResult>(this ValueTask<TResult> task)
        => new(task);
}
