using Microsoft.Extensions.DependencyInjection;
using OSK.Operations.Workflows.Managers;
using OSK.Operations.Workflows.Ports;

namespace OSK.Operations.Workflows;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds workflow related services to the service collection
    /// </summary>
    /// <param name="services">The services to add the dependencies to</param>
    /// <returns>The services for chaining</returns>
    public static IServiceCollection AddWorkflows(this IServiceCollection services)
    {
        services.AddTransient<ITaskOperationManager, TaskOperationManager>();

        return services;
    }
}
