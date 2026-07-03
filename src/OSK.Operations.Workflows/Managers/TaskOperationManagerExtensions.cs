using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Options;
using OSK.Operations.Workflows.Ports;
using OSK.Operations.Workflows.Tasks.Async;
using System;
using System.Threading.Tasks;

namespace OSK.Operations.Workflows.Managers;

public static class TaskOperationManagerExtensions
{
    // --------------------------------------------------
    // Task (no result)
    // --------------------------------------------------

    /// <summary>
    /// Adds an async operation to the manager
    /// </summary>
    /// <param name="manager">The manager to add the task to</param>
    /// <param name="taskFactory">The factory to create the async operation</param>
    /// <param name="options">The options for the task</param>
    /// <returns>The managed operation, if it could be created</returns>
    public static ManagedOperation? AddTask(this ITaskOperationManager manager, Func<Task> taskFactory, ManagedTaskOptions? options = null)
        => manager.AddOperation(new AsynchronousOperation(taskFactory), options);

    // --------------------------------------------------
    // ValueTask
    // --------------------------------------------------

    /// <summary>
    /// Adds an async operation to the manager
    /// </summary>
    /// <param name="manager">The manager to add the task to</param>
    /// <param name="taskFactory">The factory to create the async operation</param>
    /// <param name="options">The options for the task</param>
    /// <returns>The managed operation, if it could be created</returns>
    public static ManagedOperation? AddValueTask(this ITaskOperationManager manager, Func<ValueTask> taskFactory, ManagedTaskOptions? options = null)
        => manager.AddOperation(new AsynchronousOperation(taskFactory), options);


    // --------------------------------------------------
    // ValueTask<TResult>
    // --------------------------------------------------

    /// <summary>
    /// Adds an async operation to the manager
    /// </summary>
    /// <param name="manager">The manager to add the task to</param>
    /// <param name="taskFactory">The factory to create the async operation</param>
    /// <param name="options">The options for the task</param>
    /// <returns>The managed operation, if it could be created</returns>
    public static ManagedOperation? AddValueTask<TResult>(this ITaskOperationManager manager, Func<ValueTask<TResult>> taskFactory, ManagedTaskOptions? options = null)
        => manager.AddOperation(new AsynchronousOperation<TResult>(taskFactory), options);

    // --------------------------------------------------
    // Process
    // --------------------------------------------------

    /// <summary>
    /// Updates the task operation manager
    /// </summary>
    /// <param name="manager">The manager to update</param>
    /// <param name="deltaSeconds">The time elapsed since the last update, in seconds</param>
    public static void Update(this ITaskOperationManager manager, double deltaSeconds)
        => manager.Update(TimeSpan.FromSeconds(deltaSeconds));
}
