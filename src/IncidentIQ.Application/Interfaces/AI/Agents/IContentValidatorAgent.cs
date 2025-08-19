using IncidentIQ.Domain.Entities;

namespace IncidentIQ.Application.Interfaces.AI.Agents;

public interface IContentValidatorAgent
{
    Task<ContentValidationResult> ValidateScenarioContentAsync(ScenarioContent content);
    Task<ContentValidationResult> ValidateCoachingMessageAsync(string message);
    Task<bool> IsContentAppropriateAsync(string content, string context);
    Task<List<string>> SuggestImprovementsAsync(ScenarioContent content);
}

public class ContentValidationResult
{
    public bool IsValid { get; set; }
    public bool IsAppropriate { get; set; }
    public bool IsSafe { get; set; }
    public double QualityScore { get; set; }
    public List<string> Issues { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}