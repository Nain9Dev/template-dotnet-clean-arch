# Multi-stage Dockerfile for .NET 10 Clean Architecture API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution and project files first for optimized layer caching
COPY template-dotnet-clean-arch.slnx ./
COPY src/Core/Template.Domain/Template.Domain.csproj src/Core/Template.Domain/
COPY src/Core/Template.Application/Template.Application.csproj src/Core/Template.Application/
COPY src/Infrastructure/Template.Infrastructure/Template.Infrastructure.csproj src/Infrastructure/Template.Infrastructure/
COPY src/Presentation/Template.Api/Template.Api.csproj src/Presentation/Template.Api/
COPY tests/Template.Domain.UnitTests/Template.Domain.UnitTests.csproj tests/Template.Domain.UnitTests/
COPY tests/Template.Api.IntegrationTests/Template.Api.IntegrationTests.csproj tests/Template.Api.IntegrationTests/

RUN dotnet restore template-dotnet-clean-arch.slnx

# Copy entire source code and build
COPY . .
RUN dotnet publish src/Presentation/Template.Api/Template.Api.csproj -c Release -o /out --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000

COPY --from=build /out ./
ENTRYPOINT ["dotnet", "Template.Api.dll"]
