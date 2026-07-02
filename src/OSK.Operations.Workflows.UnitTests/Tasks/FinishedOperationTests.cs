using OSK.Operations.Workflows;
using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Tasks;

namespace OSK.Operations.Workflows.UnitTests.Tasks;

public class FinishedOperationTests
{
    #region Iterate

    [Fact]
    public void Iterate_ShouldReturnCompleteImmediately()
    {
        // Arrange
        var finishedOperation = new FinishedOperation();

        // Act
        var state = finishedOperation.Iterate();

        // Assert
        Assert.Equal(1.0, finishedOperation.Status.Progress);
        Assert.Equal(OperationState.Complete, state);
    }

    #endregion
}
