using Moq;
using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Ports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace OSK.Operations.Workflows.UnitTests.Models;

public class ManagedOperationTests
{
    #region WaitAsync

    [Theory]
    [InlineData(OperationState.Complete)]
    [InlineData(OperationState.Aborted)]
    [InlineData(OperationState.Failed)]
    public async Task WaitAsync_ManagedOperationNotInValidState_EndsEarlyWithoutWaiting(OperationState state)
    {
        // Arrange
        var mockOperation = new Mock<ITaskOperation>();
        mockOperation.SetupGet(o => o.Status)
            .Returns(new OperationStatus(state));

        var mockManager = new Mock<ITaskOperationManager>();
        var managedOp = new ManagedOperation(mockManager.Object)
        {
            Task = mockOperation.Object,
            TaskGroupId = string.Empty
        };

        // Act
        await managedOp.WaitAsync(TestContext.Current.CancellationToken);

        // Assert
        mockManager.VerifyAdd(m => m.OnOperationFinished += It.IsAny<Action<ITaskOperation>>(), Times.Never);
    }

    [Fact]
    public async Task WaitAsync_ManagedOperationInValidState_CancellationTokenRequestsAbort_EndsEarlyAndRemovesActionFromManager()
    {
        // Arrange
        var mockOperation = new Mock<ITaskOperation>();
        mockOperation.SetupGet(o => o.Status)
            .Returns(new OperationStatus(OperationState.InProgress));

        var mockManager = new Mock<ITaskOperationManager>();
        var managedOp = new ManagedOperation(mockManager.Object)
        {
            Task = mockOperation.Object,
            TaskGroupId = string.Empty
        };

        using var cts = new CancellationTokenSource();

        // Act
        var waitTask = managedOp.WaitAsync(cts.Token);
        cts.Cancel();

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => waitTask);

        // Ensure the event handler was safely unsubscribed via the finally block
        mockManager.VerifyRemove(m => m.OnOperationFinished -= It.IsAny<Action<ITaskOperation>>(), Times.Once);
    }

    [Fact]
    public async Task WaitAsync_ManagedOperationInValidState_WaitsForManagerToRun_EndsWhenIterated_ReturnsSuccessfully()
    {
        // Arrange
        var mockOperation = new Mock<ITaskOperation>();
        mockOperation.SetupGet(o => o.Status)
            .Returns(new OperationStatus(OperationState.InProgress));

        Action<ITaskOperation>? capturedHandler = null;

        var mockManager = new Mock<ITaskOperationManager>();
        mockManager.SetupAdd(m => m.OnOperationFinished += It.IsAny<Action<ITaskOperation>>())
           .Callback<Action<ITaskOperation>>(h => capturedHandler = h);
        mockManager.SetupRemove(m => m.OnOperationFinished -= It.IsAny<Action<ITaskOperation>>())
                   .Callback<Action<ITaskOperation>>(h => capturedHandler = null);

        var managedOp = new ManagedOperation(mockManager.Object)
        {
            Task = mockOperation.Object,
            TaskGroupId = string.Empty
        };

        // Act
        var waitTask = managedOp.WaitAsync(TestContext.Current.CancellationToken);

        // Simulate the manager finishing the operation
        Assert.NotNull(capturedHandler);
        capturedHandler.Invoke(mockOperation.Object);

        // Assert
        await waitTask; // Should complete successfully without throwing
        mockManager.VerifyRemove(m => m.OnOperationFinished -= It.IsAny<Action<ITaskOperation>>(), Times.Once);
    }

    #endregion
}
