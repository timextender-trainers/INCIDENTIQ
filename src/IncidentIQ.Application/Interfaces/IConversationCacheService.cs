using IncidentIQ.Domain.Entities;

namespace IncidentIQ.Application.Interfaces;

public interface IConversationCacheService
{
    /// <summary>
    /// Gets cached conversation context to reduce API prompt sizes
    /// </summary>
    Task<CachedConversationContext?> GetCachedContextAsync(string sessionId);
    
    /// <summary>
    /// Updates cached conversation context after each exchange
    /// </summary>
    Task UpdateCachedContextAsync(string sessionId, PhoneCallSession session, string lastResponse);
    
    /// <summary>
    /// Builds a compact conversation summary for API calls
    /// </summary>
    Task<string> BuildCompactPromptAsync(PhoneCallSession session, string userResponse, bool isFollowUp = false);
    
    /// <summary>
    /// Checks if session has reached turn limit
    /// </summary>
    bool HasReachedTurnLimit(PhoneCallSession session, int maxTurns = 20);
    
    /// <summary>
    /// Gets fallback response for API failures or when over limits
    /// </summary>
    string GetFallbackResponse(PhoneCallSession session);
}

public class CachedConversationContext
{
    public string SessionId { get; set; } = string.Empty;
    public string CompactHistory { get; set; } = string.Empty;
    public string CurrentObjective { get; set; } = string.Empty;
    public List<string> UsedTactics { get; set; } = new();
    public DateTime LastUpdated { get; set; }
    public int ExchangeCount { get; set; }
    public string LastUserResponse { get; set; } = string.Empty;
    public string LastHackerResponse { get; set; } = string.Empty;
}