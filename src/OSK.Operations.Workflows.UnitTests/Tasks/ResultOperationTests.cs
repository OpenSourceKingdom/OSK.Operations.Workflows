using OSK.Operations.Workflows.Tasks;

namespace OSK.Operations.Workflows.UnitTests.Tasks;

public class ResultOperationTests
{    
    #region Result FinishedOperationTests<TValue>

    [Fact]
    public void Result_ResultTValue_ReturnsExpectedValue()
    {
        // Arrange
        var noOpOperation = new ResultOperation<int>(1);

        // Act/Assert
        Assert.Equal(1, noOpOperation.Result);
    }
    
    #endregion
}
