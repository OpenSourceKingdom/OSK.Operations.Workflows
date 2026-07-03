using OSK.Operations.Workflows.Internal;
using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Options;
using OSK.Operations.Workflows.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Operations.Workflows.Managers;

/// <inheritdoc/>
public class TaskOperationManager : ITaskOperationManager
{
    #region Variables

    private TaskOperationManagerSettings _settings = new();

    private readonly Dictionary<string, ManagedOperationGroup> _operationGroupnLookup = [];

    #endregion

    #region 

    /// <inheritdoc/>
    public event Action<ITaskOperation>? OnOperationFinished;

    /// <inheritdoc/>
    public ManagedOperation? AddOperation(ITaskOperation operation, ManagedTaskOptions? options = null)
    {
        if (operation is null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        var taskId = string.IsNullOrWhiteSpace(options?.TaskId)
            ? operation.GetType().FullName
            : options.TaskId;

        if (!_operationGroupnLookup.TryGetValue(taskId, out var operationGroup))
        {
            operationGroup = new ManagedOperationGroup();
            _operationGroupnLookup[taskId] = operationGroup;
        }

        var groupOptions = GetGroupOptions(taskId);
        if (!groupOptions.MaxConcurrentOperations.HasValue || operationGroup.Active.Count < groupOptions.MaxConcurrentOperations.Value)
        {
            operationGroup.Active.Add(operation);
            return new ManagedOperation { TaskGroupId = taskId, Task = operation };
        }

        if (!groupOptions.MaxQueueSize.HasValue || operationGroup.Queue.Count < groupOptions.MaxQueueSize.Value)
        {
            operationGroup.Queue.Add(operation);
            return new ManagedOperation { TaskGroupId = taskId, Task = operation };
        }

        return null;
    }

    /// <inheritdoc/>
    public void Configure(Action<TaskOperationManagerSettings> options)
    {
        _settings = new();
        options?.Invoke(_settings);

        ValidateSettings(_settings.Defaults, "Defaults");
        foreach (var groupOption in _settings.GroupOptions ?? [])
        {
            ValidateSettings(groupOption.Value, groupOption.Key);
        }
    }

    /// <inheritdoc/>
    public void Update(TimeSpan deltaTime)
    {
        foreach (var groupKey in _operationGroupnLookup.Keys.ToArray())
        {
            var operationGroup = _operationGroupnLookup[groupKey];
            var groupOptions = GetGroupOptions(groupKey);
            UpdateGroup(operationGroup, groupOptions, deltaTime);
        }
    }

    #endregion

    #region Helpers

    private void UpdateGroup(ManagedOperationGroup operationGroup, ManagedOperationGroupOptions groupOptions, TimeSpan deltaTime)
    {
        var operationsToRemove = new List<ITaskOperation>();
        foreach (var operation in operationGroup.Active.ToArray())
        {
            operation.Iterate(deltaTime);
            if (operation.IsFinished)
            {
                operationsToRemove.Add(operation);
            }
        }
        foreach (var operation in operationsToRemove)
        {
            operationGroup.Active.Remove(operation);
            OnOperationFinished?.Invoke(operation);
        }

        operationsToRemove = groupOptions.MaxConcurrentOperations.HasValue
            ? [.. operationGroup.Queue.Take(groupOptions.MaxConcurrentOperations.Value - operationGroup.Active.Count)]
            : [];
        foreach (var operation in operationsToRemove)
        {
            operationGroup.Queue.Remove(operation);
            operationGroup.Active.Add(operation);
        }
    }

    private ManagedOperationGroupOptions GetGroupOptions(string groupId)
    {
        return _settings.GroupOptions is null || !_settings.GroupOptions.TryGetValue(groupId, out var groupOptions)
            ? _settings.Defaults
            : groupOptions;
    }

    private void ValidateSettings(ManagedOperationGroupOptions groupOptions, string optionName)
    {
        if (groupOptions is null)
        {
            throw new InvalidOperationException($"Group options for '{optionName}' must be provided in the settings.");
        }
        if (groupOptions.MaxConcurrentOperations.HasValue && groupOptions.MaxConcurrentOperations.Value <= 0)
        {
            throw new InvalidOperationException($"MaxConcurrentOperations for '{optionName}' must be greater than zero.");
        }
        if (groupOptions.MaxQueueSize.HasValue && groupOptions.MaxQueueSize.Value < 0)
        {
            throw new InvalidOperationException($"MaxQueueSize for '{optionName}' must be zero or greater.");
        }
    }

    internal ManagedOperationGroup? GetGroup(string groupId)
        => _operationGroupnLookup.TryGetValue(groupId, out var operationGroup)
        ? operationGroup 
        : null;

    #endregion
}
