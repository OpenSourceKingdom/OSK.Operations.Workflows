using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Tasks.Async;

namespace OSK.Operations.Workflows.UnitTests.Tasks.Asynchronous;

public class AsynchronousOperationTests
{
    #region Iterate

    [Fact]
    public void Iterate_CreatedWithCompletedValueTask_ReturnsComplete()
    {
        // Arrange
        var asyncOperation = new AsynchronousOperation(ValueTask.CompletedTask);
        var typedAsyncOperation = new AsynchronousOperation<int>(new ValueTask<int>(42));

        // Act
        var operationProgress = asyncOperation.Iterate();
        var typedOperationProgress = typedAsyncOperation.Iterate();

        // Assert
        Assert.Equal(OperationState.Complete, asyncOperation.Status.State);
        Assert.Null(asyncOperation.Exception);
        Assert.Equal(1.0, asyncOperation.Status.Progress);

        Assert.Equal(OperationState.Complete, typedAsyncOperation.Status.State);
        Assert.Null(typedAsyncOperation.Exception);
        Assert.Equal(1.0, typedAsyncOperation.Status.Progress);
        Assert.Equal(42, typedAsyncOperation.Result);
    }

    [Fact]
    public void Iterate_CreatedWithDelayedValueTask_TaskDoesNotComplete_ReturnsInProgress()
    {
        // Arrange
        var asyncOperation = new AsynchronousOperation(new ValueTask(Task.Delay(100, TestContext.Current.CancellationToken)));
        var typedAsyncOperation = new AsynchronousOperation<int>(new ValueTask<int>(Task.Delay(100, TestContext.Current.CancellationToken).ContinueWith(_ => 42)));

        // Act
        var operationProgress = asyncOperation.Iterate();
        var typedOperationProgress = typedAsyncOperation.Iterate();

        // Assert
        Assert.Equal(OperationState.InProgress, asyncOperation.Status.State);
        Assert.Null(asyncOperation.Exception);
        Assert.Equal(0, asyncOperation.Status.Progress);

        Assert.Equal(OperationState.InProgress, typedAsyncOperation.Status.State);
        Assert.Null(typedAsyncOperation.Exception);
        Assert.Equal(0, typedAsyncOperation.Status.Progress);
    }

    [Fact]
    public async Task Iterate_CreatedWithDelayedValueTask_TaskCompletes_ReturnsCompletedProgress()
    {
        // Arrange
        var asyncOperation = new AsynchronousOperation(new ValueTask(Task.Delay(100, TestContext.Current.CancellationToken)));
        var typedAsyncOperation = new AsynchronousOperation<int>(new ValueTask<int>(Task.Delay(100, TestContext.Current.CancellationToken).ContinueWith(_ => 42)));

        // Act
        await Task.Delay(150, TestContext.Current.CancellationToken); // Wait for the tasks to complete

        var operationProgress = asyncOperation.Iterate();
        var typedOperationProgress = typedAsyncOperation.Iterate();

        // Assert
        Assert.Equal(OperationState.Complete, asyncOperation.Status.State);
        Assert.Null(asyncOperation.Exception);
        Assert.Equal(1, asyncOperation.Status.Progress);

        Assert.Equal(OperationState.Complete, typedAsyncOperation.Status.State);
        Assert.Null(typedAsyncOperation.Exception);
        Assert.Equal(1, typedAsyncOperation.Status.Progress);
        Assert.Equal(42, typedAsyncOperation.Result);
    }

    [Fact]
    public void Iterate_CreatedWithValueTask_CanceledValueTasks_ReturnsAbortedProgress()
    {
        // Arrange
        var asyncOperation = new AsynchronousOperation(ValueTask.FromCanceled(AsynchronousOperation.CancelledToken));
        var typedAsyncOperation = new AsynchronousOperation<int>(ValueTask.FromCanceled<int>(AsynchronousOperation.CancelledToken));

        // Act
        var operationProgress = asyncOperation.Iterate();
        var typedOperationProgress = typedAsyncOperation.Iterate();

        // Assert
        Assert.Equal(OperationState.Aborted, asyncOperation.Status.State);
        Assert.Null(asyncOperation.Exception);
        Assert.Equal(0, asyncOperation.Status.Progress);

        Assert.Equal(OperationState.Aborted, typedAsyncOperation.Status.State);
        Assert.Null(typedAsyncOperation.Exception);
        Assert.Equal(0, typedAsyncOperation.Status.Progress);
    }

    [Fact]
    public void Iterate_CreatedWithExceptionValueTask_ReturnsFailedProgress()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var asyncOperation = new AsynchronousOperation(ValueTask.FromException(exception));
        var typedAsyncOperation = new AsynchronousOperation<int>(ValueTask.FromException<int>(exception));

