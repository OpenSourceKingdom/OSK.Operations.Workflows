using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.UnitTests._Helpers;

namespace OSK.Operations.Workflows.UnitTests.Models;

public class WorkflowOutputContextTests
{
    #region GetOperationAs

    [Fact]
    public void GetOperationAs_WhenKeyExists_ShouldReturnTypedOperation()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();
        context.AddOperation("step1", "op1", testOp);

        // Act
        var result = context.GetOperationAs<TestOperation<int>>("step1", "op1");

        // Assert
        Assert.Same(testOp, result);
    }

    [Fact]
    public void GetOperationAs_WhenStepIdMissing_ShouldReturnNull()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();
        context.AddOperation("step1", "op1", testOp);

        // Act
        var result = context.GetOperationAs<TestOperation<int>>("step2", "op1");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetOperationAs_WhenKeyIdMissing_ShouldReturnNull()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();
        context.AddOperation("step1", "op1", testOp);

        // Act
        var result = context.GetOperationAs<TestOperation<int>>("step1", "op2");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetOperationAs_WhenCastFails_ShouldReturnNull()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();
        context.AddOperation("step1", "op1", testOp);

        // Act
        var result = context.GetOperationAs<TestOperation<string>>("step1", "op1");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetLastOperationAs

    [Fact]
    public void GetLastOperationAs_WhenSingleEntry_ShouldReturnThatOperation()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();
        context.AddOperation("step1", "op1", testOp);

        // Act
        var result = context.GetLastOperationAs<TestOperation<int>>();

        // Assert
        Assert.Same(testOp, result);
    }

    [Fact]
    public void GetLastOperationAs_WhenMultipleEntries_ShouldReturnMostRecent()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var op1 = new TestOperation<int>();
        var op2 = new TestOperation<string>();
        context.AddOperation("step1", "op1", op1);
        context.AddOperation("step1", "op2", op2);

        // Act
        var result = context.GetLastOperationAs<TestOperation<string>>();

        // Assert
        Assert.Same(op2, result);
    }

    [Fact]
    public void GetLastOperationAs_WhenEmpty_ShouldReturnNull()
    {
        // Arrange
        var context = new WorkflowOutputContext();

        // Act
        var result = context.GetLastOperationAs<TestOperation<int>>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetLastOperationAs_WhenCastFails_ShouldReturnNull()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();
        context.AddOperation("step1", "op1", testOp);

        // Act
        var result = context.GetLastOperationAs<TestOperation<string>>();

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetResultAs

    [Fact]
    public void GetResultAs_WhenOperationExists_ShouldReturnResult()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();
        testOp.Result = 42;
        context.AddOperation("step1", "op1", testOp);

        // Act
        var result = context.TryGetResultAs<int>("step1", "op1", out var actualResult);

        // Assert
        Assert.True(result);
        Assert.Equal(42, actualResult);
    }

    [Fact]
    public void TryGetResultAs_WhenOperationNotFound_ShouldReturnDefault()
    {
        // Arrange
        var context = new WorkflowOutputContext();

        // Act
        var result = context.TryGetResultAs<int>("step1", "op1", out var actualResult);

        // Assert
        Assert.False(result);
        Assert.Equal(default, actualResult);
    }

    #endregion

    #region TryGetLastResult

    [Fact]
    public void TryGetLastResult_WhenOperationsExist_ShouldReturnLastResult()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var op1 = new TestOperation<int>();
        op1.Result = 10;
        var op2 = new TestOperation<int>();
        op2.Result = 20;
        context.AddOperation("step1", "op1", op1);
        context.AddOperation("step1", "op2", op2);

        // Act
        var result = context.TryGetLastResultAs<int>(out var actualResult);

        // Assert
        Assert.True(result);
        Assert.Equal(20, actualResult);
    }

    [Fact]
    public void TryGetLastResult_WhenNoOperations_ShouldReturnDefault()
    {
        // Arrange
        var context = new WorkflowOutputContext();

        // Act
        var result = context.TryGetLastResultAs<int>(out var actualResult);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region AddOperation

    [Fact]
    public void AddOperation_WithValidData_ShouldStoreOperation()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();

        // Act
        context.AddOperation("step1", "op1", testOp);

        // Assert
        var result = context.GetOperationAs<TestOperation<int>>("step1", "op1");
        Assert.Same(testOp, result);
    }

    [Fact]
    public void AddOperation_WithNullStepId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => context.AddOperation(null!, "op1", testOp));
    }

    [Fact]
    public void AddOperation_WithEmptyStepId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => context.AddOperation("", "op1", testOp));
    }

    [Fact]
    public void AddOperation_WithNullResultId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => context.AddOperation("step1", null!, testOp));
    }

    [Fact]
    public void AddOperation_WithEmptyResultId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var testOp = new TestOperation<int>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => context.AddOperation("step1", "", testOp));
    }

    [Fact]
    public void AddOperation_WithNullOperation_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = new WorkflowOutputContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => context.AddOperation("step1", "op1", (ITaskOperation)null!));
    }

    [Fact]
    public void AddOperation_MultipleKeysInSameStep_ShouldStoreSeparately()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var op1 = new TestOperation<int>();
        var op2 = new TestOperation<string>();

        // Act
        context.AddOperation("step1", "op1", op1);
        context.AddOperation("step1", "op2", op2);

        // Assert
        var result1 = context.GetOperationAs<TestOperation<int>>("step1", "op1");
        var result2 = context.GetOperationAs<TestOperation<string>>("step1", "op2");
        Assert.Same(op1, result1);
        Assert.Same(op2, result2);
    }

    [Fact]
    public void AddOperation_MultipleSteps_ShouldStoreIndependently()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var op1 = new TestOperation<int>();
        var op2 = new TestOperation<string>();

        // Act
        context.AddOperation("step1", "op1", op1);
        context.AddOperation("step2", "op1", op2);

        // Assert
        var result1 = context.GetOperationAs<TestOperation<int>>("step1", "op1");
        var result2 = context.GetOperationAs<TestOperation<string>>("step2", "op1");
        Assert.Same(op1, result1);
        Assert.Same(op2, result2);
    }

    [Fact]
    public void AddOperation_OverwritingSameKey_ShouldReplaceExisting()
    {
        // Arrange
        var context = new WorkflowOutputContext();
        var op1 = new TestOperation<int>();
        var op2 = new TestOperation<int>();

        // Act
        context.AddOperation("step1", "op1", op1);
        context.AddOperation("step1", "op1", op2);

        // Assert
        var result = context.GetOperationAs<TestOperation<int>>("step1", "op1");
        Assert.Same(op2, result);
    }

    #endregion
}
