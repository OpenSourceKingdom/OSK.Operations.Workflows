using OSK.Operations.Workflows;
using OSK.Operations.Workflows.Events;
using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Options;
using OSK.Operations.Workflows.Executors;
using OSK.Operations.Workflows.UnitTests._Helpers;
using System.Diagnostics;

namespace OSK.Operations.Workflows.UnitTests.Runners;

public class OperationRunnerTests
{
    #region Variables

    private readonly OperationRunOptions _sequentialSettings = new() { MaxConcurrenOperations = 1 };
    private readonly OperationRunOptions _concurrentSettings = new() { MaxConcurrenOperations = 3 };

    private ITaskOperation CreateCompletedOp() => new TestOperation() { DesiredIterationState = OperationState.Complete };

    private ITaskOperation CreateInProgressOp(TimeSpan delay) 
        => new TestOperation() { DesiredIterationState = OperationState.InProgress, Delay = delay };

    private ITaskOperation CreateFailedOp(string message)
        => new TestOperation() { DesiredIterationState = OperationState.Failed, Message = message };

    #endregion

    #region Constructors

    [Fact]
    public void Constructor_WithOperations_ShouldSetTotalWorkItems()
    {
        // Arrange
        var ops = new[] { CreateCompletedOp(), CreateCompletedOp() };

        // Act
        var runner = new OperationRunner(ops);

        // Assert
        Assert.Equal(2, runner.TotalWorkItems);
    }

    [Fact]
    public void Constructor_WithNullOperations_ShouldSetTotalWorkItemsToZero()
    {
        // Arrange & Act
        var runner = new OperationRunner((ITaskOperation[])null!);

        // Assert
        Assert.Equal(0, runner.TotalWorkItems);
    }

    [Fact]
    public void Constructor_WithSettings_ShouldSetTotalWorkItems()
    {
        // Arrange
        var ops = new[] { CreateCompletedOp(), CreateCompletedOp(), CreateCompletedOp() };

        // Act
        var runner = new OperationRunner(_sequentialSettings, ops);

        // Assert
        Assert.Equal(3, runner.TotalWorkItems);
    }

