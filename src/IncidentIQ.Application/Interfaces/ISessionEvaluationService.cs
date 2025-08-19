using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Application.Interfaces;

public interface ISessionEvaluationService
{
    Task<SessionEvaluationResult> EvaluateSessionAsync(PhoneCallSession session);
    Task<List<SecurityBreach>> AnalyzeSecurityBreachesAsync(List<ConversationExchange> exchanges);
    Task<List<SecurityRecommendation>> GenerateDetailedRecommendationsAsync(SessionEvaluationResult evaluation);
}

public class SessionEvaluationResult
{
    public Guid SessionId { get; set; }
    public double SecurityScore { get; set; }
    public double OverallPerformance { get; set; }
    public List<SecurityBreach> SecurityBreaches { get; set; } = new();
    public List<TacticSuccessAnalysis> TacticAnalysis { get; set; } = new();
    public List<SecurityRecommendation> Recommendations { get; set; } = new();
    public string SummaryFeedback { get; set; } = "";
    public RiskAssessment RiskAssessment { get; set; } = new();
    public List<string> KeyStrengths { get; set; } = new();
    public List<string> GrowthAreas { get; set; } = new();
    public List<FutureLearning> FutureLearnings { get; set; } = new();
    public TrainingMetrics Metrics { get; set; } = new();
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
}

public class SecurityBreach
{
    public string BreachType { get; set; } = "";
    public string Description { get; set; } = "";
    public string UserResponse { get; set; } = "";
    public int TurnNumber { get; set; }
    public RiskLevel Severity { get; set; }
    public string ImpactExplanation { get; set; } = "";
    public string PreventionAdvice { get; set; } = "";
}

public class TacticSuccessAnalysis
{
    public ManipulationTactic Tactic { get; set; }
    public bool WasSuccessful { get; set; }
    public string HowItWorked { get; set; } = "";
    public string UserVulnerability { get; set; } = "";
    public string CounterStrategy { get; set; } = "";
}

public class SecurityRecommendation
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string ActionableAdvice { get; set; } = "";
    public RecommendationPriority Priority { get; set; }
    public string RoleSpecificContext { get; set; } = "";
}

public class RiskAssessment
{
    public RiskLevel OverallRiskLevel { get; set; }
    public List<string> PrimaryVulnerabilities { get; set; } = new();
    public double PhishingResistanceScore { get; set; }
    public string RiskProfile { get; set; } = "";
}

public enum RecommendationPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public class FutureLearning
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string EstimatedTime { get; set; } = "";
    public string ResourceType { get; set; } = "";
    public string Priority { get; set; } = "";
}

public class TrainingMetrics
{
    public TimeSpan CompletionTime { get; set; }
    public int ThreatsDetected { get; set; }
    public int TotalExchanges { get; set; }
    public double ResponseTime { get; set; }
    public string Grade { get; set; } = "";
}