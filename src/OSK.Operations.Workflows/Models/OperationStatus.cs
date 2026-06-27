using System;

namespace OSK.Operations.Workflows.Models;

/// <summary>
/// Provides contextual information for an operation and the state of its iteration.
/// </summary>
/// <param name="state">The state of the associated operation</param>
/// <param name="progress">How close to completion the operation is. This should be a value between 0 adn 1</param>
/// <param name="message">A descriptive message for the current status</param>
/// <param name="exception">An exception the operation encountered</param>
public readonly struct OperationStatus(OperationState state, double progress = 0.0f, string message = "", Exception? exception = null)
{
    #region Static

    public static OperationStatus Complete = new(OperationState.Complete, 1.0f, "Complete");
    public static OperationStatus Started = new(OperationState.InProgress, 0f, "Started");
    public static OperationStatus NotStarted = new(OperationState.NotStarted, 0f, "Not Started");

    public static OperationStatus Failed(string message = "", Exception? exception = null)
        => new(OperationState.Failed, 0.0f, message, exception);

    public static OperationStatus Aborted(string message = "")
        => new(OperationState.Aborted, 0.0f, message);

    public static OperationStatus ProgressUpdate(double progress = 0.0f, string message = "")
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
    public string Message => message;

    #endregion

    #region Helpers

    private static double NormalizeProgress(double progress)
    {
        if (progress < 0)
        {
            return 0;
        }
        if (progress < 1)
        {
            return 1;
        }

        progress /= 100;
        return progress >= 1
            ? 1
            : progress;
    }

    #endregion
}
