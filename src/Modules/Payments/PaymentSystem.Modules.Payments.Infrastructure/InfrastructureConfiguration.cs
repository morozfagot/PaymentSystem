using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using PaymentSystem.Modules.Payments.Application;
using PaymentSystem.Modules.Payments.Application.Operations.Abstractions;
using PaymentSystem.Modules.Payments.Domain.Operations;
using PaymentSystem.Modules.Payments.Infrastructure.Database;
using PaymentSystem.Modules.Payments.Infrastructure.Database.Outbox;
using PaymentSystem.Modules.Payments.Infrastructure.Provider;
using PaymentSystem.Shared.Application.Data;
using PaymentSystem.Shared.Application.Messaging;
using PaymentSystem.Shared.Infrastructure.Outbox;
using Quartz;

namespace PaymentSystem.Modules.Payments.Infrastructure;

/// <summary>
/// DI-регистрация слоя Infrastructure для модуля Payments.
/// </summary>
public static class InfrastructureConfiguration
{
    /// <summary>
    /// Регистрирует PaymentsDbContext (SQLite), репозиторий, UnitOfWork, Outbox-обработку
    /// и HTTP-клиент для provider-simulator.
    /// </summary>
    public static IServiceCollection AddPaymentsInfrastructure(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration)
    {
        // SQLite + EF Core
        services.AddDbContext<PaymentsDbContext>((sp, options) =>
        {
            options.UseSqlite(connectionString);
            options.AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>());
        });

        // Interceptor for outbox
        services.AddScoped<InsertOutboxMessagesInterceptor>();

        // SQLite connection factory (для Dapper в ProcessOutboxJob)
        services.AddSingleton<IDbConnectionFactory>(
            _ => new PaymentsDbConnectionFactory(connectionString));

        // Repository + UnitOfWork
        services.AddScoped<IOperationRepository, OperationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Domain event handlers: регистрируем все реализации DomainEventHandler<>
        // и декорируем каждую IdempotentDomainEventHandler<> для идемпотентности
        AddDomainEventHandlers(services);

        // Outbox options из конфигурации
        services.Configure<OutboxOptions>(configuration.GetSection("Payments:Outbox"));

        // Provider options из конфигурации
        services.Configure<ProviderOptions>(configuration.GetSection(ProviderOptions.SectionName));

        // HTTP-клиент для provider-simulator 
        services.AddHttpClient<ProviderSimulatorClient>((sp, client) =>
        {
            ProviderOptions options = sp.GetRequiredService<IOptions<ProviderOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + '/');
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        // Регистрация IPaymentService
        services.TryAddScoped<IPaymentService>(sp =>
            sp.GetRequiredService<ProviderSimulatorClient>());

        // Quartz
        services.AddQuartz(configurator =>
        {
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        return services;
    }

    private static void AddDomainEventHandlers(IServiceCollection services)
    {
        Type[] domainEventHandlerTypes = AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
            .ToArray();

        foreach (Type domainEventHandler in domainEventHandlerTypes)
        {
            Type? baseType = domainEventHandler.BaseType;
            while (baseType is not null)
            {
                if (baseType.IsGenericType &&
                    baseType.GetGenericTypeDefinition() == typeof(DomainEventHandler<>))
                {
                    break;
                }
                baseType = baseType.BaseType;
            }

            if (baseType is null)
            {
                continue;
            }

            Type domainEventType = baseType.GetGenericArguments().Single();

            services.TryAddScoped(domainEventHandler);

            Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>)
                .MakeGenericType(domainEventType);

            services.Decorate(domainEventHandler, closedIdempotentHandler);
        }
    }
}