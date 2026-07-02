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

    public static ManagedOperation? AddTask(this ITaskOperationManager manager, Func<Task> taskFactory, ManagedTaskOptions? options = null)
        => manager.AddOperation(new AsynchronousOperation(taskFactory), options);

    // --------------------------------------------------
    // ValueTask
    // --------------------------------------------------

    public static ManagedOperation? AddValueTask(this ITaskOperationManager manager, Func<ValueTask> taskFactory, ManagedTaskOptions? options = null)
        => manager.AddOperation(new AsynchronousOperation(taskFactory), options);


    // --------------------------------------------------
    // ValueTask<TResult>
    // --------------------------------------------------

    public static ManagedOperation? AddValueTask<TResult>(this ITaskOperationManager manager, Func<ValueTask<TResult>> taskFactory, ManagedTaskOptions? options = null)
        => manager.AddOperation(new AsynchronousOperation<TResult>(taskFactory), options);

    // --------------------------------------------------
    // Process
    // --------------------------------------------------

    public static void Update(this ITaskOperationManager manager, double deltaTime)
        => manager.Update(TimeSpan.FromSeconds(deltaTime));
}
