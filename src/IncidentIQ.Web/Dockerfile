# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY src/IncidentIQ.Domain/IncidentIQ.Domain.csproj src/IncidentIQ.Domain/
COPY src/IncidentIQ.Application/IncidentIQ.Application.csproj src/IncidentIQ.Application/
COPY src/IncidentIQ.Infrastructure/IncidentIQ.Infrastructure.csproj src/IncidentIQ.Infrastructure/
COPY src/IncidentIQ.Web/IncidentIQ.Web.csproj src/IncidentIQ.Web/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build the application
WORKDIR /src/src/IncidentIQ.Web
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos "" --uid 1000 appuser && chown -R appuser /app
USER appuser

EXPOSE 8080
EXPOSE 8081

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "IncidentIQ.Web.dll"]