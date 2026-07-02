using OSK.Operations.Workflows.Models;
using System;
using System.Threading.Tasks;

namespace OSK.Operations.Workflows.Tasks.Async;

/// <summary>
/// An <see cref="AsynchronousOperation"/> that returns a <see cref="{TResult}"/> when it has finished iterating successfully
/// </summary>
/// <typeparam name="TResult"></typeparam>
public class AsynchronousOperation<TResult> : AsynchronousOperation, ITaskOperation<TResult>
{
    #region Constructors

    /// <summary>
    /// Create an async operation using a delayed factory
    /// </summary>
    /// <param name="taskFactory">The factory to use to create a given task</param>
    /// <exception cref="ArgumentNullException">Task factory can not be null</exception>
    public AsynchronousOperation(Func<Task<TResult>> taskFactory)
        : base(taskFactory)
    {
    }

    /// <summary>
    /// Create an async operation using a delayed factory; i.e. the async task will not start until the operation is iterated at least once
    /// </summary>
    /// <param name="taskFactory">The factory to use to create a given task</param>
    /// <exception cref="ArgumentNullException">Value task factory can not be null</exception>
    public AsynchronousOperation(Func<ValueTask<TResult>> taskFactory)
        : base(GetTaskFactory(taskFactory))
    {
    }

    /// <summary>
    /// Create an async operation using a task directly
    /// </summary>
    /// <param name="task">A task</param>
    /// <exception cref="ArgumentNullException">Task can not be null</exception>
    public AsynchronousOperation(Task<TResult> task)
        : base(task)
    {
    }

    /// <summary>
    /// Create an async operation using a value task directly
    /// </summary>
    /// <param name="task">The value task</param>
    public AsynchronousOperation(ValueTask<TResult> task)
        : base(GetTask(task))
    {
    }

    #endregion

    #region AsynchronousOperation Overrides

    protected override void OnTaskComplete(Task task)
    {
        if (task is Task<TResult> resultTask)
        {
            Result = resultTask.Result;
        }
    }

    #endregion

    #region IterativeOperation
    
    /// <inheritdoc/>
    public TResult? Result { get; private set; }

    #endregion

    #region Helpers

    private static Func<Task<TResult>> GetTaskFactory(Func<ValueTask<TResult>> taskFactory)
        => () =>
        {
            var valueTask = taskFactory();
            return valueTask.IsCompleted
                ? valueTask.IsCompletedSuccessfully
                    ? Task.FromResult(valueTask.Result)
                    : valueTask.IsCanceled? Task.FromCanceled<TResult>(CancelledToken) : valueTask.AsTask()
                : valueTask.AsTask();
        };

private static Task<TResult> GetTask(ValueTask<TResult> task)
        => task.IsCompleted
            ? task.IsCompletedSuccessfully
                ? Task.FromResult(task.Result)
                : task.IsCanceled ? Task.FromCanceled<TResult>(CancelledToken) : task.AsTask()
            : task.AsTask();

    #endregion
}
