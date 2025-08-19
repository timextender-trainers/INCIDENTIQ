using IncidentIQ.Domain.Common;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Domain.Entities;

public class TrainingSession : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid TrainingScenarioId { get; set; }
    
    public SessionStatus Status { get; set; } = SessionStatus.NotStarted;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int DurationSeconds { get; set; }
    
    // User responses and interactions
    public List<UserResponse> Responses { get; set; } = new();
    public SessionScoring Scoring { get; set; } = new();
    
    // AI coaching data
    public List<CoachingInteraction> CoachingInteractions { get; set; } = new();
    
    // Navigation properties
    public User User { get; set; } = null!;
    public TrainingScenario TrainingScenario { get; set; } = null!;
    public ICollection<AgentInteraction> AgentInteractions { get; set; } = new List<AgentInteraction>();
}

public class UserResponse
{
    public string DecisionPointId { get; set; } = string.Empty;
    public string SelectedOption { get; set; } = string.Empty;
    public int ResponseTimeSeconds { get; set; }
    public DateTime RespondedAt { get; set; } = DateTime.UtcNow;
    public bool IsCorrect { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public class SessionScoring
{
    public double OverallScore { get; set; }
    public Dictionary<string, double> CategoryScores { get; set; } = new();
    public int CorrectResponses { get; set; }
    public int TotalResponses { get; set; }
    public List<string> StrengthAreas { get; set; } = new();
    public List<string> ImprovementAreas { get; set; } = new();
    public string FeedbackSummary { get; set; } = string.Empty;
}

public class CoachingInteraction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string CoachingMessage { get; set; } = string.Empty;
    public string TriggerContext { get; set; } = string.Empty;
    public CoachingType Type { get; set; }
    public bool WasHelpful { get; set; } // User feedback
}