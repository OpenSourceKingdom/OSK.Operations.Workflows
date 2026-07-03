using System;
using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Tasks.Timers;

/// <summary>
/// An operation that is based on a given timer
/// </summary>
/// <param name="timeDelay">The time delay before the operation completes</param>
public class TimerOperation(TimeSpan timeDelay) : IterativeOperation, ITaskOperation
{
    #region Api

    /// <summary>
    /// How much time is left for the given operation to complete
    /// </summary>
    public TimeSpan TimeRemaining { get; private set; } = TimeSpan.FromMilliseconds(timeDelay.TotalMilliseconds);

    #endregion

    #region IterativeOperation Overrides

    public override int TotalWorkItems { get; } = 1;

    protected override OperationStatus RunIteration(TimeSpan deltaTime)
    {
        TimeRemaining = TimeRemaining.Subtract(deltaTime);
        if (TimeRemaining <= TimeSpan.Zero)
        {
            TimeRemaining = TimeSpan.Zero;
        }

        var percentage = TimeRemaining == TimeSpan.Zero
            ? 1
            : (timeDelay.TotalMilliseconds - TimeRemaining.TotalMilliseconds) / timeDelay.TotalMilliseconds;
        var status = new OperationStatus(percentage is 1 ? OperationState.Complete : OperationState.InProgress, percentage, percentage is 1 ? "Finished" : "Running");

        if (status.State == OperationState.Complete)
        {
            OnTimerComplete();
        }

        return status;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Triggered when the timer operation has completed and is finalizing
    /// </summary>
    protected virtual void OnTimerComplete()
    {
    }

    #endregion
}
