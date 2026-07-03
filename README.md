# OSK.Operations.Workflows

This library helps to handle running operations iteratively, over the course of one or many steps. These operations can then be grouped together within a workflow, which is a sequence of steps, to
run and build a larger flow of work to perform. This is especially useful in instances where standard async or other operations that need to be ran and managed is required - for example, in game engines
across multiple frames, or in other applications where you need to run a series of operations over time.

## Usage

Add the required dependencies to your DI container, if you are using one, using `AddWorkflows`, but most of the constructs required are avialable without DI to help facilitate a broader usage within game engines.

Primary components within the library are:
- `WorkflowStep`: a configuration object that defines how a group of operations should be run together, using in conjunction with an `IWorkflowRunner` to run the operations in a workflow.
- `IOperationRunner`: a class that can be used to run a group of operations together as a unit of work
- `IWorkflowRunner`: a class that can be used to run a workflow consisting of multiple steps
  - Data can be referenced from earlier steps in the workflow using an output context if required
- `IIterativeOperation`: an interface that defines the contract for iterative operations, that is objects that have tasks to run over one or many steps
- `IIterativeOperation<T>`: a generic interface for iterative operations with a specific result type
- `ITaskOperation`: a specific type of iterative operation that is designed to run a single task, and can be used to wrap existing async operations into an iterative operation

With these components, you can build workflows that run operations over time, manage their execution, and handle their results in a structured manner, such saving, loading, or more.

Currently defined operations include:
- Timers
- Async
- Action
- No-op style operations