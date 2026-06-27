using System;
using System.Threading;
using System.Threading.Tasks;
using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Tasks.Async;

/// <summary>
/// An operation that is driven by an async style task
/// </summary>
public class AsynchronousOperation : WorkflowOperation
{
    #region Variables

    internal static CancellationToken CancelledToken = new(canceled: true);

    private readonly Func<Task>? _taskFactory;
    private Task? _task;

    #endregion

    #region Constructors

    /// <summary>
    /// Create an async operation using a delayed factory
    /// </summary>
    /// <param name="taskFactory">The factory to use to create a given task</param>
    /// <exception cref="ArgumentNullException">Task factory can not be null</exception>
    public AsynchronousOperation(Func<Task> taskFactory)
    {
        if (taskFactory is null)
        {
            throw new ArgumentNullException(nameof(taskFactory));
        }

        _taskFactory = taskFactory;
    }

    /// <summary>
    /// Create an async operation using a task directly
    /// </summary>
    /// <param name="task">A task</param>
    /// <exception cref="ArgumentNullException">Task can not be null</exception>
    public AsynchronousOperation(Task task)
    {
        if (task is null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        _task = task;
    }

    /// <summary>
    /// Create an async operation using a value task directly
    /// </summary>
    /// <param name="task">The value task</param>
    public AsynchronousOperation(ValueTask task)
    {
        _task = task.IsCompleted
            ? task.IsCompletedSuccessfully
                ? Task.CompletedTask
                : task.IsCanceled ? Task.FromCanceled(CancelledToken) : task.AsTask()
            : task.AsTask();
    }

    #endregion

    #region Public

    /// <inheritdoc/>
    public Exception? Exception { get; private set; }

    #endregion

    #region IterativeOperation Overrides

    /// <summary>
    /// Triggered when a task has completed iterating and is finzalized
    /// </summary>
    /// <param name="task">The task that just completed</param>
    protected virtual void OnTaskComplete(Task task)
    {
    }

    protected override OperationStatus RunIteration(TimeSpan deltaTime)
    {
        if (_task is null)
        {
            _task = _taskFactory?.Invoke();
            if (_task is null)
            {
                return OperationStatus.Complete;
            }
        }
        if (_task.IsCompletedSuccessfully)
        {
            OnTaskComplete(_task);
            return OperationStatus.Complete;
        }
        if (_task.IsCanceled)
        {
            return OperationStatus.Aborted("Async operation was cancelled.");
        }
        if (_task.IsFaulted)
        {
            Exception = _task.Exception is AggregateException aggregateException
                ? aggregateException.InnerException
                : _task.Exception;
            return OperationStatus.Failed(_task.Exception?.Message ?? "Unknown exception.", _task.Exception);
        }

        return OperationStatus.ProgressUpdate(0, "Operation Running...");
    }

    #endregion
}
