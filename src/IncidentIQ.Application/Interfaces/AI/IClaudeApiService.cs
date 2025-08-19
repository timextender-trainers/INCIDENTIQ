using IncidentIQ.Domain.Entities;

namespace IncidentIQ.Application.Interfaces.AI;

public interface IClaudeApiService
{
    Task<string> GeneratePhishingResponseAsync(PhoneCallSession session, string userMessage, List<string> conversationHistory);
    Task<string> GenerateFeedbackAsync(List<ConversationExchange> conversationHistory, string clickedLink);
    Task<bool> IsApiAvailableAsync();
}