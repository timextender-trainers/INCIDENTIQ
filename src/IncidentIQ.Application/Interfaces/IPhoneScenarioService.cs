using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Application.Interfaces;

public interface IPhoneScenarioService
{
    Task<PhoneCallScenario> CreateScenarioAsync(string userId, string targetRole, string targetCompany);
    Task<PhoneCallSession> StartSessionAsync(string userId, Guid scenarioId);
    Task<PhoneCallSession> GetActiveSessionAsync(string userId);
    Task<PhoneCallSession?> GetSessionByIdAsync(Guid sessionId);
    Task<PhoneCallSession> UpdateSessionAsync(Guid sessionId, CallState newState);
    Task EndSessionAsync(Guid sessionId);
    Task<List<PhoneCallScenario>> GetScenariosForRoleAsync(string role);
}

public interface IConversationFlowService
{
    Task<string> GenerateHackerResponseAsync(PhoneCallSession session, string userResponse);
    Task<List<ResponseOption>> GenerateResponseOptionsAsync(PhoneCallSession session);
    Task<ConversationNode> ProcessUserResponseAsync(PhoneCallSession session, string responseId, string responseText);
    Task<List<SecurityAlert>> AnalyzeManipulationTacticsAsync(PhoneCallSession session, string hackerMessage);
    Task<RiskLevel> CalculateCurrentRiskLevelAsync(PhoneCallSession session);
}

public interface ISocialEngineeringAnalyzer
{
    Task<ManipulationTactic> DetectManipulationTacticAsync(string message);
    Task<List<SecurityAlert>> GenerateAlertsAsync(PhoneCallSession session);
    Task<double> CalculateTacticResistanceScore(List<ConversationExchange> exchanges, ManipulationTactic tactic);
    Task<string> GenerateCoachingTipAsync(ManipulationTactic tactic, string context);
}