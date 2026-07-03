using OSK.Operations.Workflows.Managers;
using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Options;
using OSK.Operations.Workflows.UnitTests._Helpers;

namespace OSK.Operations.Workflows.UnitTests.Managers;

public class TaskOperationManagerTests
{
    #region Variables

    private readonly TaskOperationManager _manager;

    #endregion

    #region Constructors

    public TaskOperationManagerTests()
    {
        _manager = new TaskOperationManager();
    }

    #endregion

    #region AddOperation

    [Fact]
    public void AddOperation_NullOperation_ShouldThrowArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _manager.AddOperation(null!));
    }

    [Fact]
    public void AddOperation_MaxConcurrentOperationsReached_QueueHasSpace_ShouldAddToQueue()
    {
        // Arrange
        var expectedId = typeof(TestOperation).FullName!;

        _manager.Configure(o => o.Defaults = new ManagedOperationGroupOptions
        {
            MaxConcurrentOperations = 1,
            MaxQueueSize = 2
        });

        _manager.AddOperation(new TestOperation());

        // Act
        var result = _manager.AddOperation(new TestOperation());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedId, result.TaskGroupId);

        var operationGroup = _manager.GetGroup(expectedId);
        Assert.Single(operationGroup!.Active);
        Assert.Single(operationGroup.Queue);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void AddOperation_MaxQueueOperationsReached_ShouldReturnNull(int maxQueueSize)
    {
        // Arrange
        var expectedId = typeof(TestOperation).FullName!;

        _manager.Configure(o => o.Defaults = new ManagedOperationGroupOptions
        {
            MaxConcurrentOperations = 1,
            MaxQueueSize = maxQueueSize
        });

        _manager.AddOperation(new TestOperation());
        while (maxQueueSize >= 1)
        {
            _manager.AddOperation(new TestOperation());
            maxQueueSize--;
        }

        // Act
        var result = _manager.AddOperation(new TestOperation());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void AddOperation_WithoutOptions_ShouldUseTypeFullNameAsTaskId()
    {
        // Arrange
        var operation = new TestOperation();
        var expectedId = typeof(TestOperation).FullName;

        // Act
        var result = _manager.AddOperation(operation);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedId, result.TaskGroupId);
    }

    [Fact]
    public void AddOperation_WithCustomTaskId_ShouldUseProvidedTaskId()
    {
        // Arrange
        var operation = new TestOperation();
        var customId = "custom-task-id";

        // Act
        var result = _manager.AddOperation(operation, new ManagedTaskOptions { TaskId = customId });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customId, result.TaskGroupId);
    }

    [Fact]
    public void AddOperation_FirstOperation_ShouldCreateNewGroup()
    {
        // Arrange
        var operation = new TestOperation();

        // Act
        var result = _manager.AddOperation(operation);

        // Assert
        Assert.NotNull(result);
        Assert.Same(operation, result.Task);

        var group = _manager.GetGroup(result.TaskGroupId);
        Assert.NotNull(group);
        Assert.Single(group.Active);
    }

    [Fact]
    public void AddOperation_MultipleOperationsSameId_ShouldAddToExistingGroup()
    {
        // Arrange
        var op1 = new TestOperation();
        var op2 = new TestOperation();
        var options = new ManagedTaskOptions { TaskId = "same-group" };

        // Act
        _manager.AddOperation(op1, options);
        var result = _manager.AddOperation(op2, options);

        // Assert
        Assert.NotNull(result);
        Assert.Same(op2, result.Task);

        var group = _manager.GetGroup(options.TaskId);
        Assert.NotNull(group);
        Assert.Equal(2, group.Active.Count);
    }

    [Fact]
    public void AddOperation_MultipleOperationsSameOperation_ShouldOnlyAddOnce()
    {
        // Arrange
        var op1 = new TestOperation();
        var options = new ManagedTaskOptions { TaskId = "same-group" };

        // Act
        _manager.AddOperation(op1, options);
        var result = _manager.AddOperation(op1, options);

        // Assert
        Assert.NotNull(result);
        Assert.Same(op1, result.Task);

        var group = _manager.GetGroup(options.TaskId);
        Assert.NotNull(group);
        Assert.Single(group.Active);
    }

    #endregion

    #region Configure

    [Fact]
    public void Configure_WithValidSettings_ShouldApplyDefaults()
    {
        // Arrange/Act
        _manager.Configure(settings =>
        {
            settings.Defaults = new ManagedOperationGroupOptions
            {
                MaxConcurrentOperations = 3,
                MaxQueueSize = 5
            };
        });

        // Assert - verify by adding operations and checking behavior
        var op1 = new TestOperation();
        var result = _manager.AddOperation(op1);
        Assert.NotNull(result);
    }

    [Fact]
    public void Configure_WithNullDefaults_ShouldThrowInvalidOperationException()
    {
        // Arrange/Act/Assert
        Assert.Throws<InvalidOperationException>(() => _manager.Configure(o => o.Defaults = null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Configure_DefaultsWithInvalidConcurrentLimit_ShouldThrowInvalidOperationException(int value)
    {
        // Arrange/Act/Assert
        Assert.Throws<InvalidOperationException>(() => _manager.Configure(o =>
        {
            o.Defaults = new ManagedOperationGroupOptions
            {
                MaxConcurrentOperations = value,
                MaxQueueSize = 5
            };
        }));
    }

    [Theory]
    [InlineData(-1)]
    public void Configure_DefaultsWithInvalidQueueSize_ShouldThrowInvalidOperationException(int value)
    {
        // Arrange/Act/Assert
        Assert.Throws<InvalidOperationException>(() => _manager.Configure(o =>
        {
            o.Defaults = new ManagedOperationGroupOptions
            {
                MaxConcurrentOperations = 2,
                MaxQueueSize = value
            };
        }));
    }

    [Fact]
    public void Configure_WithInvalidGroupOptions_ShouldThrowInvalidOperationException()
    {
        // Arrange/Act/Assert
        Assert.Throws<InvalidOperationException>(() => _manager.Configure(o =>
        {
            o.GroupOptions = new Dictionary<string, ManagedOperationGroupOptions>
            {
                { "group1", null! }
            };
        }));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Configure_OverrideWithInvalidConcurrentLimit_ShouldThrowInvalidOperationException(int value)
    {
        // Arrange/Act/Assert
        Assert.Throws<InvalidOperationException>(() => _manager.Configure(o =>
        {
            o.Defaults = new();
            o.GroupOptions["group1"] = new ManagedOperationGroupOptions
            {
                MaxConcurrentOperations = value,
                MaxQueueSize = 5
            };
        }));
    }

    [Theory]
    [InlineData(-1)]
    public void Configure_OverrideWithInvalidQueueSize_ShouldThrowInvalidOperationException(int value)
    {
        // Arrange/Act/Assert
        Assert.Throws<InvalidOperationException>(() => _manager.Configure(o =>
        {
            o.Defaults = new();
            o.GroupOptions["group1"] = new ManagedOperationGroupOptions
            {
                MaxConcurrentOperations = 5,
                MaxQueueSize = value
            };
        }));
    }

    [Fact]
    public void Configure_WithNullAction_ShouldNotThrow()
    {
        // Arrange/Act/Assert
        _manager.Configure(null!);
    }

    #endregion

    #region Update

    [Fact]
    public void Update_WithActiveOperation_ShouldIterateAllActiveOperations()
    {
        // Arrange
        var testOp = new TestOperation();
        testOp.DesiredIterationState = OperationState.Complete;
        _manager.AddOperation(testOp);

        var finished = false;
        _manager.OnOperationFinished += _ =>
        {
            finished = true;
        };

        // Act
        _manager.Update(TimeSpan.FromSeconds(1));

        // Assert
        Assert.True(finished);
    }

    [Fact]
    public void Update_WhenOperationCompletes_ShouldFireOnOperationFinishedEvent()
    {
        // Arrange
        var fired = false;
        ITaskOperation? capturedOp = null;
        _manager.OnOperationFinished += op => { fired = true; capturedOp = op; };

        var testOp = new TestOperation();
        testOp.DesiredIterationState = OperationState.Complete;
        _manager.AddOperation(testOp);

        // Act
        _manager.Update(TimeSpan.FromSeconds(1));

        // Assert
        Assert.True(fired);
        Assert.Same(testOp, capturedOp);
    }

    [Fact]
    public void Update_WithMultipleGroups_ShouldUpdateAllGroups()
    {
        // Arrange
        var op1 = new TestOperation();
        var op2 = new TestOperation();
        var options1 = new ManagedTaskOptions { TaskId = "group1" };
        var options2 = new ManagedTaskOptions { TaskId = "group2" };

        _manager.AddOperation(op1, options1);
        _manager.AddOperation(op2, options2);

        // Act
        _manager.Update(TimeSpan.FromSeconds(1));

        // Assert - both should have been iterated without error
    }

    #endregion
}
