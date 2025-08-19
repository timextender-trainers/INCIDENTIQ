using IncidentIQ.Application.Models;

namespace IncidentIQ.Application.Interfaces
{
    public interface IUserProfileService
    {
        // Profile Management
        Task<UserProfile?> GetUserProfileAsync(string userId);
        Task<UserProfile> CreateUserProfileAsync(OnboardingFormData formData, string userId);
        Task<UserProfile> UpdateUserProfileAsync(UserProfile profile);
        Task<bool> DeleteUserProfileAsync(string userId);

        // Onboarding Support
        Task<bool> IsOnboardingCompleteAsync(string userId);
        Task<OnboardingProgress> GetOnboardingProgressAsync(string userId);
        Task SaveOnboardingStepAsync(string userId, int step, object stepData);

        // AI Personalization
        Task<PersonalizationProfile> GeneratePersonalizationAsync(UserProfile profile);
        Task<ScenarioParameters> GetScenarioParametersAsync(string userId);
        Task<CoachingSettings> GetCoachingSettingsAsync(string userId);
        Task<List<string>> GetRecommendedTrainingAsync(string userId);

        // Progress Tracking
        Task UpdateProgressAsync(string userId, TrainingSession session);
        Task<AnalyticsData> CalculateAnalyticsAsync(string userId);
        Task<ComparisonMetrics> GetComparisonMetricsAsync(string userId);

        // Risk Assessment
        Task<RiskProfile> AssessUserRiskAsync(UserProfile profile);
        Task<List<string>> GetPersonalizedThreatsAsync(string userId);
        Task<double> CalculateSecurityScoreAsync(string userId);

        // Learning Path Management
        Task<List<string>> GetLearningPathAsync(string userId);
        Task<string> GetNextRecommendedTrainingAsync(string userId);
        Task UpdateLearningProgressAsync(string userId, string trainingType, double score);

        // Scenario Generation Support
        Task<Dictionary<string, object>> GetScenarioContextAsync(string userId);
        Task<List<string>> GeneratePersonalizedEmailsAsync(string userId, string scenarioType);
        Task<string> GenerateCoachingTipAsync(string userId, string context);
    }

    public class OnboardingProgress
    {
        public int CurrentStep { get; set; } = 1;
        public bool IsComplete { get; set; } = false;
        public Dictionary<string, object> StepData { get; set; } = new();
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}