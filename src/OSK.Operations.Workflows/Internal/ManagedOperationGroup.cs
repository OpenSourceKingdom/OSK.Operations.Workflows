using OSK.Operations.Workflows.Models;
using System.Collections.Generic;

namespace OSK.Operations.Workflows.Internal;

internal class ManagedOperationGroup
{    
    public HashSet<ITaskOperation> Active = [];

    public HashSet<ITaskOperation> Queue = [];
}
