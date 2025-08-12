using IncidentIQ.Domain.Entities;

namespace IncidentIQ.Application.Interfaces.AI.Agents;

public interface IBehaviorAnalystAgent
{
    Task<BehavioralAnalytics> AnalyzeUserBehaviorAsync(User user, List<TrainingSession> sessions);
    Task<SecurityCompetencyProfile> AssessSecurityCompetencyAsync(User user, List<TrainingSession> recentSessions);
    Task<RiskAssessment> PerformRiskAssessmentAsync(User user, BehavioralAnalytics analytics);
    Task<List<ImprovementRecommendation>> GenerateRecommendationsAsync(BehavioralAnalytics analytics);
    Task<LearningBehaviorProfile> AnalyzeLearningPatternsAsync(User user, List<TrainingSession> sessions);
}

public class BehaviorAnalysisRequest
{
    public User User { get; set; } = null!;
    public List<TrainingSession> Sessions { get; set; } = new();
    public BehavioralAnalytics? ExistingAnalytics { get; set; }
    public DateTime AnalysisWindowStart { get; set; }
    public DateTime AnalysisWindowEnd { get; set; }
}