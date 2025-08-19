using IncidentIQ.Domain.Common;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Domain.Entities;

public class PhoneCallScenario : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ScenarioType Type { get; set; } = ScenarioType.CustomerServiceCall;
    public DifficultyLevel Difficulty { get; set; }
    
    // Phone call specific properties
    public CallerProfile CallerProfile { get; set; } = new();
    public ConversationFlow ConversationFlow { get; set; } = new();
    public List<ManipulationTactic> PlannedTactics { get; set; } = new();
    public List<string> LearningObjectives { get; set; } = new();
    
    // Personalization context
    public string TargetCompany { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public Dictionary<string, object> CompanyContext { get; set; } = new();
    
    // Navigation properties
    public ICollection<PhoneCallSession> Sessions { get; set; } = new List<PhoneCallSession>();
}

public class CallerProfile
{
    public string Name { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Persona { get; set; } = string.Empty;
    public Dictionary<string, object> Background { get; set; } = new();
}

public class ConversationFlow
{
    public ConversationNode InitialNode { get; set; } = new();
    public List<ConversationNode> Nodes { get; set; } = new();
    public Dictionary<string, string> SuccessConditions { get; set; } = new();
    public Dictionary<string, string> FailureConsequences { get; set; } = new();
}

public class ConversationNode
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string HackerMessage { get; set; } = string.Empty;
    public List<ResponseOption> UserOptions { get; set; } = new();
    public ManipulationTactic PrimaryTactic { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public string NextNodeId { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ResponseOption
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string NextNodeId { get; set; } = string.Empty;
    public string Consequence { get; set; } = string.Empty;
    public int SecurityScore { get; set; }
}

public class PhoneCallSession : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ScenarioId { get; set; }
    public CallState CallState { get; set; } = CallState.Incoming;
    
    public DateTime? CallStartedAt { get; set; }
    public DateTime? CallEndedAt { get; set; }
    public int CallDurationSeconds { get; set; }
    
    // Conversation tracking
    public List<ConversationExchange> Exchanges { get; set; } = new();
    public string CurrentNodeId { get; set; } = string.Empty;
    public List<ManipulationTactic> TacticsUsed { get; set; } = new();
    public List<SecurityAlert> AlertsTriggered { get; set; } = new();
    
    // Scoring
    public PhoneCallScoring Scoring { get; set; } = new();
    
    // Navigation properties
    public User User { get; set; } = null!;
    public PhoneCallScenario Scenario { get; set; } = null!;
}

public class ConversationExchange
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string HackerMessage { get; set; } = string.Empty;
    public string UserResponse { get; set; } = string.Empty;
    public ManipulationTactic TacticUsed { get; set; }
    public bool UserMadeGoodChoice { get; set; }
    public int ResponseTimeSeconds { get; set; }
    public Dictionary<string, object> ContextData { get; set; } = new();
}

public class SecurityAlert
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ManipulationTactic TacticDetected { get; set; }
    public RiskLevel Level { get; set; }
    public string Icon { get; set; } = string.Empty;
    public bool WasAcknowledged { get; set; }
}

public class PhoneCallScoring
{
    public double OverallScore { get; set; }
    public int CorrectResponses { get; set; }
    public int TotalResponses { get; set; }
    public Dictionary<ManipulationTactic, double> TacticResistance { get; set; } = new();
    public List<string> StrengthAreas { get; set; } = new();
    public List<string> VulnerabilityAreas { get; set; } = new();
    public double AverageResponseTime { get; set; }
    public string FeedbackSummary { get; set; } = string.Empty;
}