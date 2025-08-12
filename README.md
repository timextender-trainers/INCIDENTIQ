# IncidentIQ - Developer Setup Guide

IncidentIQ is an AI-powered interactive security training platform built with .NET 8, Entity Framework Core, and Blazor Server.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Docker Desktop](https://www.docker.com/products/docker-desktop) for running dependencies
- [Git](https://git-scm.com/) for version control
- A code editor ([Visual Studio](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/))

## Project Structure

```
IncidentIQ/
├── src/
│   ├── IncidentIQ.Domain/          # Domain entities and business rules
│   ├── IncidentIQ.Application/     # Application services and interfaces  
│   ├── IncidentIQ.Infrastructure/  # Data access and external services
│   └── IncidentIQ.Web/            # Blazor Server web application
├── IncidentIQ.sln                 # Solution file
├── docker-compose.yml             # Docker services configuration
├── .env.example                   # Environment variables template
└── README.md                      # This file
```

### Architecture

The project follows Clean Architecture principles:

- **Domain Layer**: Core business entities, enums, and domain logic
- **Application Layer**: Application services, DTOs, and interfaces
- **Infrastructure Layer**: Data persistence, external APIs, and service implementations
- **Presentation Layer**: Blazor Server components, controllers, and UI logic

## Quick Start

### 1. Clone the Repository

```bash
git clone <repository-url>
cd IncidentIQ
```

### 2. Set Up Environment Variables

Copy the example environment file and configure it:

```bash
cp .env.example .env
```

Edit `.env` with your configuration:

```bash
# OpenAI Configuration (required for AI features)
OPENAI_API_KEY=your_openai_api_key_here

# Azure OpenAI Configuration (optional)
AZURE_OPENAI_API_KEY=your_azure_openai_api_key_here
AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/

# Database Configuration
DB_PASSWORD=IncidentIQ123!

# Application Settings
ASPNETCORE_ENVIRONMENT=Development
```

### 3. Start Required Services

Start the database and cache services using Docker:

```bash
docker compose up -d sqlserver redis
```

This will start:
- **SQL Server 2022** on port `1433`
- **Redis** on port `6379`

### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Apply Database Migrations

```bash
dotnet ef database update --startup-project src/IncidentIQ.Web --project src/IncidentIQ.Infrastructure
```

### 6. Build the Solution

```bash
dotnet build
```

### 7. Run the Application

```bash
cd src/IncidentIQ.Web
dotnet run --urls="http://localhost:5001"
```

The application will be available at `http://localhost:5001`.

## Development Workflow

### Running in Development Mode

For development with hot reload:

```bash
cd src/IncidentIQ.Web
dotnet watch run --urls="http://localhost:5001"
```

### Database Management

#### Create a New Migration

```bash
dotnet ef migrations add <MigrationName> --startup-project src/IncidentIQ.Web --project src/IncidentIQ.Infrastructure
```

#### Update Database

```bash
dotnet ef database update --startup-project src/IncidentIQ.Web --project src/IncidentIQ.Infrastructure
```

#### Drop Database (for development reset)

```bash
dotnet ef database drop --startup-project src/IncidentIQ.Web --project src/IncidentIQ.Infrastructure
```

### Code Quality

#### Build and Check for Warnings

```bash
dotnet build --verbosity normal
```

#### Run Tests (when available)

```bash
dotnet test
```

## Configuration

### Application Settings

The application uses multiple configuration sources:

1. `appsettings.json` - Base configuration
2. `appsettings.Development.json` - Development overrides
3. Environment variables - Production secrets
4. `.env` file - Local development (via Docker Compose)

### Key Configuration Sections

#### Database Connection

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=IncidentIQDb;User Id=sa;Password=IncidentIQ123!;TrustServerCertificate=true;",
    "Redis": "localhost:6379"
  }
}
```

#### AI Services

```json
{
  "OpenAI": {
    "ApiKey": "your-api-key-here"
  },
  "AzureOpenAI": {
    "ApiKey": "your-azure-api-key-here",
    "Endpoint": "https://your-resource.openai.azure.com/"
  }
}
```

## Docker Deployment

### Full Application with Docker Compose

To run the entire application stack:

```bash
# Start all services
docker compose up -d

# View logs
docker compose logs -f web

# Stop all services  
docker compose down
```

### Individual Services

Start only the dependencies:

```bash
docker compose up -d sqlserver redis
```

## Troubleshooting

### Common Issues

#### Port Already in Use

If you get a "port already in use" error:

```bash
# Check what's using the port
netstat -ano | findstr :5001  # Windows
lsof -i :5001                 # macOS/Linux

# Kill the process or use a different port
dotnet run --urls="http://localhost:5002"
```

#### Database Connection Issues

1. Ensure Docker containers are running:
   ```bash
   docker compose ps
   ```

2. Check SQL Server logs:
   ```bash
   docker compose logs sqlserver
   ```

3. Test connection manually:
   ```bash
   docker exec -it incidentiq-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "IncidentIQ123!"
   ```

#### Entity Framework Issues

1. Clear EF cache:
   ```bash
   dotnet ef database drop --startup-project src/IncidentIQ.Web --project src/IncidentIQ.Infrastructure --force
   dotnet ef database update --startup-project src/IncidentIQ.Web --project src/IncidentIQ.Infrastructure
   ```

2. Check migration status:
   ```bash
   dotnet ef migrations list --startup-project src/IncidentIQ.Web --project src/IncidentIQ.Infrastructure
   ```

#### Build Warnings

The current build has some warnings related to:
- Entity Framework value comparers for collection properties
- Async methods without await operators
- Possible null reference returns

These are non-breaking but should be addressed in future iterations.

## API Keys and External Services

### OpenAI Integration

The application requires OpenAI API access for AI-powered features:

1. Get an API key from [OpenAI](https://platform.openai.com/api-keys)
2. Add it to your `.env` file or environment variables
3. The application uses GPT models for scenario generation and coaching

### Redis Caching

Redis is used for:
- Session state management
- AI conversation memory
- Caching training scenarios
- Performance optimization

## Contributing

### Code Style

- Follow standard C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and single-purpose

### Git Workflow

1. Create feature branches from `main`
2. Make atomic commits with clear messages
3. Create pull requests for code review
4. Ensure all builds pass before merging

### Database Changes

- Always create migrations for schema changes
- Test migrations on a clean database
- Consider backward compatibility
- Document significant schema changes

## Production Deployment

### Environment Variables

Set these environment variables in production:

```bash
ASPNETCORE_ENVIRONMENT=Production
OPENAI_API_KEY=<your-production-key>
ConnectionStrings__DefaultConnection=<your-production-db-connection>
ConnectionStrings__Redis=<your-production-redis-connection>
```

### Security Considerations

- Use managed identities in cloud environments
- Store secrets in secure key vaults
- Enable HTTPS in production
- Configure proper CORS policies
- Use connection string encryption

## Support

For development questions or issues:

1. Check this README and troubleshooting section
2. Review existing GitHub issues
3. Create a new issue with detailed reproduction steps
4. Include environment details and error logs

## Related Documentation

- [MARKETING.md](MARKETING.md) - Business overview and feature descriptions
- [SlideDeck.md](SlideDeck.md) - Presentation materials and pitch deck
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Blazor Server Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
- [OpenAI API Documentation](https://platform.openai.com/docs)