        // Act
        var operationProgress = asyncOperation.Iterate();
        var typedOperationProgress = typedAsyncOperation.Iterate();

        // Assert
        Assert.Equal(OperationState.Failed, asyncOperation.Status.State);
        Assert.Equal(exception, asyncOperation.Exception);
        Assert.Equal(0, asyncOperation.Status.Progress);

        Assert.Equal(OperationState.Failed, typedAsyncOperation.Status.State);
        Assert.Equal(exception, typedAsyncOperation.Exception);
        Assert.Equal(0, typedAsyncOperation.Status.Progress);
    }

    [Fact]
    public void Iterate_CreatedWithDelayedTask_TaskDoesNotComplete_ReturnsInProgress()
    {
        // Arrange
        var asyncOperation = new AsynchronousOperation(Task.Delay(100, TestContext.Current.CancellationToken));
        var typedAsyncOperation = new AsynchronousOperation<int>(Task.Delay(100, TestContext.Current.CancellationToken).ContinueWith(_ => 42));

        // Act
        var operationProgress = asyncOperation.Iterate();
        var typedOperationProgress = typedAsyncOperation.Iterate();

        // Assert
        Assert.Equal(OperationState.InProgress, asyncOperation.Status.State);
        Assert.Null(asyncOperation.Exception);
        Assert.Equal(0, asyncOperation.Status.Progress);

        Assert.Equal(OperationState.InProgress, typedAsyncOperation.Status.State);
        Assert.Null(typedAsyncOperation.Exception);
        Assert.Equal(0, typedAsyncOperation.Status.Progress);
    }

    [Fact]
    public async Task Iterate_CreatedWithDelayedTask_TaskCompletes_ReturnsCompletedProgress()
    {
        // Arrange
        var asyncOperation = new AsynchronousOperation(Task.Delay(100, TestContext.Current.CancellationToken));
        var typedAsyncOperation = new AsynchronousOperation<int>(Task.Delay(100, TestContext.Current.CancellationToken).ContinueWith(_ => 42));

        // Act
        await Task.Delay(150, TestContext.Current.CancellationToken); // Wait for the tasks to complete

        var operationProgress = asyncOperation.Iterate();
        var typedOperationProgress = typedAsyncOperation.Iterate();

        // Assert
        Assert.Equal(OperationState.Complete, asyncOperation.Status.State);
        Assert.Null(asyncOperation.Exception);
        Assert.Equal(1, asyncOperation.Status.Progress);

        Assert.Equal(OperationState.Complete, typedAsyncOperation.Status.State);
        Assert.Null(typedAsyncOperation.Exception);
        Assert.Equal(1, typedAsyncOperation.Status.Progress);
        Assert.Equal(42, typedAsyncOperation.Result);
    }

    [Fact]
    public void Iterate_CreatedWithTask_CanceledTasks_ReturnsAbortedProgress()
    {
        // Arrange
        var asyncOperation = new AsynchronousOperation(Task.FromCanceled(AsynchronousOperation.CancelledToken));
        var typedAsyncOperation = new AsynchronousOperation<int>(Task.FromCanceled<int>(AsynchronousOperation.CancelledToken));

        // Act
        var operationProgress = asyncOperation.Iterate();
        var typedOperationProgress = typedAsyncOperation.Iterate();

        // Assert
        Assert.Equal(OperationState.Aborted, asyncOperation.Status.State);
        Assert.Null(asyncOperation.Exception);
        Assert.Equal(0, asyncOperation.Status.Progress);

        Assert.Equal(OperationState.Aborted, typedAsyncOperation.Status.State);
        Assert.Null(typedAsyncOperation.Exception);
        Assert.Equal(0, typedAsyncOperation.Status.Progress);
    }

    [Fact]
    public void Iterate_CreatedWithExceptionTask_ReturnsFailedProgress()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var asyncOperation = new AsynchronousOperation(Task.FromException(exception));
        var typedAsyncOperation = new AsynchronousOperation<int>(Task.FromException<int>(exception));

        // Act
        var operationProgress = asyncOperation.Iterate();
        var typedOperationProgress = typedAsyncOperation.Iterate();

        // Assert
        Assert.Equal(OperationState.Failed, asyncOperation.Status.State);
        Assert.Equal(exception, asyncOperation.Exception);
        Assert.Equal(0, asyncOperation.Status.Progress);

        Assert.Equal(OperationState.Failed, typedAsyncOperation.Status.State);
        Assert.Equal(exception, typedAsyncOperation.Exception);
        Assert.Equal(0, typedAsyncOperation.Status.Progress);
    }

    #endregion
}
