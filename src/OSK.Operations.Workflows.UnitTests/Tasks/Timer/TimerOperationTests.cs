using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Tasks.Timers;

namespace OSK.Operations.Workflows.UnitTests.Tasks.Timer;

public class TimerOperationTests
{
    #region Iterate

    [Theory]
    [InlineData(10, 5, .5, false)]
    [InlineData(10, 10, 1.0, true)]
    [InlineData(10, 15, 1.0, true)]
    [InlineData(10, 0, 0.0, false)]
    public void Iterate_TimePassing_ShouldReturnCorrectProgress(double timeDelaySeconds, double timePassedSeconds, double expectedPercentage, bool shouldBeComplete)
    {
        // Arrange
        var timeDelay = TimeSpan.FromSeconds(timeDelaySeconds);
        var timerOperation = new TimerOperation(timeDelay);

        // Act
        var state = timerOperation.IterateSeconds(timePassedSeconds);

        // Assert

        Assert.Equal(expectedPercentage, timerOperation.Status.Progress);

        if (shouldBeComplete)
        {
            Assert.Equal(OperationState.Complete, state);
        }
        else
        {
            Assert.Equal(OperationState.InProgress, state);
        }
    }

    #endregion

    #region TimeRemaining

    [Fact]
    public void TimeRemaining_Initial_ShouldReturnFullTimeDelay()
    {
        // Arrange
        var timeDelay = TimeSpan.FromSeconds(10);
        var timerOperation = new TimerOperation(timeDelay);

        // Act/Assert
        Assert.Equal(timeDelay, timerOperation.TimeRemaining);
    }

    [Fact]
    public void TimeRemaining_TimePasses_ShouldReturnCorrectTimeRemaining()
    {
        // Arrange
        var timeDelay = TimeSpan.FromSeconds(5);
        var timerOperation = new TimerOperation(timeDelay);

        // Act
        timerOperation.IterateSeconds(2);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(3), timerOperation.TimeRemaining);
    }

    #endregion

    #region Result TimerOperation<TValue>

    [Fact]
    public void Result_TimerOperationTValue_TimerHasNotExpired_ReturnsNull()
    {
        // Arrange
        var timeDelay = TimeSpan.FromSeconds(5);
        var expectedValue = "TestValue";
        var timerOperation = new TimerOperation<string>(timeDelay, expectedValue);

        // Act/Assert
        Assert.Null(timerOperation.Result);
    }

    [Fact]
    public void Result_TimerOperationTValue_ReturnsExpectedValue()
    {
        // Arrange
        var timeDelay = TimeSpan.FromSeconds(5);
        var expectedValue = "TestValue";
        var timerOperation = new TimerOperation<string>(timeDelay, expectedValue);

        // Act
        timerOperation.IterateSeconds(5);
        
        // Assert
        Assert.Equal(expectedValue, timerOperation.Result);
    }

    #endregion
}
