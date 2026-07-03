using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.UnitTests._Helpers;

public class TestOperation() : IterativeOperation, ITaskOperation
{
    public OperationState DesiredIterationState { get; set; }

    public int InitializeCalled { get; private set; }

    public int IteratationsCalled { get; private set; }

    public TimeSpan? Delay { get; set; }

    public string? Message { get; set; }

    public override int TotalWorkItems => 1;

    protected override void Initialize()
    {
        InitializeCalled++;
    }

    protected override OperationStatus RunIteration(TimeSpan delta)
    {
        IteratationsCalled++;
        if (Delay.HasValue && Delay.Value > TimeSpan.Zero)
        {
            Delay = Delay.Value - delta;
            return OperationStatus.ProgressUpdate(.5, "Delaying");
        }

        return DesiredIterationState is OperationState.Failed
            ? OperationStatus.Failed(string.IsNullOrWhiteSpace(Message) ? "Failed" : Message)
            : DesiredIterationState is OperationState.Aborted
                ? OperationStatus.Aborted(string.IsNullOrWhiteSpace(Message) ? "Aborted" : Message)
                : DesiredIterationState is OperationState.Complete ? OperationStatus.Complete : OperationStatus.ProgressUpdate(.5);
    }
}
