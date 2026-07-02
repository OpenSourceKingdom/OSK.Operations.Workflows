using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.UnitTests._Helpers;

internal class TestOperation(int group, int index) : IIterativeOperation
{
    public int Group => group;
    public int Index => index;

    public OperationState TestState { get; set; }

    public int TotalWorkItems => 1;

    public OperationStatus Status { get; set; }

    public OperationState Iterate(TimeSpan deltaTime)
    {
        Status = TestState is OperationState.Failed
            ? OperationStatus.Failed("Failed")
            : TestState is OperationState.Aborted
                ? OperationStatus.Aborted("Aborted")
                : TestState is OperationState.Complete ? OperationStatus.Complete : OperationStatus.ProgressUpdate(.5);

        return TestState;
    }
}
