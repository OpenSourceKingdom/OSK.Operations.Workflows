using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.UnitTests._Helpers;

namespace OSK.Operations.Workflows.UnitTests;

public class IterativeOperationTests
{
    #region Variables

    private TestOperation _operation;

    #endregion

    #region Constructors

    public IterativeOperationTests()
    {
        _operation = new TestOperation();
    }

    #endregion

    #region Iterate

    [Fact]
    public void Iterate_FirstCall_ShouldCallInitializeOnce()
    {
        // Arrange/Act
        _operation.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(1, _operation.InitializeCalled);
    }

    [Fact]
    public void Iterate_SecondCall_ShouldNotCallInitializeAgain()
    {
        // Arrange/Act
        _operation.Iterate(TimeSpan.Zero);
        _operation.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(1, _operation.InitializeCalled);
    }

    [Fact]
    public void Iterate_ShouldCallRunIterationEveryTime()
    {
        // Arrange/Act
        _operation.Iterate(TimeSpan.Zero);
        _operation.Iterate(TimeSpan.Zero);
        _operation.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(3, _operation.IteratationsCalled);
    }

    [Fact]
    public void Iterate_ShouldUpdateStatusProperty()
    {
        // Arrange
        var expectedStatus = OperationStatus.Complete;
        _operation.DesiredIterationState = expectedStatus.State;

        // Act
        _operation.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(expectedStatus.State, _operation.Status.State);
    }

    [Fact]
    public void Iterate_WhenFinished_ShouldSkipLogicAndReturnCurrentState()
    {
        // Arrange
        _operation.DesiredIterationState = OperationState.Complete;

        // Act
        var state = _operation.Iterate(TimeSpan.Zero);
        var state2 = _operation.Iterate(TimeSpan.Zero);

        // Assert
        Assert.Equal(OperationState.Complete, state);
        Assert.Equal(state, state2);
        Assert.Equal(1, _operation.IteratationsCalled);
    }

    #endregion

    #region IsSuccessful

    [Fact]
    public void IsSuccessful_WhenStateIsComplete_ShouldReturnTrue()
    {
        // Arrange
        _operation.DesiredIterationState = OperationState.Complete;

        // Act
        _operation.Iterate(TimeSpan.Zero);

        // Assert
        Assert.True(_operation.IsSuccessful);
    }

    [Fact]
    public void IsSuccessful_WhenStateIsNotComplete_ShouldReturnFalse()
    {
        // Arrange/Act/Assert
        Assert.False(_operation.IsSuccessful);
    }

    #endregion

    #region IsFinished

    [Theory]
    [InlineData(OperationState.Failed, true)]
    [InlineData(OperationState.Aborted, true)]
    [InlineData(OperationState.Complete, true)]
    [InlineData(OperationState.NotStarted, false)]
    [InlineData(OperationState.InProgress, false)]
    public void IsFinished_ShouldReturnExpectedValueBasedOnState(OperationState state, bool expected)
    {
        // Arrange
        _operation.DesiredIterationState = state;

        // Act
        _operation.Iterate();

        // Assert
        Assert.Equal(expected, _operation.IsFinished);
    }

    #endregion
}
