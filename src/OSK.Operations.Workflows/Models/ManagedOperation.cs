using OSK.Operations.Workflows.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Operations.Workflows.Models;

/// <summary>
/// An operation that is assigned to an <see cref="ITaskOperationManager"/>
/// </summary>
public class ManagedOperation(ITaskOperationManager manager)
{
    /// <summary>
    /// The task id group for the operation and any similar operations that are managed together
    /// </summary>
    public required string TaskGroupId { get; init; }

    /// <summary>
    /// The task operation that is being managed
    /// </summary>
    public required ITaskOperation Task { get; init; }

    /// <summary>
    /// Waits for the managed operation to complete asynchronously
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
    /// <returns>The task to cancel the operation</returns>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item><b>IMPORTANT:</b> This ASSUMES that some other caller is running the <see cref="ITaskOperationManager.Update(System.TimeSpan)"/> method on the manager.
    /// If this is not run then this task will never complete. This is simply a convenience method for async style calls where it helps, be sure to use a 
    /// manager or similar to run the iterations frame by frame if you're using a game engine or some variation that works for your application</item>
    /// </list>
    /// </remarks>
    public async Task WaitAsync(CancellationToken cancellationToken = default)
    {
        if (Task.Status.State is not OperationState.InProgress && Task.Status.State is not OperationState.NotStarted)
        {
            return;
        }

        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnManagerEvent(ITaskOperation operation)
        {
            if (operation == Task)
            {
                tcs.TrySetResult(true);
            }
        }

        manager.OnOperationFinished += OnManagerEvent;

        using var _ = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

        try
        {
            await tcs.Task;
        }
        finally
        {
            // Always unsubscribe to prevent memory leaks, even if canceled or faulted
            manager.OnOperationFinished -= OnManagerEvent;
        }
    }
}
