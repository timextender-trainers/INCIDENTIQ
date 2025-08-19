using IncidentIQ.Application.Interfaces;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IncidentIQ.Infrastructure.Services;

public class ConversationCacheService : IConversationCacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<ConversationCacheService> _logger;
    
    private readonly string[] _fallbackResponses = new[]
    {
        "I apologize, but I'm experiencing technical difficulties. Let me transfer you to my supervisor.",
        "I need to verify this request through our standard procedures. Please hold while I check.",
        "I'm having trouble accessing the system right now. Can I call you back in a few minutes?",
        "For security reasons, I need to escalate this to my manager who can better assist you."
    };

    public ConversationCacheService(IMemoryCache cache, ILogger<ConversationCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<CachedConversationContext?> GetCachedContextAsync(string sessionId)
    {
        var cacheKey = $"conversation_context_{sessionId}";
        return _cache.Get<CachedConversationContext>(cacheKey);
    }

    public async Task UpdateCachedContextAsync(string sessionId, PhoneCallSession session, string lastResponse)
    {
        var context = new CachedConversationContext
        {
            SessionId = sessionId,
            CompactHistory = BuildCompactHistory(session),
            CurrentObjective = DetermineCurrentObjective(session),
            UsedTactics = session.TacticsUsed.Select(t => t.ToString()).ToList(),
            LastUpdated = DateTime.UtcNow,
            ExchangeCount = session.Exchanges.Count,
            LastUserResponse = session.Exchanges.LastOrDefault()?.UserResponse ?? "",
            LastHackerResponse = lastResponse
        };

        var cacheKey = $"conversation_context_{sessionId}";
        _cache.Set(cacheKey, context, TimeSpan.FromMinutes(30));
        
        _logger.LogDebug("Updated cached context for session {SessionId} with {ExchangeCount} exchanges", 
            sessionId, context.ExchangeCount);
    }

    public async Task<string> BuildCompactPromptAsync(PhoneCallSession session, string userResponse, bool isFollowUp = false)
    {
        var cached = await GetCachedContextAsync(session.Id.ToString());
        
        if (isFollowUp && cached != null)
        {
            // Use shorter prompt for follow-up messages
            return BuildFollowUpPrompt(cached, userResponse);
        }
        
        // Full prompt for initial or non-cached requests
        return BuildFullPrompt(session, userResponse);
    }

    public bool HasReachedTurnLimit(PhoneCallSession session, int maxTurns = 20)
    {
        return session.Exchanges.Count >= maxTurns;
    }

    public string GetFallbackResponse(PhoneCallSession session)
    {
        var random = new Random();
        var baseResponse = _fallbackResponses[random.Next(_fallbackResponses.Length)];
        
        // Add some variety based on session state
        if (session.Exchanges.Count > 10)
        {
            return "I've been trying to help, but this seems like a complex issue. Let me get my supervisor to ensure we handle this properly.";
        }
        
        if (session.TacticsUsed.Contains(ManipulationTactic.Urgency))
        {
            return "I understand this is urgent, but I need to follow proper procedures. Let me escalate this appropriately.";
        }
        
        return baseResponse;
    }

    private string BuildCompactHistory(PhoneCallSession session)
    {
        // Keep only last 3 exchanges for context, summarize earlier ones
        var recentExchanges = session.Exchanges.OrderByDescending(e => e.Timestamp).Take(3).ToList();
        var earlierCount = session.Exchanges.Count - recentExchanges.Count;
        
        var history = "";
        if (earlierCount > 0)
        {
            history += $"[Earlier: {earlierCount} exchanges where user ";
            var goodChoices = session.Exchanges.Take(earlierCount).Count(e => e.UserMadeGoodChoice);
            history += goodChoices > earlierCount / 2 ? "mostly followed security protocols" : "was somewhat compliant";
            history += "]\n";
        }
        
        foreach (var exchange in recentExchanges.OrderBy(e => e.Timestamp))
        {
            history += $"H: {exchange.HackerMessage}\n";
            if (!string.IsNullOrEmpty(exchange.UserResponse))
            {
                history += $"U: {exchange.UserResponse}\n";
            }
        }
        
        return history.Trim();
    }

    private string DetermineCurrentObjective(PhoneCallSession session)
    {
        var exchangeCount = session.Exchanges.Count;
        var recentGoodChoices = session.Exchanges.TakeLast(3).Count(e => e.UserMadeGoodChoice);
        
        return exchangeCount switch
        {
            <= 3 => "Establish trust and create urgency",
            <= 7 => recentGoodChoices >= 2 ? "Escalate pressure and authority" : "Get them to click link or provide access",
            _ => "Final push for compromise or tactical retreat"
        };
    }

    private string BuildFollowUpPrompt(CachedConversationContext cached, string userResponse)
    {
        return $"""
        Continue as frustrated customer. Last exchange count: {cached.ExchangeCount}
        Objective: {cached.CurrentObjective}
        Used tactics: {string.Join(", ", cached.UsedTactics)}
        
        Recent context:
        {cached.CompactHistory}
        
        User just said: "{userResponse}"
        
        Respond naturally as angry customer (max 100 words). Escalate if user resists.
        """;
    }

    private string BuildFullPrompt(PhoneCallSession session, string userResponse)
    {
        var scenario = session.Scenario;
        if (scenario == null) 
            return BuildFollowUpPrompt(new CachedConversationContext { ExchangeCount = session.Exchanges.Count }, userResponse);
            
        var conversationHistory = BuildCompactHistory(session);
        var usedTactics = string.Join(", ", session.TacticsUsed.Select(t => t.ToString()));
        
        return $"""
        You are {scenario.CallerProfile.Name}, angry customer from {scenario.CallerProfile.Company} 
        calling {scenario.TargetRole} at {scenario.TargetCompany}.
        
        Goal: Get them to click suspicious link or provide access
        Persona: {scenario.CallerProfile.Persona}
        
        Recent conversation:
        {conversationHistory}
        
        User said: "{userResponse}"
        Used tactics: {usedTactics}
        
        Respond as frustrated customer (max 150 words). Use manipulation tactics naturally.
        """;
    }
}