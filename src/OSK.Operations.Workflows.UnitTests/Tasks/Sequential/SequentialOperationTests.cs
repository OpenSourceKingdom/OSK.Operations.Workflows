using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Tasks;
using OSK.Operations.Workflows.Tasks.Sequential;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Operations.Workflows.UnitTests.Tasks.Sequential;

public class SequentialOperationTests
{
    #region Constructors

    [Fact]
    public void Constructor_NullInitialOperation_ThrowsInvalidOperationException()
    {
        // Arrange/Act/Asset
        Assert.Throws<InvalidOperationException>(() => new SequentialOperation(null!, []));
    }

    #endregion

    #region TotalWorkItems

    [Fact]
    public void TotalWorksItems_OnlyInitialOperation_NoExtraOperations_ReturnsOne()
    {
        // Arrange
        var sequentialOp = new SequentialOperation(new FinishedOperation(), []);

        // Act/Assert
        Assert.Equal(1, sequentialOp.TotalWorkItems);
    }


    [Fact]
    public void TotalWorksItems_MultipleExtraOperations_ReturnsExpected()
    {
        // Arrange
        var extraOps = new List<SequentialStep>();
        for (var i = 0; i < 10; i++)
        {
            extraOps.Add(new(_ => new FinishedOperation()));
        }

        var sequentialOp = new SequentialOperation(new FinishedOperation(), extraOps);

        // Act/Assert
        Assert.Equal(1 + extraOps.Count, sequentialOp.TotalWorkItems);
    }

    #endregion

    #region RunIteration

    [Fact]
    public void RunIteration_OnlyInitial_CompletesWhenFinished()
    {
        // Arrange
        var operation = new SequentialOperation(new FinishedOperation(), []);

        // Act
        var result = operation.IterateSeconds(1);

        // Assert
        Assert.True(result is OperationState.Complete);
    }

    [Fact]
    public void RunIteration_IncludesExtraOperations_RemainsInProgress()
    {
        // Arrange
        var operation = new SequentialOperation(new FinishedOperation(), [new(_ => new FinishedOperation())]);

        // Act
        var result = operation.IterateSeconds(1);

        // Assert
        Assert.True(result is OperationState.InProgress);
    }

    [Fact]
    public void RunIteration_IncludesExtraOperations_RunsToCompletion_RemainsComplete()
    {
        // Arrange
        var operation = new SequentialOperation(new FinishedOperation(), [new(_ => new FinishedOperation())]);

        // Act
        operation.IterateSeconds(1);
        var result = operation.IterateSeconds(1);

        // Assert
        Assert.True(result is OperationState.Complete);
    }

    #endregion
}
