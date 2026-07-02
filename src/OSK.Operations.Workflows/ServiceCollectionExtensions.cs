using Microsoft.Extensions.DependencyInjection;
using OSK.Operations.Workflows.Managers;
using OSK.Operations.Workflows.Ports;

namespace OSK.Operations.Workflows;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflows(this IServiceCollection services)
    {
        services.AddTransient<ITaskOperationManager, TaskOperationManager>();

        return services;
    }
}
