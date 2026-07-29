using Dapper;
using PaymentSystem.Shared.Application.Clock;
using PaymentSystem.Shared.Application.Data;
using PaymentSystem.Shared.Infrastructure.Clock;
using PaymentSystem.Shared.Infrastructure.Data;
using PaymentSystem.Shared.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Quartz;

namespace PaymentSystem.Shared.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string databaseConnectionString)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddSingleton(_ =>
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(databaseConnectionString);
            dataSourceBuilder.EnableDynamicJson();
            return dataSourceBuilder.Build();
        });

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());

        services.AddScoped<InsertOutboxMessagesInterceptor>();

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