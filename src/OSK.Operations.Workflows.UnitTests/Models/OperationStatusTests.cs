using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.UnitTests.Models;

public class OperationStatusTests
{
    #region Complete

    [Fact]
    public void Complete_StateShouldBeComplete()
    {
        // Arrange & Act
        var status = OperationStatus.Complete;

        // Assert
        Assert.Equal(OperationState.Complete, status.State);
        Assert.Equal(1.0, status.Progress);
        Assert.Equal("Complete", status.Message);
        Assert.Null(status.Exception);
    }

    #endregion

    #region Started

    [Fact]
    public void Started_StateShouldBeInProgress()
    {
        // Arrange & Act
        var status = OperationStatus.Started;

        // Assert
        Assert.Equal(OperationState.InProgress, status.State);
        Assert.Equal(0f, status.Progress);
        Assert.Equal("Started", status.Message);
    }

    #endregion

    #region NotStarted

    [Fact]
    public void NotStarted_StateShouldBeNotStarted()
    {
        // Arrange & Act
        var status = OperationStatus.NotStarted;

        // Assert
        Assert.Equal(OperationState.NotStarted, status.State);
        Assert.Equal(0f, status.Progress);
        Assert.Equal("Not Started", status.Message);
    }

    #endregion

    #region Failed

    [Fact]
    public void Failed_WithMessage_ShouldSetCorrectProperties()
    {
        // Arrange & Act
        var message = "Something went wrong";
        var status = OperationStatus.Failed(message);

        // Assert
        Assert.Equal(OperationState.Failed, status.State);
        Assert.Equal(0.0, status.Progress);
        Assert.Equal(message, status.Message);
        Assert.Null(status.Exception);
    }

    [Fact]
    public void Failed_WithException_ShouldUseExceptionMessage()
    {
        // Arrange & Act
        var ex = new InvalidOperationException("boom");
        var status = OperationStatus.Failed(ex);

        // Assert
        Assert.Equal(OperationState.Failed, status.State);
        Assert.Equal(0.0, status.Progress);
        Assert.Equal("boom", status.Message);
        Assert.Same(ex, status.Exception);
    }

    [Fact]
    public void Failed_WithExceptionAndCustomMessage_ShouldUseCustomMessage()
    {
        // Arrange & Act
        var ex = new InvalidOperationException("boom");
        var message = "custom failure";
        var status = OperationStatus.Failed(ex, message);

        // Assert
        Assert.Equal(OperationState.Failed, status.State);
        Assert.Equal(0.0, status.Progress);
        Assert.Equal(message, status.Message);
        Assert.Same(ex, status.Exception);
    }

    #endregion

    #region Aborted

    [Fact]
    public void Aborted_WithMessage_ShouldSetCorrectProperties()
    {
        // Arrange & Act
        var message = "Aborted by user";
        var status = OperationStatus.Aborted(message);

        // Assert
        Assert.Equal(OperationState.Aborted, status.State);
        Assert.Equal(0.0, status.Progress);
        Assert.Equal(message, status.Message);
    }

    [Fact]
    public void Aborted_WithoutMessage_ShouldAllowNull()
    {
        // Arrange & Act
        var status = OperationStatus.Aborted();

        // Assert
        Assert.Equal(OperationState.Aborted, status.State);
        Assert.Equal(0.0, status.Progress);
        Assert.Null(status.Message);
    }

    #endregion

    #region ProgressUpdate

    [Fact]
    public void ProgressUpdate_WithProgress_ShouldSetCorrectProperties()
    {
        // Arrange & Act
        var status = OperationStatus.ProgressUpdate(0.75, "Working...");

        // Assert
        Assert.Equal(OperationState.InProgress, status.State);
        Assert.Equal(0.75, status.Progress);
        Assert.Equal("Working...", status.Message);
    }

    [Fact]
    public void ProgressUpdate_WithoutMessage_ShouldAllowNull()
    {
        // Arrange & Act
        var status = OperationStatus.ProgressUpdate(0.3);

        // Assert
        Assert.Equal(OperationState.InProgress, status.State);
        Assert.Equal(0.3, status.Progress);
        Assert.Null(status.Message);
    }

    #endregion
}
