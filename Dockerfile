FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY PaymentSystem.slnx .
COPY src/API/PaymentSystem.Api/PaymentSystem.Api.csproj src/API/PaymentSystem.Api/
COPY src/Modules/Payments/PaymentSystem.Modules.Payments.Domain/PaymentSystem.Modules.Payments.Domain.csproj src/Modules/Payments/PaymentSystem.Modules.Payments.Domain/
COPY src/Modules/Payments/PaymentSystem.Modules.Payments.Application/PaymentSystem.Modules.Payments.Application.csproj src/Modules/Payments/PaymentSystem.Modules.Payments.Application/
COPY src/Modules/Payments/PaymentSystem.Modules.Payments.Infrastructure/PaymentSystem.Modules.Payments.Infrastructure.csproj src/Modules/Payments/PaymentSystem.Modules.Payments.Infrastructure/
COPY src/Modules/Payments/PaymentSystem.Modules.Payments.Presentation/PaymentSystem.Modules.Payments.Presentation.csproj src/Modules/Payments/PaymentSystem.Modules.Payments.Presentation/
COPY src/Shared/PaymentSystem.Shared.Domain/PaymentSystem.Shared.Domain.csproj src/Shared/PaymentSystem.Shared.Domain/
COPY src/Shared/PaymentSystem.Shared.Application/PaymentSystem.Shared.Application.csproj src/Shared/PaymentSystem.Shared.Application/
COPY src/Shared/PaymentSystem.Shared.Infrastructure/PaymentSystem.Shared.Infrastructure.csproj src/Shared/PaymentSystem.Shared.Infrastructure/
COPY src/Shared/PaymentSystem.Shared.Presentation/PaymentSystem.Shared.Presentation.csproj src/Shared/PaymentSystem.Shared.Presentation/

# Restore
RUN dotnet restore src/API/PaymentSystem.Api/PaymentSystem.Api.csproj

# Copy all source code
COPY . .

# Build and publish
WORKDIR /src/src/API/PaymentSystem.Api
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PaymentSystem.Api.dll"]