using OSK.Operations.Workflows;
using OSK.Operations.Workflows.Events;
using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Options;
using OSK.Operations.Workflows.Runners;
using OSK.Operations.Workflows.UnitTests._Helpers;

namespace OSK.Operations.Workflows.UnitTests.Runners;

public class WorkflowRunnerTests
{
    #region Variables

    private WorkflowStep CreateCompletedStep(string id = "step1")
        => new([new TestOperation() { DesiredIterationState = OperationState.Complete }]) { Id = id };

    private WorkflowStep CreateInProgressStep()
        => new([new TestOperation() { DesiredIterationState = OperationState.InProgress, Delay = TimeSpan.FromSeconds(10) }]);

    private WorkflowStep CreateFailedStep()
        => new([new TestOperation() { DesiredIterationState = OperationState.Failed, Message = "step error" }]);

    #endregion

    #region GetSnapshot

    [Fact]
    public void GetSnapshot_BeforeFirstIteration_ShouldReturnNotStartedStatus()
    {
        // Arrange
        var steps = new[] { CreateCompletedStep() };
        var runner = new WorkflowRunner(steps);

        // Act
        var snapshot = runner.GetSnapshot();

        // Assert
        Assert.Equal(OperationState.NotStarted, snapshot.Status.State);
    }

    [Fact]
    public void GetSnapshot_AfterAllStepsComplete_ShouldReturnCompleteStatus()
    {
        // Arrange
        var steps = new[] { CreateCompletedStep() };
        var runner = new WorkflowRunner(steps);
        runner.Iterate(TimeSpan.Zero);

        // Act
        var snapshot = runner.GetSnapshot();

        // Assert
        Assert.Equal(OperationState.Complete, snapshot.Status.State);
    }

    [Fact]
    public void GetSnapshot_ShouldIncludeOutputContext()
    {
        // Arrange
        var steps = new[] { CreateCompletedStep() };
        var runner = new WorkflowRunner(steps);

        // Act
        var snapshot = runner.GetSnapshot();

        // Assert
        Assert.NotNull(snapshot.Outputs);
    }

    #endregion

    #region Initialize

    [Fact]
    public void Initialize_ShouldLoadFirstStep()
    {
        // Arrange
        var steps = new[] { CreateCompletedStep() };
        var runner = new WorkflowRunner(steps);

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert - snapshot should show in-progress (initialized, not complete)
        var snapshot = runner.GetSnapshot();
        Assert.True(snapshot.Status.State != OperationState.NotStarted);
    }

    [Fact]
    public void Initialize_ShouldSetInitializedFlag()
    {
        // Arrange
        var steps = new[] { CreateCompletedStep() };
        var runner = new WorkflowRunner(steps);

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert - after initialization, snapshot should not be NotStarted
        var snapshot = runner.GetSnapshot();
        Assert.NotEqual(OperationState.NotStarted, snapshot.Status.State);
    }

    #endregion

    #region RunIteration

    [Fact]
    public void RunIteration_EmptyWorkflow_ShouldReturnComplete()
    {
        // Arrange
        var runner = new WorkflowRunner(Array.Empty<WorkflowStep>());

        // Act
        var state = runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(OperationState.Complete, state);
    }

    [Fact]
    public void RunIteration_DuringStep_ShouldReturnInProgress()
    {
        // Arrange
        var steps = new[] { CreateInProgressStep() };
        var runner = new WorkflowRunner(steps);
        runner.Iterate(TimeSpan.Zero); // initialize

        // Act
        var state = runner.Iterate(TimeSpan.FromMilliseconds(10));

        // Assert
        Assert.Equal(OperationState.InProgress, state);
    }

    [Fact]
    public void RunIteration_WhenStepCompletes_ShouldQueueNextStep()
    {
        // Arrange
        var steps = new[] { CreateCompletedStep("step1"), CreateCompletedStep("step2") };
        var runner = new WorkflowRunner(steps);

        // Act - iterate through first step
        runner.Iterate(TimeSpan.Zero);

        // Act - iterate through second step
        runner.Iterate(TimeSpan.Zero);

        // Assert - both steps should have been processed
        var snapshot = runner.GetSnapshot();
        Assert.Equal(OperationState.Complete, snapshot.Status.State);
    }

    [Fact]
    public void RunIteration_WhenAllStepsDone_ShouldReturnComplete()
    {
        // Arrange
        var steps = new[] { CreateCompletedStep("step1"), CreateCompletedStep("step2") };
        var runner = new WorkflowRunner(steps);

        // Act
        runner.Iterate(TimeSpan.Zero);
        runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.True(runner.IsFinished);
    }

    [Fact]
    public void RunIteration_WhenStepFails_ShouldPropagateFailedStatus()
    {
        // Arrange
        var steps = new[] { CreateFailedStep() };
        var runner = new WorkflowRunner(steps);

        // Act
        var state = runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(OperationState.Failed, state);
    }

    [Fact]
    public void RunIteration_ShouldCalculateProgressCorrectly()
    {
        // Arrange
        var steps = new[] { CreateCompletedStep("step1"), CreateInProgressStep(), CreateCompletedStep("step3") };
        var runner = new WorkflowRunner(steps);

        // Act
        runner.Iterate(TimeSpan.Zero); // step 1 complete
        var state = runner.Iterate(TimeSpan.FromMilliseconds(50)); // step 2 in progress

        // Assert - progress should be approximately (1 + partial) / 3
        var snapshot = runner.GetSnapshot();
        Assert.True(snapshot.Status.Progress > 0.3);
        Assert.True(snapshot.Status.Progress < 0.7);
    }

    [Fact]
    public void RunIteration_ShouldFireOnWorkflowEventWhenStepCompletes()
    {
        // Arrange
        bool eventFired = false;
        var steps = new[] { CreateCompletedStep("step1") };
        var runner = new WorkflowRunner(steps);

        runner.OnWorkflowEvent += _ => { eventFired = true; };

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void RunIteration_ShouldFireOnOperationFinishedWhenStepCompletes()
    {
        // Arrange
        bool eventFired = false;
        var steps = new[] { CreateCompletedStep("step1") };
        var runner = new WorkflowRunner(steps);

        runner.OnOperationFinished += _ => { eventFired = true; };

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void RunIteration_TotalWorkItems_ShouldMatchStepCount()
    {
        // Arrange
        var steps = new[] { CreateCompletedStep(), CreateCompletedStep(), CreateCompletedStep() };
        var runner = new WorkflowRunner(steps);

        // Act & Assert
        Assert.Equal(3, runner.TotalWorkItems);
    }

    #endregion

    #region OutputContext

    [Fact]
    public void RunIteration_SuccessfulStep_ShouldStoreOutputInContext()
    {
        // Arrange
        var steps = new[] { CreateCompletedStep("step1") };
        var runner = new WorkflowRunner(steps);

        // Act
        runner.Iterate(TimeSpan.Zero);

        // Assert
        var snapshot = runner.GetSnapshot();
        Assert.NotNull(snapshot.Outputs);
    }

    #endregion
}