    [Fact]
    public void Constructor_WithNullSettings_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OperationRunner((OperationRunOptions)null!, CreateCompletedOp()));
    }

    #endregion

    #region RunIteration

    [Fact]
    public void RunIteration_EmptyOperations_ShouldReturnComplete()
    {
        // Arrange
        var runner = new OperationRunner((ITaskOperation[])[]);

        // Act
        var state = runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(OperationState.Complete, state);
    }

    [Fact]
    public void RunIteration_WithCompletedOperations_ShouldReturnComplete()
    {
        // Arrange
        var ops = new[] { CreateCompletedOp(), CreateCompletedOp() };
        var runner = new OperationRunner(_sequentialSettings, ops);

        // Act
        runner.Iterate(TimeSpan.Zero);
        runner.Iterate(TimeSpan.Zero);

        // Assert
        var state = runner.Iterate(TimeSpan.Zero);
        Assert.Equal(OperationState.Complete, state);
    }

    [Fact]
    public void RunIteration_WithInProgressOperations_ShouldReturnInProgress()
    {
        // Arrange
        var ops = new[] { CreateInProgressOp(TimeSpan.FromSeconds(10)) };
        var runner = new OperationRunner(_sequentialSettings, ops);

        // Act
        var state = runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(OperationState.InProgress, state);
    }

    [Fact]
    public void RunIteration_WithFailedOperations_ShouldPropagateFailedStatus()
    {
        // Arrange
        var ops = new[] { CreateFailedOp("test failure") };
        var runner = new OperationRunner(_sequentialSettings, ops);

        // Act
        var state = runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(OperationState.Failed, state);
    }

    [Fact]
    public void RunIteration_ShouldDequeueUpToMaxConcurrentOperations()
    {
        // Arrange
        var ops = new[] 
        { 
            CreateInProgressOp(TimeSpan.FromSeconds(10)),
            CreateInProgressOp(TimeSpan.FromSeconds(10)),
            CreateInProgressOp(TimeSpan.FromSeconds(10))
        };
        var runner = new OperationRunner(_concurrentSettings, ops);

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert - all 3 should have been dequeued and in progress
        Assert.True(runner.Status.State == OperationState.InProgress);
    }

    [Fact]
    public void RunIteration_SequentialExecution_ShouldCompleteAllSequentially()
    {
        // Arrange
        var ops = new[] 
        {
            CreateCompletedOp(),
            CreateCompletedOp(),
            CreateCompletedOp()
        };
        var runner = new OperationRunner(_sequentialSettings, ops);

        // Act
        while (!runner.IsFinished)
        {
            runner.Iterate(TimeSpan.Zero);
        }

        // Assert
        Assert.Equal(OperationState.Complete, runner.Status.State);
    }

    [Fact]
    public void RunIteration_ShouldCalculateProgressCorrectly()
    {
        // Arrange
        var ops = new[]
        {
            CreateCompletedOp(),
            CreateInProgressOp(TimeSpan.FromSeconds(10)),
            CreateInProgressOp(TimeSpan.FromSeconds(10))
        };
        var runner = new OperationRunner(_concurrentSettings, ops);

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert - 
        Assert.True(runner.Status.Progress is .5);
    }

    [Fact]
    public void RunIteration_WithMixedCompletedAndFailed_IgnoresFailures_ShouldCountFailedSeparately()
    {
        // Arrange
        var ops = new[] 
        {
            CreateCompletedOp(),
            CreateFailedOp("fail"),
            CreateInProgressOp(TimeSpan.FromSeconds(10))
        };
        var runner = new OperationRunner(_concurrentSettings, ops.Select(op => new WorkflowTaskOperationDetails(op)
        {
            RunSettings = new TaskRunOptions() { IgnoreFailure = true }
        }).ToArray());

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert - should be in progress (last op is still running)
        Assert.Equal(OperationState.InProgress, runner.Status.State);
    }

    #endregion

    #region OnOperationFinished

    [Fact]
    public void RunIteration_WhenOperationCompletes_ShouldFireOnOperationFinishedEvent()
    {
        // Arrange
        bool eventFired = false;
        var ops = new[] { CreateCompletedOp() };
        var runner = new OperationRunner(_sequentialSettings, ops);

        runner.OnOperationFinished += _ => { eventFired = true; };

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void RunIteration_WhenOperationCompletes_ShouldIncludeTaskId()
    {
        // Arrange
        ITaskOperation? capturedOp = null;
        string? capturedTaskId = null;
        var ops = new[] { CreateCompletedOp() };
        var runner = new OperationRunner(_sequentialSettings, ops);

        runner.OnOperationFinished += evt => 
        { 
            capturedOp = evt.Operation; 
            capturedTaskId = evt.TaskId;
        };

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.NotNull(capturedOp);
    }

    #endregion

    #region Status

    [Fact]
    public void Status_WhenComplete_ShouldHaveCompleteState()
    {
        // Arrange
        var ops = new[] { CreateCompletedOp() };
        var runner = new OperationRunner(_sequentialSettings, ops);

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(OperationState.Complete, runner.Status.State);
    }

    [Fact]
    public void Status_WhenInProgress_ShouldHaveInProgressState()
    {
        // Arrange
        var ops = new[] { CreateInProgressOp(TimeSpan.FromSeconds(10)) };
        var runner = new OperationRunner(_sequentialSettings, ops);

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(OperationState.InProgress, runner.Status.State);
    }

    [Fact]
    public void Status_WhenFailed_ShouldHaveFailedState()
    {
        // Arrange
        var ops = new[] { CreateFailedOp("fail") };
        var runner = new OperationRunner(_sequentialSettings, ops);

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(OperationState.Failed, runner.Status.State);
    }

    #endregion

    #region IsSuccessful / IsFinished

    [Fact]
    public void IsSuccessful_WhenComplete_ShouldReturnTrue()
    {
        // Arrange
        var ops = new[] { CreateCompletedOp() };
        var runner = new OperationRunner(_sequentialSettings, ops);
        runner.Iterate(TimeSpan.Zero);

        // Act & Assert
        Assert.True(runner.IsSuccessful);
    }

    [Fact]
    public void IsFinished_WhenComplete_ShouldReturnTrue()
    {
        // Arrange
        var ops = new[] { CreateCompletedOp() };
        var runner = new OperationRunner(_sequentialSettings, ops);
        runner.Iterate(TimeSpan.Zero);

        // Act & Assert
        Assert.True(runner.IsFinished);
    }

    [Fact]
    public void IsFinished_WhenInProgress_ShouldReturnFalse()
    {
        // Arrange
        var ops = new[] { CreateInProgressOp(TimeSpan.FromSeconds(10)) };
        var runner = new OperationRunner(_sequentialSettings, ops);
        runner.Iterate(TimeSpan.Zero);

        // Act & Assert
        Assert.False(runner.IsFinished);
    }

    #endregion
}
