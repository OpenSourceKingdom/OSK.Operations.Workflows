using OSK.Operations.Workflows.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Operations.Workflows.UnitTests._Helpers;

public class TestOperation<T> : TestOperation, ITaskOperation<T>
{
    public T? Result { get; set; }
}
