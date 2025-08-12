using IncidentIQ.Domain.Common;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Domain.Entities;

public class AgentInteraction : BaseEntity
{
    public Guid? UserId { get; set; }
    public Guid? TrainingSessionId { get; set; }
    public Guid? TrainingScenarioId { get; set; }
    
    public AgentType AgentType { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public InteractionType InteractionType { get; set; }
    
    // Interaction content
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public Dictionary<string, object> Context { get; set; } = new();
    
    // Performance metrics
    public int ProcessingTimeMs { get; set; }
    public double ConfidenceScore { get; set; }
    public bool WasSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
    
    // Cost tracking
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public decimal EstimatedCost { get; set; }
    
    // Navigation properties
    public User? User { get; set; }
    public TrainingSession? TrainingSession { get; set; }
    public TrainingScenario? TrainingScenario { get; set; }
}