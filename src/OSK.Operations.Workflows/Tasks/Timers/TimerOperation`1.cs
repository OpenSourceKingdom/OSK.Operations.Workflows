using OSK.Operations.Workflows.Models;
using System;

namespace OSK.Operations.Workflows.Tasks.Timers;

/// <summary>
/// A <see cref="TimerOperation"/> that returns a result when completed successfully
/// </summary>
/// <typeparam name="TResult">The type of result</typeparam>
/// <param name="timeDelay">The time delay before the operation completes</param>
/// <param name="result">Thje result to return once the timer completes</param>
public class TimerOperation<TResult>(TimeSpan timeDelay, TResult result) : TimerOperation(timeDelay), ITaskOperation<TResult>
{
    #region TimerOperation Overrides

    /// <inheritdoc/>>
    public TResult? Result { get; private set; }

    protected override void OnTimerComplete()
    {
        Result = result;
    }

    #endregion
}
