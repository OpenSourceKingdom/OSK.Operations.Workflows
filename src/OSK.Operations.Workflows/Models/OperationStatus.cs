using System;

namespace OSK.Operations.Workflows.Models;

/// <summary>
/// Provides contextual information for an operation and the state of its iteration.
/// </summary>
/// <param name="state">The state of the associated operation</param>
/// <param name="progress">How close to completion the operation is. This should be a value between 0 adn 1</param>
/// <param name="message">A descriptive message for the current status</param>
/// <param name="exception">An exception the operation encountered</param>
public readonly struct OperationStatus(OperationState state, double progress = 0.0f, string? message = null, Exception? exception = null)
{
    #region Static

    /// <summary>
    /// A status that represents a completed task.
    /// </summary>
    public static OperationStatus Complete = new(OperationState.Complete, 1.0f, "Complete");

    /// <summary>
    /// A status that represents a task that just started
    /// </summary>
    public static OperationStatus Started = new(OperationState.InProgress, 0f, "Started");

    /// <summary>
    /// A status that represents a task that has not yet started
    /// </summary>
    public static OperationStatus NotStarted = new(OperationState.NotStarted, 0f, "Not Started");

    /// <summary>
    /// Creates a status that represents a failed task with an optional message
    /// </summary>
    /// <param name="message">The status message</param>
    /// <returns>The failed status</returns>
    public static OperationStatus Failed(string? message = null)
        => new(OperationState.Failed, 0.0f, message, null);

    /// <summary>
    /// Creates a status that represents a failed task with an exception and an optional message
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The exception message is used for the status by default</item>
    /// </list>
    /// </remarks>
    /// <param name="exception">The exception the operation encountered</param>
    /// <param name="message">The status message</param>
    /// <returns>The failed status</returns>
    public static OperationStatus Failed(Exception exception, string? message = null)
        => new(OperationState.Failed, 0.0f, message ?? exception.Message, exception);

    /// <summary>
    /// Creates a status that represents an aborted task with an optional message
    /// </summary>
    /// <param name="message">The status message</param>
    /// <returns>The aborted status</returns>
    public static OperationStatus Aborted(string? message = null)
        => new(OperationState.Aborted, 0.0f, message);

    /// <summary>
    /// Creates a status that represents an operation that is in progress with a given progress value and an optional message
    /// </summary>
    /// <param name="progress">The progress value</param>
    /// <param name="message">The status message</param>
    /// <returns>The in-progress status</returns>
    public static OperationStatus ProgressUpdate(double progress, string? message = null)
        => new(OperationState.InProgress, progress, message);

    #endregion

    #region Variables

    /// <summary>
    /// The current state of the associated oepration
    /// </summary>
    public OperationState State => state;

    /// <summary>
    /// The current progress for the operation. i.e. how close to being completed the operation is
    /// </summary>
    public double Progress { get; } = NormalizeProgress(progress);

    /// <summary>
    /// The exception the operation encountered, if any
    /// </summary>
    public Exception? Exception => exception;

    /// <summary>
    /// A descriptive message for the current status
    /// </summary>
    public string? Message => message;

    #endregion

    #region Helpers

    private static double NormalizeProgress(double progress)
    {
        if (progress < 0)
        {
            return 0;
        }
        if (progress <= 1)
        {
            return progress;
        }

        progress /= 100;
        return progress >= 1
            ? 1
            : progress;
    }

    #endregion
}
