using Dapper;
using PaymentSystem.Shared.Application.Clock;
using PaymentSystem.Shared.Infrastructure.Clock;
using PaymentSystem.Shared.Infrastructure.Data;
using PaymentSystem.Shared.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;

namespace PaymentSystem.Shared.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string databaseConnectionString)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());

        services.AddQuartz(configurator =>
        {
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}