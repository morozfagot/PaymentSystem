using System.Reflection;
using PaymentSystem.Api;
using PaymentSystem.Modules.Payments.Infrastructure;
using PaymentSystem.Shared.Application;
using PaymentSystem.Shared.Infrastructure;
using PaymentSystem.Shared.Infrastructure.Configuration;
using PaymentSystem.Shared.Presentation.Endpoints;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

Assembly[] moduleApplicationAssemblies = [
    PaymentSystem.Modules.Payments.Application.AssemblyReference.Assembly];

builder.Services.AddApplication(moduleApplicationAssemblies);

builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionStringOrThrow("PaymentsDatabase"));

string connectionString = builder.Configuration.GetConnectionString("PaymentsDatabase")
    ?? "Data Source=/data/payments.db";

builder.Services.AddPaymentsModule(connectionString, builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ApplyMigrations();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.MapEndpoints();

app.Run();

#pragma warning disable CA1515
public partial class Program;
#pragma warning restore CA1515