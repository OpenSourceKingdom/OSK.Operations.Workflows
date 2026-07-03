using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.UnitTests._Helpers;

public class TestOperation<T> : TestOperation, ITaskOperation<T>
{
    public T? Result { get; set; }
}
