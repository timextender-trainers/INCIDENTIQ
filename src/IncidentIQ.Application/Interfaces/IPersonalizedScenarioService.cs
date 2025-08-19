using IncidentIQ.Application.Models;

namespace IncidentIQ.Application.Interfaces
{
    public interface IPersonalizedScenarioService
    {
        // Scenario Generation
        Task<List<PersonalizedRecommendation>> GeneratePersonalizedRecommendationsAsync(string userId);
        Task<PersonalizedRecommendation> GenerateScenarioForRoleAsync(string userId, string roleId, string scenarioType);
        Task<TrainingScenario> CreatePersonalizedScenarioAsync(string userId, ScenarioTemplate template);
        
        // Template Management
        Task<List<ScenarioTemplate>> GetTemplatesForRoleAsync(string roleId);
        Task<List<ScenarioTemplate>> GetTemplatesForIndustryAsync(string industry);
        Task<ScenarioTemplate?> GetTemplateAsync(string templateId);
        
        // Personalization
        Task<Dictionary<string, string>> BuildPersonalizationContextAsync(string userId);
        Task<string> PersonalizeContentAsync(string content, Dictionary<string, string> context);
        Task<List<string>> GenerateCompanySpecificExamplesAsync(string userId, string scenarioType);
        
        // Difficulty & Progression
        Task<string> DetermineOptimalDifficultyAsync(string userId, string scenarioType);
        Task<List<PersonalizedRecommendation>> GetProgressiveRecommendationsAsync(string userId);
        Task<List<PersonalizedRecommendation>> GetReinforcementScenariosAsync(string userId);
        
        // Industry-Specific Content
        Task<List<string>> GetIndustryThreatsAsync(string industry);
        Task<List<string>> GetRoleSpecificToolsAsync(string roleId);
        Task<Dictionary<string, string>> GetIndustryTerminologyAsync(string industry);
        
        // Real-time Adaptation
        Task UpdateScenarioPerformanceAsync(string userId, string scenarioId, double score, TimeSpan completionTime);
        Task<List<PersonalizedRecommendation>> RefreshRecommendationsAsync(string userId);
        Task<PersonalizedRecommendation> AdaptScenarioDifficultyAsync(string userId, PersonalizedRecommendation recommendation);
        
        // Analytics & Insights
        Task<ScenarioAnalytics> GetScenarioAnalyticsAsync(string userId);
        Task<List<string>> GetPersonalizedLearningInsightsAsync(string userId);
        Task<Dictionary<string, double>> CalculateRoleProficiencyAsync(string userId);
    }

    public class ScenarioAnalytics
    {
        public string UserId { get; set; } = "";
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
        
        // Overall Performance
        public double OverallScore { get; set; } = 0.0;
        public int TotalScenariosCompleted { get; set; } = 0;
        public TimeSpan AverageCompletionTime { get; set; } = TimeSpan.Zero;
        
        // Role-Specific Performance
        public Dictionary<string, double> ScenarioTypeScores { get; set; } = new();
        public Dictionary<string, int> ScenarioTypeCompletions { get; set; } = new();
        
        // Improvement Areas
        public List<string> StrengthAreas { get; set; } = new();
        public List<string> ImprovementAreas { get; set; } = new();
        public List<string> RecommendedFocus { get; set; } = new();
        
        // Personalization Effectiveness
        public double PersonalizationScore { get; set; } = 0.0;
        public Dictionary<string, double> ContextualRelevanceScores { get; set; } = new();
        
        // Learning Velocity
        public double LearningVelocity { get; set; } = 0.0;
        public List<DateTime> ScenarioCompletionTimeline { get; set; } = new();
        public Dictionary<string, double> DifficultyProgressionRates { get; set; } = new();
    }
}