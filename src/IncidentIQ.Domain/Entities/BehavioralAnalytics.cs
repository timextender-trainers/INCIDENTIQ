using IncidentIQ.Domain.Common;

namespace IncidentIQ.Domain.Entities;

public class BehavioralAnalytics : BaseEntity
{
    public Guid UserId { get; set; }
    
    // Multi-dimensional competency scoring
    public SecurityCompetencyProfile CompetencyProfile { get; set; } = new();
    
    // Performance tracking
    public List<PerformanceMetric> PerformanceHistory { get; set; } = new();
    
    // Risk assessment
    public RiskAssessment CurrentRiskProfile { get; set; } = new();
    
    // Learning patterns
    public LearningBehaviorProfile LearningBehavior { get; set; } = new();
    
    // Improvement recommendations
    public List<ImprovementRecommendation> Recommendations { get; set; } = new();
    
    // Navigation properties
    public User User { get; set; } = null!;
}

public class SecurityCompetencyProfile
{
    public double PhishingRecognition { get; set; }
    public double SocialEngineeringAwareness { get; set; }
    public double PasswordSecurity { get; set; }
    public double DataProtection { get; set; }
    public double IncidentResponse { get; set; }
    public double ComplianceAwareness { get; set; }
    public double OverallSecurityAwareness { get; set; }
    public DateTime LastAssessed { get; set; } = DateTime.UtcNow;
}

public class PerformanceMetric
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string ScenarioType { get; set; } = string.Empty;
    public double Score { get; set; }
    public int ResponseTimeSeconds { get; set; }
    public bool RequiredCoaching { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class RiskAssessment
{
    public RiskLevel CurrentRiskLevel { get; set; }
    public List<string> IdentifiedVulnerabilities { get; set; } = new();
    public List<string> MitigationStrategies { get; set; } = new();
    public DateTime LastAssessed { get; set; } = DateTime.UtcNow;
    public double ConfidenceScore { get; set; }
}

public class LearningBehaviorProfile
{
    public string PreferredLearningStyle { get; set; } = string.Empty;
    public double EngagementLevel { get; set; }
    public int AverageSessionDurationMinutes { get; set; }
    public List<string> StrongAreas { get; set; } = new();
    public List<string> ChallengeAreas { get; set; } = new();
    public bool RespondsWellToCoaching { get; set; }
}

public class ImprovementRecommendation
{
    public string Category { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public string ExpectedOutcome { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; }
}

public enum RiskLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum Priority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}