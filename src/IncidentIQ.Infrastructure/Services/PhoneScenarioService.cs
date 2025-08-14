using IncidentIQ.Application.Interfaces;
using IncidentIQ.Application.Interfaces.AI;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;
using IncidentIQ.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace IncidentIQ.Infrastructure.Services;

public class PhoneScenarioService : IPhoneScenarioService
{
    private readonly ApplicationDbContext _context;
    private readonly ISemanticKernelService _kernelService;
    private readonly ILogger<PhoneScenarioService> _logger;

    public PhoneScenarioService(
        ApplicationDbContext context,
        ISemanticKernelService kernelService,
        ILogger<PhoneScenarioService> logger)
    {
        _context = context;
        _kernelService = kernelService;
        _logger = logger;
    }

    public async Task<PhoneCallScenario> CreateScenarioAsync(string userId, string targetRole, string targetCompany)
    {
        try
        {
            var prompt = $@"Create a realistic customer service social engineering attack scenario.
            
            Target Details:
            - Role: {targetRole}
            - Company: {targetCompany}
            - User ID: {userId}
            
            Generate a JSON response with the following structure:
            {{
                ""title"": ""Descriptive title for the scenario"",
                ""description"": ""Brief description of what the user will experience"",
                ""callerProfile"": {{
                    ""name"": ""Fake customer name"",
                    ""company"": ""Fake company name (not the target company)"",
                    ""phoneNumber"": ""Fake phone number"",
                    ""role"": ""Premium customer or similar"",
                    ""persona"": ""Angry, frustrated, demanding""
                }},
                ""learningObjectives"": [
                    ""What the user should learn from this scenario""
                ],
                ""plannedTactics"": [
                    ""Authority"", ""Urgency"", ""Fear"", ""Reciprocity""
                ]
            }}
            
            Make it realistic for a {targetRole} at {targetCompany}.
            The hacker will pretend to be an angry customer trying to get the employee to click a link or provide access.";

            var arguments = new KernelArguments
            {
                ["targetRole"] = targetRole,
                ["targetCompany"] = targetCompany,
                ["userId"] = userId
            };

            var jsonResponse = await _kernelService.ExecutePromptAsync(prompt, arguments);
            
            // Parse the response and create the scenario
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
                UserId = Guid.Parse(userId),
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
            .FirstOrDefaultAsync(s => s.UserId == Guid.Parse(userId) && 
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
}