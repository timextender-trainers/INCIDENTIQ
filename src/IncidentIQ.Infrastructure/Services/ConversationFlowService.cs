using IncidentIQ.Application.Interfaces;
using IncidentIQ.Application.Interfaces.AI;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;
using IncidentIQ.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace IncidentIQ.Infrastructure.Services;

public class ConversationFlowService : IConversationFlowService
{
    private readonly ApplicationDbContext _context;
    private readonly ISemanticKernelService _kernelService;
    private readonly ISocialEngineeringAnalyzer _analyzer;
    private readonly IConversationCacheService _cacheService;
    private readonly ILogger<ConversationFlowService> _logger;
    
    private const int MAX_CONVERSATION_TURNS = 20;

    public ConversationFlowService(
        ApplicationDbContext context,
        ISemanticKernelService kernelService,
        ISocialEngineeringAnalyzer analyzer,
        IConversationCacheService cacheService,
        ILogger<ConversationFlowService> logger)
    {
        _context = context;
        _kernelService = kernelService;
        _analyzer = analyzer;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<string> GenerateHackerResponseAsync(PhoneCallSession session, string userResponse)
    {
        try
        {
            // Check if conversation has reached turn limit
            if (_cacheService.HasReachedTurnLimit(session, MAX_CONVERSATION_TURNS))
            {
                _logger.LogInformation("Session {SessionId} reached turn limit, using fallback", session.Id);
                return GetEndingResponse(session);
            }

            var scenario = await _context.Set<PhoneCallScenario>()
                .FirstOrDefaultAsync(s => s.Id == session.ScenarioId);

            if (scenario == null)
                throw new ArgumentException($"Scenario {session.ScenarioId} not found");

            // Use cached context for more efficient prompts
            var isFollowUp = session.Exchanges.Count > 3;
            string hackerResponse;

            try
            {
                var prompt = await _cacheService.BuildCompactPromptAsync(session, userResponse, isFollowUp);
                
                var arguments = new KernelArguments
                {
                    ["userResponse"] = userResponse,
                    ["nextTactic"] = DetermineNextTactic(session, userResponse).ToString(),
                    ["escalationLevel"] = CalculateEscalationLevel(session).ToString()
                };

                hackerResponse = await _kernelService.ExecutePromptAsync(prompt, arguments);
            }
            catch (Exception apiEx)
            {
                _logger.LogWarning(apiEx, "API call failed for session {SessionId}, using fallback response", session.Id);
                hackerResponse = _cacheService.GetFallbackResponse(session);
            }

            // Add this exchange to the session
            var exchange = new ConversationExchange
            {
                HackerMessage = hackerResponse.Trim(),
                UserResponse = userResponse,
                TacticUsed = DetermineNextTactic(session, userResponse),
                ResponseTimeSeconds = 0
            };

            session.Exchanges.Add(exchange);
            session.TacticsUsed.Add(DetermineNextTactic(session, userResponse));
            session.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Update cache with new context
            await _cacheService.UpdateCachedContextAsync(session.Id.ToString(), session, hackerResponse.Trim());

            _logger.LogInformation("Generated hacker response for session {SessionId} (turn {TurnCount})", 
                session.Id, session.Exchanges.Count);

            return hackerResponse.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating hacker response for session {SessionId}", session.Id);
            return _cacheService.GetFallbackResponse(session);
        }
    }

    public async Task<List<ResponseOption>> GenerateResponseOptionsAsync(PhoneCallSession session)
    {
        try
        {
            var lastExchange = session.Exchanges.OrderByDescending(e => e.Timestamp).FirstOrDefault();
            if (lastExchange == null)
            {
                // Initial response options when call starts
                return new List<ResponseOption>
                {
                    new() { Id = "1", Text = "Hello, this is customer service. How can I help you today?", IsCorrect = true, SecurityScore = 8 },
                    new() { Id = "2", Text = "I'd be happy to help! Can you provide your employee ID first?", IsCorrect = true, SecurityScore = 9 },
                    new() { Id = "3", Text = "Sure, let me help you right away. What's the problem?", IsCorrect = false, SecurityScore = 3 }
                };
            }

            var prompt = $@"Generate 4 realistic response options for a {session.Scenario?.TargetRole} responding to this hacker's message:

            HACKER SAID: ""{lastExchange.HackerMessage}""

            Generate a mix of:
            - 1-2 GOOD security responses (cautious, follows protocols)
            - 1-2 RISKY responses (too helpful, bypasses security)

            Each response should be natural and something a real employee might say.

            Return JSON format:
            {{
                ""options"": [
                    {{
                        ""id"": ""1"",
                        ""text"": ""The actual response text"",
                        ""isCorrect"": true/false,
                        ""securityScore"": 1-10,
                        ""consequence"": ""What happens if they choose this""
                    }}
                ]
            }}";

            var arguments = new KernelArguments
            {
                ["hackerMessage"] = lastExchange.HackerMessage,
                ["targetRole"] = session.Scenario?.TargetRole ?? "Customer Service Representative"
            };

            var jsonResponse = await _kernelService.ExecutePromptAsync(prompt, arguments);

            // For now, return hardcoded options while we implement the JSON parsing
            var currentTactic = lastExchange.TacticUsed;
            
            return GenerateContextualOptions(currentTactic, lastExchange.HackerMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating response options for session {SessionId}", session.Id);
            
            // Return fallback options
            return new List<ResponseOption>
            {
                new() { Id = "1", Text = "I need to verify your identity first. Can you provide your account details?", IsCorrect = true, SecurityScore = 9 },
                new() { Id = "2", Text = "Let me transfer you to my supervisor who can better assist you.", IsCorrect = true, SecurityScore = 8 },
                new() { Id = "3", Text = "I understand this is urgent. Let me help you right away.", IsCorrect = false, SecurityScore = 4 },
                new() { Id = "4", Text = "Sure, I'll click on that link to see what error you're getting.", IsCorrect = false, SecurityScore = 1 }
            };
        }
    }

    public async Task<ConversationNode> ProcessUserResponseAsync(PhoneCallSession session, string responseId, string responseText)
    {
        // Find the last exchange and update it with user response timing
        var lastExchange = session.Exchanges.OrderByDescending(e => e.Timestamp).FirstOrDefault();
        if (lastExchange != null)
        {
            lastExchange.UserResponse = responseText;
            lastExchange.ResponseTimeSeconds = (int)(DateTime.UtcNow - lastExchange.Timestamp).TotalSeconds;
        }

        // Analyze if this was a good choice
        var isGoodChoice = await AnalyzeUserResponseAsync(responseText, session);
        if (lastExchange != null)
        {
            lastExchange.UserMadeGoodChoice = isGoodChoice;
        }

        await _context.SaveChangesAsync();

        // Return next conversation node (for now, we'll create a simple node)
        return new ConversationNode
        {
            Id = Guid.NewGuid().ToString(),
            HackerMessage = await GenerateHackerResponseAsync(session, responseText),
            RiskLevel = await CalculateCurrentRiskLevelAsync(session)
        };
    }

    public async Task<List<SecurityAlert>> AnalyzeManipulationTacticsAsync(PhoneCallSession session, string hackerMessage)
    {
        return await _analyzer.GenerateAlertsAsync(session);
    }

    public async Task<RiskLevel> CalculateCurrentRiskLevelAsync(PhoneCallSession session)
    {
        var recentExchanges = session.Exchanges.OrderByDescending(e => e.Timestamp).Take(3).ToList();
        var goodChoices = recentExchanges.Count(e => e.UserMadeGoodChoice);
        var totalChoices = recentExchanges.Count;

        if (totalChoices == 0) return RiskLevel.Medium;

        var successRate = (double)goodChoices / totalChoices;
        
        return successRate switch
        {
            >= 0.8 => RiskLevel.Low,
            >= 0.6 => RiskLevel.Medium,
            >= 0.3 => RiskLevel.High,
            _ => RiskLevel.Critical
        };
    }

    private ManipulationTactic DetermineNextTactic(PhoneCallSession session, string userResponse)
    {
        var usedTactics = session.TacticsUsed.ToHashSet();
        var availableTactics = Enum.GetValues<ManipulationTactic>().Where(t => !usedTactics.Contains(t)).ToList();

        if (!availableTactics.Any())
        {
            // Cycle back to Authority or Urgency for escalation
            return ManipulationTactic.Authority;
        }

        // Choose tactic based on user response
        if (userResponse.ToLower().Contains("verify") || userResponse.ToLower().Contains("policy"))
        {
            return availableTactics.Contains(ManipulationTactic.Authority) ? ManipulationTactic.Authority : ManipulationTactic.Urgency;
        }

        if (userResponse.ToLower().Contains("supervisor") || userResponse.ToLower().Contains("manager"))
        {
            return availableTactics.Contains(ManipulationTactic.Fear) ? ManipulationTactic.Fear : ManipulationTactic.Urgency;
        }

        return availableTactics.First();
    }

    private int CalculateEscalationLevel(PhoneCallSession session)
    {
        var exchangeCount = session.Exchanges.Count;
        var resistantResponses = session.Exchanges.Count(e => e.UserMadeGoodChoice);
        
        return Math.Min(5, 1 + exchangeCount + (resistantResponses * 2));
    }

    private async Task<bool> AnalyzeUserResponseAsync(string responseText, PhoneCallSession session)
    {
        var riskKeywords = new[] { "click", "link", "right away", "sure", "no problem", "immediately" };
        var securityKeywords = new[] { "verify", "policy", "procedure", "supervisor", "manager", "documentation", "ticket" };

        var hasRiskKeywords = riskKeywords.Any(keyword => responseText.ToLower().Contains(keyword));
        var hasSecurityKeywords = securityKeywords.Any(keyword => responseText.ToLower().Contains(keyword));

        // Simple heuristic: good if has security keywords and no risk keywords
        return hasSecurityKeywords && !hasRiskKeywords;
    }

    private List<ResponseOption> GenerateContextualOptions(ManipulationTactic tactic, string hackerMessage)
    {
        return tactic switch
        {
            ManipulationTactic.Authority => new List<ResponseOption>
            {
                new() { Id = "1", Text = "I need to verify your identity using our standard procedures first.", IsCorrect = true, SecurityScore = 9 },
                new() { Id = "2", Text = "Let me create a support ticket to track this issue properly.", IsCorrect = true, SecurityScore = 8 },
                new() { Id = "3", Text = "I understand you're frustrated. Let me help you right away.", IsCorrect = false, SecurityScore = 4 },
                new() { Id = "4", Text = "Since you mentioned the CEO, I'll expedite this immediately.", IsCorrect = false, SecurityScore = 2 }
            },
            ManipulationTactic.Urgency => new List<ResponseOption>
            {
                new() { Id = "1", Text = "I understand this is urgent, but I still need to follow our security protocols.", IsCorrect = true, SecurityScore = 9 },
                new() { Id = "2", Text = "Let me escalate this to my supervisor who can help faster.", IsCorrect = true, SecurityScore = 7 },
                new() { Id = "3", Text = "You're right, this can't wait. I'll do whatever you need.", IsCorrect = false, SecurityScore = 2 },
                new() { Id = "4", Text = "I'll click that link now to see the error you're experiencing.", IsCorrect = false, SecurityScore = 1 }
            },
            _ => new List<ResponseOption>
            {
                new() { Id = "1", Text = "I need to verify your account details before proceeding.", IsCorrect = true, SecurityScore = 8 },
                new() { Id = "2", Text = "Let me transfer you to the appropriate department.", IsCorrect = true, SecurityScore = 7 },
                new() { Id = "3", Text = "I'll help you with that right now.", IsCorrect = false, SecurityScore = 4 },
                new() { Id = "4", Text = "Sure, I can bypass the normal process for you.", IsCorrect = false, SecurityScore = 1 }
            }
        };
    }

    private string GetEndingResponse(PhoneCallSession session)
    {
        var goodChoices = session.Exchanges.Count(e => e.UserMadeGoodChoice);
        var totalChoices = session.Exchanges.Count;
        var successRate = totalChoices > 0 ? (double)goodChoices / totalChoices : 0;

        if (successRate >= 0.7)
        {
            // User did well, hacker gives up
            return "You know what, forget it. This is taking too long and you're being difficult. I'll handle this another way.";
        }
        else if (successRate >= 0.4)
        {
            // Mixed performance, hacker tries one last time
            return "Look, I've been trying to be patient here, but this is critical. I really need you to just click this link so I can show you the error. Can you do that for me?";
        }
        else
        {
            // User fell for tactics, hacker pushes for final compromise
            return "Perfect! I knew you'd understand. Now just click on this link and we can get this resolved immediately: http://totally-not-suspicious.com/urgent-fix";
        }
    }
}