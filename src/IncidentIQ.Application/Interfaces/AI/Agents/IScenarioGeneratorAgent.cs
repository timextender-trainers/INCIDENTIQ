using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Application.Interfaces.AI.Agents;

public interface IScenarioGeneratorAgent
{
    Task<TrainingScenario> GenerateScenarioAsync(ScenarioGenerationRequest request);
    Task<ScenarioContent> GenerateContentAsync(TrainingScenario scenario, User user);
    Task<List<DecisionPoint>> GenerateDecisionPointsAsync(ScenarioContent content, DifficultyLevel difficulty);
}

public class ScenarioGenerationRequest
{
    public ScenarioType Type { get; set; }
    public UserRole TargetRole { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public SecurityLevel TargetSecurityLevel { get; set; }
    public User User { get; set; } = null!;
    public Dictionary<string, object> AdditionalContext { get; set; } = new();
}