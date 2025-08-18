# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

IncidentIQ is an AI-powered interactive security training platform built with .NET 8, Entity Framework Core, and Blazor Server. The platform generates personalized phishing and social engineering scenarios using Claude API (primary) and OpenAI's GPT models (fallback) to train employees on cybersecurity awareness.

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
- **Claude API (Anthropic)** for advanced phishing simulation conversations
- **HttpClient** for external AI API integration

### AI Agent Architecture

The system implements several specialized AI agents:
- `IScenarioGeneratorAgent`: Generates personalized training scenarios
- `ICoachingAgent`: Provides real-time coaching during training sessions
- `IBehaviorAnalystAgent`: Analyzes user behavior patterns (interface only)
- `IContentValidatorAgent`: Validates generated content (interface only)
- `IClaudeApiService`: Handles Claude API integration for phishing simulations

AI services are orchestrated through:
- `ISemanticKernelService` for OpenAI/general AI functionality
- `IClaudeApiService` for specialized phishing conversation generation
- Automatic fallback from Claude → OpenAI → hardcoded responses

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

The application supports multiple AI providers with automatic fallback:

#### Claude API (Primary for Phone Training)
**Option 1: Environment Variables (Recommended)**
```bash
export ANTHROPIC_API_KEY=your_claude_api_key_here
```

**Option 2: appsettings.Development.json**
```json
{
  "Claude": {
    "ApiKey": "your_claude_api_key_here"
  }
}
```

#### OpenAI API (Fallback/General AI)
**Option 1: Environment Variables (Recommended)**
```bash
export OPENAI_API_KEY=your_openai_api_key_here
```

**Option 2: appsettings.Development.json**
```json
{
  "OpenAI": {
    "ApiKey": "your_openai_api_key_here"
  }
}
```

#### API Priority and Fallback
The phone training system uses the following priority order:
1. **Claude API** - Primary for Jennifer Clark phishing simulations
2. **OpenAI/Semantic Kernel** - Fallback when Claude is unavailable
3. **Hardcoded responses** - Final fallback for reliability

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

The platform uses multiple AI services with intelligent fallback:

**Claude API Integration (`ClaudeApiService`)**:
- Primary service for Jennifer Clark phishing simulations
- Uses Claude-3.5-Sonnet for natural conversation flow
- Specialized prompts for social engineering tactics
- Handles conversation history and context management

**OpenAI Integration (`SemanticKernelService`)**:
- General AI functionality and scenario generation
- Fallback for when Claude API is unavailable
- Coaching agents provide real-time feedback during training sessions
- Content validation ensures appropriate and safe training materials

**Conversation Flow (`ConversationFlowService`)**:
- Orchestrates AI responses with multi-tier fallback
- Tracks user responses and social engineering tactics
- Analyzes security alerts and risk levels
- Provides educational feedback and recommendations

## Testing

Currently no test framework is configured. When adding tests:
- Use `Microsoft.EntityFrameworkCore.InMemory` for database testing (already referenced)
- Consider testing AI agents with mock ISemanticKernelService implementations

## Security Considerations

- **API Keys**: Claude API and OpenAI API keys must never be committed to git
- **Configuration Files**: appsettings.Development.json and .env files are git-ignored
- **Database Security**: SQL Server uses TrustServerCertificate=true (development only)
- **User Authentication**: Handled through ASP.NET Core Identity
- **AI Safety**: All AI-generated content is for educational phishing simulation only
- **Fallback Security**: System gracefully degrades to hardcoded responses if AI APIs fail