using IncidentIQ.Domain.Common;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Domain.Entities;

public class TrainingScenario : BaseEntity
{
    public ScenarioType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public SecurityLevel TargetSecurityLevel { get; set; }
    public UserRole TargetRole { get; set; }
    
    // AI-generated content
    public ScenarioContent Content { get; set; } = new();
    
    // Personalization metadata
    public PersonalizationData PersonalizationContext { get; set; } = new();
    
    // Scenario configuration
    public ScenarioConfiguration Configuration { get; set; } = new();
    
    // Navigation properties
    public ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
    public ICollection<AgentInteraction> AgentInteractions { get; set; } = new List<AgentInteraction>();
}

public class ScenarioContent
{
    public string PrimaryContent { get; set; } = string.Empty; // Email, call script, etc.
    public Dictionary<string, object> InteractiveElements { get; set; } = new();
    public List<DecisionPoint> DecisionPoints { get; set; } = new();
    public Dictionary<string, string> Consequences { get; set; } = new();
    public List<string> LearningObjectives { get; set; } = new();
}

public class PersonalizationData
{
    public string CompanyName { get; set; } = string.Empty;
    public List<string> ColleagueNames { get; set; } = new();
    public List<string> RelevantSystems { get; set; } = new();
    public Dictionary<string, object> RoleSpecificContext { get; set; } = new();
    public Dictionary<string, object> CompanySpecificDetails { get; set; } = new();
}

public class ScenarioConfiguration
{
    public int EstimatedDurationMinutes { get; set; }
    public bool AllowsMultipleAttempts { get; set; } = true;
    public int MaxAttempts { get; set; } = 3;
    public Dictionary<string, object> ScoringCriteria { get; set; } = new();
    public bool RequiresRealTimeCoaching { get; set; } = true;
}

public class DecisionPoint
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Question { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public int CorrectOptionIndex { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public string CoachingTip { get; set; } = string.Empty;
}