# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

IncidentIQ is an AI-powered interactive security training platform built with .NET 8, Entity Framework Core, and Blazor Server. The platform generates personalized phishing and social engineering scenarios using OpenAI's GPT models to train employees on cybersecurity awareness.

## Architecture

The solution follows Clean Architecture principles with four distinct layers:

- **Domain** (`IncidentIQ.Domain`): Core business entities, enums, and domain logic
- **Application** (`IncidentIQ.Application`): Application services, DTOs, and interfaces  
- **Infrastructure** (`IncidentIQ.Infrastructure`): Data persistence, external APIs, and service implementations
- **Presentation** (`IncidentIQ.Web`): Blazor Server components, controllers, and UI logic

### Key Dependencies

- **Entity Framework Core 8.0.11** with SQL Server provider for data persistence
- **Microsoft.SemanticKernel 1.61.0** for AI orchestration and agents
- **Microsoft.AspNetCore.Identity** for user authentication and authorization
- **StackExchange.Redis** for caching and session management
- **Blazor Server** for interactive web UI components

### AI Agent Architecture

The system implements several specialized AI agents:
- `IScenarioGeneratorAgent`: Generates personalized training scenarios
- `ICoachingAgent`: Provides real-time coaching during training sessions
- `IBehaviorAnalystAgent`: Analyzes user behavior patterns (interface only)
- `IContentValidatorAgent`: Validates generated content (interface only)

All agents are orchestrated through `ISemanticKernelService` which handles OpenAI API integration.

## Development Commands

### Building and Running

```bash
# Restore dependencies
dotnet restore

# Build the entire solution
dotnet build

# Run the web application (from project root)
cd src/IncidentIQ.Web
dotnet run --urls="http://localhost:5001"

# Development with hot reload
dotnet watch run --urls="http://localhost:5001"
```

### Database Operations

```bash
# Create a new migration
dotnet ef migrations add <MigrationName> --startup-project src/IncidentIQ.Web --project src/IncidentIQ.Infrastructure

# Apply migrations to database
dotnet ef database update --startup-project src/IncidentIQ.Web --project src/IncidentIQ.Infrastructure

# Drop database (development only)
dotnet ef database drop --startup-project src/IncidentIQ.Web --project src/IncidentIQ.Infrastructure --force
```

### Docker Development

```bash
# Start only database and Redis dependencies
docker compose up -d sqlserver redis

# Start all services including web application
docker compose up -d

# View logs from web application
docker compose logs -f web

# Stop all services
docker compose down
```

## Configuration Requirements

### Required Environment Variables/Settings

The application requires OpenAI API configuration:

**Option 1: Environment Variables (Recommended)**
```bash
export OPENAI_API_KEY=your_actual_api_key_here
```

**Option 2: appsettings.Development.json**
```json
{
  "OpenAI": {
    "ApiKey": "your_actual_api_key_here"
  }
}
```

### Database Configuration

- **SQL Server 2022** runs on port 1433 via Docker
- Default credentials: `sa` / `IncidentIQ123!` (development only)
- **Redis** cache on port 6379
- Connection strings are configured in docker-compose.yml and appsettings.json

## Data Model Key Points

### Entity Framework Configuration

The `ApplicationDbContext` uses extensive JSON serialization for complex data structures:
- `User.SecurityProfile`: Stores vulnerability patterns, learning preferences, company context
- `TrainingScenario.Content`: Interactive elements, decision points, consequences
- `TrainingSession.Responses`: User responses and coaching interactions
- `BehavioralAnalytics`: Performance history and risk profiles

### Important Entity Relationships

- `User` → `TrainingSession` (one-to-many)
- `TrainingScenario` → `TrainingSession` (one-to-many)  
- `TrainingSession` → `AgentInteraction` (one-to-many)
- All entities inherit from `BaseEntity` with automatic timestamp management

## Development Patterns

### Service Registration Pattern

Services are registered in Program.cs using dependency injection:
- AI agents are registered as scoped services
- Database context uses SQL Server with automatic migration
- Redis caching with memory cache fallback
- Identity services with custom password requirements

### Error Handling

The application implements automatic database migration with fallback:
- Attempts migrations on startup
- Falls back to EnsureCreated() in development if migrations fail
- Comprehensive logging for database connection issues

### AI Integration

All AI functionality goes through `SemanticKernelService`:
- Scenario generation uses structured prompts with user context
- Coaching agents provide real-time feedback during training sessions
- Content validation ensures appropriate and safe training materials

## Testing

Currently no test framework is configured. When adding tests:
- Use `Microsoft.EntityFrameworkCore.InMemory` for database testing (already referenced)
- Consider testing AI agents with mock ISemanticKernelService implementations

## Security Considerations

- OpenAI API keys must never be committed to git
- appsettings.Development.json and .env files are git-ignored
- SQL Server uses TrustServerCertificate=true (development only)
- User authentication handled through ASP.NET Core Identity