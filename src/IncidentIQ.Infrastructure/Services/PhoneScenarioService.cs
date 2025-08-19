using IncidentIQ.Application.Interfaces;
using IncidentIQ.Application.Interfaces.AI;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;
using IncidentIQ.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IncidentIQ.Infrastructure.Services;

public class PhoneScenarioService : IPhoneScenarioService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PhoneScenarioService> _logger;

    public PhoneScenarioService(
        ApplicationDbContext context,
        ILogger<PhoneScenarioService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PhoneCallScenario> CreateScenarioAsync(string userId, string targetRole, string targetCompany)
    {
        try
        {
            // Create a standard scenario without AI generation to avoid OpenAI dependency
            // The dynamic conversation happens later via Claude API
            var scenario = new PhoneCallScenario
            {
                Id = Guid.NewGuid(),
                Title = "Customer Service Social Engineering Attack",
                Description = "Interactive phone call with a social engineer pretending to be an angry customer",
                Type = ScenarioType.CustomerServiceCall,
                Difficulty = DifficultyLevel.Medium,
                TargetCompany = targetCompany,
                TargetRole = targetRole,
                CallerProfile = new CallerProfile
                {
                    Name = "Jennifer Clark",
                    Company = "CustomerCorp",
                    PhoneNumber = "+1 (555) 0123",
                    Role = "Premium Customer",
                    Persona = "Frustrated customer with urgent IT issues"
                },
                LearningObjectives = new List<string>
                {
                    "Recognize social engineering tactics in phone calls",
                    "Maintain security protocols under pressure",
                    "Identify red flags in customer requests",
                    "Practice proper verification procedures"
                },
                PlannedTactics = new List<ManipulationTactic>
                {
                    ManipulationTactic.Authority,
                    ManipulationTactic.Urgency,
                    ManipulationTactic.Fear,
                    ManipulationTactic.Reciprocity
                },
                CreatedAt = DateTime.UtcNow
            };

            _context.Add(scenario);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Created phone scenario {ScenarioId} for user {UserId}", scenario.Id, userId);
            return scenario;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating phone scenario for user {UserId}", userId);
            throw;
        }
    }

    public async Task<PhoneCallSession> StartSessionAsync(string userId, Guid scenarioId)
    {
        try
        {
            var scenario = await _context.Set<PhoneCallScenario>()
                .FirstOrDefaultAsync(s => s.Id == scenarioId);

            if (scenario == null)
                throw new ArgumentException($"Scenario {scenarioId} not found");

            var session = new PhoneCallSession
            {
                Id = Guid.NewGuid(),
                UserId = GenerateUserGuid(userId),
                ScenarioId = scenarioId,
                CallState = CallState.Incoming,
                CreatedAt = DateTime.UtcNow,
                CurrentNodeId = "initial"
            };

            _context.Add(session);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Started phone session {SessionId} for user {UserId}", session.Id, userId);
            return session;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting phone session for user {UserId}, scenario {ScenarioId}", userId, scenarioId);
            throw;
        }
    }

    public async Task<PhoneCallSession?> GetActiveSessionAsync(string userId)
    {
        return await _context.Set<PhoneCallSession>()
            .Include(s => s.Scenario)
            .FirstOrDefaultAsync(s => s.UserId == GenerateUserGuid(userId) && 
                                    s.CallState != CallState.Ended);
    }

    public async Task<PhoneCallSession> UpdateSessionAsync(Guid sessionId, CallState newState)
    {
        var session = await _context.Set<PhoneCallSession>()
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
            throw new ArgumentException($"Session {sessionId} not found");

        session.CallState = newState;

        if (newState == CallState.Active && !session.CallStartedAt.HasValue)
        {
            session.CallStartedAt = DateTime.UtcNow;
        }
        else if (newState == CallState.Ended && !session.CallEndedAt.HasValue)
        {
            session.CallEndedAt = DateTime.UtcNow;
            if (session.CallStartedAt.HasValue)
            {
                session.CallDurationSeconds = (int)(DateTime.UtcNow - session.CallStartedAt.Value).TotalSeconds;
            }
        }

        await _context.SaveChangesAsync();
        return session;
    }

    public async Task EndSessionAsync(Guid sessionId)
    {
        await UpdateSessionAsync(sessionId, CallState.Ended);
    }

    public async Task<List<PhoneCallScenario>> GetScenariosForRoleAsync(string role)
    {
        return await _context.Set<PhoneCallScenario>()
            .Where(s => s.TargetRole == role || string.IsNullOrEmpty(s.TargetRole))
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<PhoneCallSession?> GetSessionByIdAsync(Guid sessionId)
    {
        return await _context.Set<PhoneCallSession>()
            .Include(s => s.Scenario)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    private static Guid GenerateUserGuid(string userId)
    {
        // Generate a consistent GUID for demo users
        if (userId == "demo-user-123")
            return new Guid("12345678-1234-1234-1234-123456789abc");
        
        // For other cases, try to parse as GUID first, then generate from hash
        if (Guid.TryParse(userId, out var guid))
            return guid;
            
        // Generate deterministic GUID from string hash
        using var md5 = System.Security.Cryptography.MD5.Create();
        var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(userId));
        return new Guid(hash);
    }
}