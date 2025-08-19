using IncidentIQ.Application.Interfaces;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Application.Interfaces.AI.Agents;

public interface ICoachingAgent
{
    Task<CoachingResponse> ProvideGuidanceAsync(CoachingRequest request);
    Task<string> GenerateHintAsync(DecisionPoint decisionPoint, User user, string userContext);
    Task<string> GenerateEncouragementAsync(User user, SessionScoring currentScore);
    Task<string> ExplainConsequenceAsync(UserResponse response, DecisionPoint decisionPoint);
    Task<SessionEvaluationResult> AnalyzeConversationSessionAsync(PhoneCallSession session);
}

public class CoachingRequest
{
    public User User { get; set; } = null!;
    public TrainingSession Session { get; set; } = null!;
    public DecisionPoint CurrentDecisionPoint { get; set; } = null!;
    public UserResponse? LastResponse { get; set; }
    public CoachingType RequestedType { get; set; }
    public string Context { get; set; } = string.Empty;
}

public class CoachingResponse
{
    public string Message { get; set; } = string.Empty;
    public CoachingType Type { get; set; }
    public string Context { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}