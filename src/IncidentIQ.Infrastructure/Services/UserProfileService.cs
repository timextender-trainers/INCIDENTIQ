using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using IncidentIQ.Application.Interfaces;
using IncidentIQ.Application.Models;
using IncidentIQ.Infrastructure.Data;
using IncidentIQ.Application.Interfaces.AI;

namespace IncidentIQ.Infrastructure.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<UserProfileService> _logger;
        private readonly ISemanticKernelService _semanticKernel;

        private const string PROFILE_CACHE_KEY = "user_profile_{0}";
        private const string ONBOARDING_CACHE_KEY = "onboarding_{0}";
        private const int CACHE_EXPIRATION_MINUTES = 60;

        public UserProfileService(
            ApplicationDbContext context, 
            IDistributedCache cache, 
            ILogger<UserProfileService> logger,
            ISemanticKernelService semanticKernel)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
            _semanticKernel = semanticKernel;
        }

        #region Profile Management

        public async Task<UserProfile?> GetUserProfileAsync(string userId)
        {
            try
            {
                var cacheKey = string.Format(PROFILE_CACHE_KEY, userId);
                var cachedProfile = await _cache.GetStringAsync(cacheKey);
                
                if (!string.IsNullOrEmpty(cachedProfile))
                {
                    return JsonSerializer.Deserialize<UserProfile>(cachedProfile);
                }

                // Query from database (simulated - in real implementation, you'd have a UserProfiles table)
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null) return null;

                // For now, create a basic profile from user data
                // In production, you'd have a dedicated UserProfiles table
                var profile = new UserProfile
                {
                    UserId = userId,
                    Email = user.Email ?? "",
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    // Additional fields would be populated from dedicated profile table
                };

                await CacheProfileAsync(profile);
                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile for user {UserId}", userId);
                return null;
            }
        }

        public async Task<UserProfile> CreateUserProfileAsync(OnboardingFormData formData, string userId)
        {
            try
            {
                var profile = formData.ToUserProfile(userId);
                
                // Generate AI personalization
                profile.Personalization = await GeneratePersonalizationAsync(profile);
                
                // Initialize analytics
                profile.Progress.Analytics = await CalculateAnalyticsAsync(userId);
                
                // In production, you'd save to a UserProfiles table
                // For now, we'll cache it and update the user record
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    user.FirstName = profile.FirstName;
                    user.LastName = profile.LastName;
                    user.CompletedOnboarding = true;
                    await _context.SaveChangesAsync();
                }

                await CacheProfileAsync(profile);
                
                _logger.LogInformation("Created user profile for user {UserId}", userId);
                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user profile for user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserProfile> UpdateUserProfileAsync(UserProfile profile)
        {
            try
            {
                profile.UpdatedAt = DateTime.UtcNow;
                
                // Update cache
                await CacheProfileAsync(profile);
                
                // In production, update database table
                _logger.LogInformation("Updated user profile for user {UserId}", profile.UserId);
                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for user {UserId}", profile.UserId);
                throw;
            }
        }

        public async Task<bool> DeleteUserProfileAsync(string userId)
        {
            try
            {
                var cacheKey = string.Format(PROFILE_CACHE_KEY, userId);
                await _cache.RemoveAsync(cacheKey);
                
                // In production, delete from database
                _logger.LogInformation("Deleted user profile for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user profile for user {UserId}", userId);
                return false;
            }
        }

        #endregion

        #region Onboarding Support

        public async Task<bool> IsOnboardingCompleteAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                return user?.CompletedOnboarding ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking onboarding status for user {UserId}", userId);
                return false;
            }
        }

        public async Task<OnboardingProgress> GetOnboardingProgressAsync(string userId)
        {
            try
            {
                var cacheKey = string.Format(ONBOARDING_CACHE_KEY, userId);
                var cachedProgress = await _cache.GetStringAsync(cacheKey);
                
                if (!string.IsNullOrEmpty(cachedProgress))
                {
                    return JsonSerializer.Deserialize<OnboardingProgress>(cachedProgress) ?? new OnboardingProgress();
                }

                return new OnboardingProgress();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving onboarding progress for user {UserId}", userId);
                return new OnboardingProgress();
            }
        }

        public async Task SaveOnboardingStepAsync(string userId, int step, object stepData)
        {
            try
            {
                var progress = await GetOnboardingProgressAsync(userId);
                progress.CurrentStep = step;
                progress.StepData[step.ToString()] = stepData;
                
                if (step >= 5) // Assuming 5 steps total
                {
                    progress.IsComplete = true;
                    progress.CompletedAt = DateTime.UtcNow;
                }

                var cacheKey = string.Format(ONBOARDING_CACHE_KEY, userId);
                var serializedProgress = JsonSerializer.Serialize(progress);
                
                await _cache.SetStringAsync(cacheKey, serializedProgress, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) // Onboarding data expires in a week
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving onboarding step {Step} for user {UserId}", step, userId);
            }
        }

        #endregion

        #region AI Personalization

        public async Task<PersonalizationProfile> GeneratePersonalizationAsync(UserProfile profile)
        {
            try
            {
                var personalization = new PersonalizationProfile();

                // Generate risk assessment
                personalization.RiskAssessment = await AssessUserRiskAsync(profile);
                
                // Configure coaching settings
                personalization.CoachSettings = GenerateCoachingSettings(profile);
                
                // Set scenario parameters
                personalization.ScenarioConfig = GenerateScenarioParameters(profile);
                
                // Get recommended training
                personalization.RecommendedTrainingTypes = await GetRecommendedTrainingInternalAsync(profile);

                _logger.LogInformation("Generated AI personalization for user {UserId}", profile.UserId);
                return personalization;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating personalization for user {UserId}", profile.UserId);
                return new PersonalizationProfile(); // Return default
            }
        }

        public async Task<ScenarioParameters> GetScenarioParametersAsync(string userId)
        {
            var profile = await GetUserProfileAsync(userId);
            return profile?.Personalization.ScenarioConfig ?? new ScenarioParameters();
        }

        public async Task<CoachingSettings> GetCoachingSettingsAsync(string userId)
        {
            var profile = await GetUserProfileAsync(userId);
            return profile?.Personalization.CoachSettings ?? new CoachingSettings();
        }

        public async Task<List<string>> GetRecommendedTrainingAsync(string userId)
        {
            var profile = await GetUserProfileAsync(userId);
            return profile?.Personalization.RecommendedTrainingTypes ?? new List<string> { "phishing" };
        }

        #endregion

        #region Progress Tracking

        public async Task UpdateProgressAsync(string userId, TrainingSession session)
        {
            try
            {
                var profile = await GetUserProfileAsync(userId);
                if (profile == null) return;

                profile.Progress.RecentSessions.Add(session);
                profile.Progress.TotalSessionsCompleted++;
                profile.Progress.TotalScenariosCompleted += session.ScenariosCompleted.Count;
                profile.Progress.LastTrainingDate = session.StartTime;
                
                // Keep only last 10 sessions
                if (profile.Progress.RecentSessions.Count > 10)
                {
                    profile.Progress.RecentSessions.RemoveAt(0);
                }

                // Recalculate overall accuracy
                profile.Progress.OverallAccuracy = profile.Progress.RecentSessions.Average(s => s.AccuracyScore);

                await UpdateUserProfileAsync(profile);
                
                _logger.LogInformation("Updated progress for user {UserId} - Session: {SessionId}", userId, session.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating progress for user {UserId}", userId);
            }
        }

        public async Task<AnalyticsData> CalculateAnalyticsAsync(string userId)
        {
            try
            {
                var profile = await GetUserProfileAsync(userId);
                if (profile == null) return new AnalyticsData();

                var analytics = new AnalyticsData
                {
                    LastCalculated = DateTime.UtcNow,
                    SecurityAwarenessScore = CalculateSecurityAwarenessScore(profile),
                    ThreatDetectionRate = CalculateThreatDetectionRate(profile),
                    LearningVelocity = CalculateLearningVelocity(profile)
                };

                // Add behavioral metrics
                analytics.BehavioralMetrics = CalculateBehavioralMetrics(profile);
                
                // Generate predictions
                analytics.RiskPredictions = await GenerateRiskPredictions(profile);
                analytics.RecommendedFocus = await GenerateRecommendedFocus(profile);

                // Get comparison metrics
                analytics.Comparisons = await GetComparisonMetricsAsync(userId);

                return analytics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating analytics for user {UserId}", userId);
                return new AnalyticsData();
            }
        }

        public async Task<ComparisonMetrics> GetComparisonMetricsAsync(string userId)
        {
            try
            {
                var profile = await GetUserProfileAsync(userId);
                if (profile == null) return new ComparisonMetrics();

                // In production, these would be calculated from actual user data
                var userScore = profile.Progress.Analytics.SecurityAwarenessScore;
                
                return new ComparisonMetrics
                {
                    PeerAverageScore = GetPeerAverage(profile.SelectedRoleId, profile.Industry),
                    IndustryAverageScore = GetIndustryAverage(profile.Industry),
                    RoleAverageScore = GetRoleAverage(profile.SelectedRoleId),
                    PerformanceRank = CalculatePerformanceRank(userScore, profile)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comparison metrics for user {UserId}", userId);
                return new ComparisonMetrics();
            }
        }

        #endregion

        #region Risk Assessment

        public async Task<RiskProfile> AssessUserRiskAsync(UserProfile profile)
        {
            try
            {
                var riskProfile = new RiskProfile();
                
                // Assess risk level based on role and industry
                var riskFactors = CalculateRiskFactors(profile);
                riskProfile.RiskLevel = DetermineRiskLevel(riskFactors);
                
                // Set primary threats based on role
                riskProfile.PrimaryThreats = GetRoleBasedThreats(profile.SelectedRoleId);
                
                // Identify vulnerability areas
                riskProfile.VulnerabilityAreas = await IdentifyVulnerabilities(profile);
                
                // Calculate threat scores
                riskProfile.ThreatScores = CalculateThreatScores(profile);

                return riskProfile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assessing risk for user {UserId}", profile.UserId);
                return new RiskProfile();
            }
        }

        public async Task<List<string>> GetPersonalizedThreatsAsync(string userId)
        {
            var profile = await GetUserProfileAsync(userId);
            return profile?.Personalization.RiskAssessment.PrimaryThreats ?? new List<string> { "phishing", "malware" };
        }

        public async Task<double> CalculateSecurityScoreAsync(string userId)
        {
            var profile = await GetUserProfileAsync(userId);
            return profile?.Progress.Analytics.SecurityAwarenessScore ?? 0.6; // Default moderate score
        }

        #endregion

        #region Learning Path Management

        public async Task<List<string>> GetLearningPathAsync(string userId)
        {
            var profile = await GetUserProfileAsync(userId);
            if (profile == null) return new List<string> { "email_security", "social_engineering" };

            var path = new List<string>();
            var experienceLevel = profile.ExperienceLevel ?? "intermediate";
            
            // Base path for all users
            path.Add("email_security");
            
            // Add role-specific training
            switch (profile.SelectedRoleId)
            {
                case "developer":
                    path.AddRange(new[] { "secure_development", "supply_chain_security", "code_injection" });
                    break;
                case "finance":
                    path.AddRange(new[] { "fraud_prevention", "business_email_compromise", "financial_regulations" });
                    break;
                case "marketing":
                    path.AddRange(new[] { "social_media_security", "brand_protection", "customer_data_protection" });
                    break;
                case "hr":
                    path.AddRange(new[] { "employee_data_security", "recruitment_security", "internal_threats" });
                    break;
                default:
                    path.AddRange(new[] { "social_engineering", "password_security", "device_security" });
                    break;
            }
            
            // Add advanced topics for experienced users
            if (experienceLevel is "advanced" or "expert")
            {
                path.AddRange(new[] { "advanced_persistent_threats", "incident_response", "security_leadership" });
            }

            return path;
        }

        public async Task<string> GetNextRecommendedTrainingAsync(string userId)
        {
            var path = await GetLearningPathAsync(userId);
            var profile = await GetUserProfileAsync(userId);
            
            if (profile == null || path.Count == 0) return "email_security";

            // Find the next uncompleted training in the path
            foreach (var training in path)
            {
                var hasCompleted = profile.Progress.RecentSessions
                    .Any(s => s.TrainingType == training && s.AccuracyScore >= 0.7);
                
                if (!hasCompleted) return training;
            }

            // If all completed, return the most relevant for reinforcement
            return path.First();
        }

        public async Task UpdateLearningProgressAsync(string userId, string trainingType, double score)
        {
            var profile = await GetUserProfileAsync(userId);
            if (profile == null) return;

            // Update skill level
            if (!profile.Progress.SkillLevels.ContainsKey(trainingType))
            {
                profile.Progress.SkillLevels[trainingType] = new SkillLevel { SkillName = trainingType };
            }

            var skill = profile.Progress.SkillLevels[trainingType];
            skill.CurrentLevel = Math.Max(skill.CurrentLevel, score);
            skill.LastAssessed = DateTime.UtcNow;

            await UpdateUserProfileAsync(profile);
        }

        #endregion

        #region Scenario Generation Support

        public async Task<Dictionary<string, object>> GetScenarioContextAsync(string userId)
        {
            var profile = await GetUserProfileAsync(userId);
            if (profile == null) return new Dictionary<string, object>();

            return new Dictionary<string, object>
            {
                ["company"] = profile.Company,
                ["role"] = profile.JobTitle,
                ["industry"] = profile.Industry ?? "Technology",
                ["experience"] = profile.ExperienceLevel ?? "intermediate",
                ["tools"] = profile.SelectedTools,
                ["riskLevel"] = profile.Personalization.RiskAssessment.RiskLevel,
                ["primaryThreats"] = profile.Personalization.RiskAssessment.PrimaryThreats
            };
        }

        public async Task<List<string>> GeneratePersonalizedEmailsAsync(string userId, string scenarioType)
        {
            try
            {
                var context = await GetScenarioContextAsync(userId);
                
                // Use AI to generate personalized phishing emails
                var prompt = BuildEmailGenerationPrompt(context, scenarioType);
                var response = await _semanticKernel.ExecutePromptAsync(prompt);
                
                // Parse the response into individual emails (simplified)
                var emails = response.Split(new[] { "---EMAIL---" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .Where(e => !string.IsNullOrEmpty(e))
                    .ToList();

                return emails.Take(5).ToList(); // Return up to 5 emails
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating personalized emails for user {UserId}", userId);
                return GetFallbackEmails(scenarioType);
            }
        }

        public async Task<string> GenerateCoachingTipAsync(string userId, string context)
        {
            try
            {
                var profile = await GetUserProfileAsync(userId);
                if (profile == null) return "Great job staying alert to security threats!";

                var coachingStyle = profile.Personalization.CoachSettings.CoachingStyle;
                var experienceLevel = profile.ExperienceLevel ?? "intermediate";

                var prompt = $"As a {coachingStyle} AI security coach for a {experienceLevel} level {profile.JobTitle}, provide a helpful tip about: {context}";
                
                return await _semanticKernel.ExecutePromptAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating coaching tip for user {UserId}", userId);
                return "Remember to verify suspicious emails before taking any action.";
            }
        }

        #endregion

        #region Private Helper Methods

        private async Task CacheProfileAsync(UserProfile profile)
        {
            var cacheKey = string.Format(PROFILE_CACHE_KEY, profile.UserId);
            var serializedProfile = JsonSerializer.Serialize(profile);
            
            await _cache.SetStringAsync(cacheKey, serializedProfile, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_EXPIRATION_MINUTES)
            });
        }

        private CoachingSettings GenerateCoachingSettings(UserProfile profile)
        {
            var settings = new CoachingSettings();
            
            settings.CoachingStyle = profile.ExperienceLevel switch
            {
                "beginner" => "supportive",
                "intermediate" => "balanced",
                "advanced" => "direct",
                "expert" => "adaptive",
                _ => "supportive"
            };

            settings.InterventionLevel = profile.DifficultyPreference switch
            {
                "easy" => "high",
                "moderate" => "moderate",
                "challenging" => "minimal",
                "adaptive" => "moderate",
                _ => "moderate"
            };

            return settings;
        }

        private ScenarioParameters GenerateScenarioParameters(UserProfile profile)
        {
            var parameters = new ScenarioParameters();
            
            parameters.DifficultyMultiplier = profile.ExperienceLevel switch
            {
                "beginner" => 0.6,
                "intermediate" => 1.0,
                "advanced" => 1.4,
                "expert" => 1.8,
                _ => 1.0
            };

            parameters.EnableAdvancedThreats = profile.ExperienceLevel is "advanced" or "expert";
            parameters.UseCompanyBranding = !string.IsNullOrEmpty(profile.Company);
            parameters.UseRoleSpecificLanguage = !string.IsNullOrEmpty(profile.JobTitle);

            // Set preferred scenario types based on role
            parameters.PreferredScenarioTypes = profile.SelectedRoleId switch
            {
                "developer" => new List<string> { "phishing", "supply_chain", "social_engineering" },
                "marketing" => new List<string> { "social_media", "brand_protection", "customer_impersonation" },
                "finance" => new List<string> { "business_email_compromise", "invoice_fraud", "wire_fraud" },
                "hr" => new List<string> { "employee_data_theft", "fake_applications", "social_engineering" },
                _ => new List<string> { "phishing", "social_engineering", "malware" }
            };

            return parameters;
        }

        private async Task<List<string>> GetRecommendedTrainingInternalAsync(UserProfile profile)
        {
            var training = new List<string> { "email_security" };
            
            switch (profile.SelectedRoleId)
            {
                case "developer":
                    training.AddRange(new[] { "secure_development", "supply_chain_security" });
                    break;
                case "finance":
                    training.AddRange(new[] { "fraud_prevention", "business_email_compromise" });
                    break;
                case "marketing":
                    training.AddRange(new[] { "social_media_security", "brand_protection" });
                    break;
                case "hr":
                    training.AddRange(new[] { "employee_data_security", "recruitment_security" });
                    break;
                case "executive":
                    training.AddRange(new[] { "advanced_threats", "strategic_security" });
                    break;
                default:
                    training.AddRange(new[] { "social_engineering", "password_security" });
                    break;
            }

            return training;
        }

        private double CalculateSecurityAwarenessScore(UserProfile profile)
        {
            if (profile.Progress.RecentSessions.Count == 0) return 0.5; // Default starting score
            
            var baseScore = profile.Progress.OverallAccuracy;
            var sessionCount = profile.Progress.TotalSessionsCompleted;
            var recency = (DateTime.UtcNow - profile.Progress.LastTrainingDate).TotalDays;
            
            // Adjust score based on training frequency and recency
            var recencyFactor = recency < 7 ? 1.0 : Math.Max(0.8, 1.0 - (recency / 30));
            var experienceFactor = Math.Min(1.2, 1.0 + (sessionCount / 50.0));
            
            return Math.Min(1.0, baseScore * recencyFactor * experienceFactor);
        }

        private double CalculateThreatDetectionRate(UserProfile profile)
        {
            // Calculate based on phishing and threat detection performance
            var phishingSessions = profile.Progress.RecentSessions
                .Where(s => s.TrainingType.Contains("phishing") || s.TrainingType.Contains("threat"))
                .ToList();
            
            if (phishingSessions.Count == 0) return 0.5;
            
            return phishingSessions.Average(s => s.AccuracyScore);
        }

        private double CalculateLearningVelocity(UserProfile profile)
        {
            if (profile.Progress.RecentSessions.Count < 2) return 0.5;
            
            var sessions = profile.Progress.RecentSessions.OrderBy(s => s.StartTime).ToList();
            var improvements = new List<double>();
            
            for (int i = 1; i < sessions.Count; i++)
            {
                var improvement = sessions[i].AccuracyScore - sessions[i-1].AccuracyScore;
                improvements.Add(improvement);
            }
            
            return Math.Max(0, Math.Min(1, improvements.Average() + 0.5));
        }

        private Dictionary<string, double> CalculateBehavioralMetrics(UserProfile profile)
        {
            return new Dictionary<string, double>
            {
                ["consistency"] = CalculateConsistency(profile),
                ["engagement"] = CalculateEngagement(profile),
                ["retention"] = CalculateRetention(profile),
                ["adaptability"] = CalculateAdaptability(profile)
            };
        }

        private double CalculateConsistency(UserProfile profile)
        {
            if (profile.Progress.RecentSessions.Count < 3) return 0.5;
            
            var scores = profile.Progress.RecentSessions.Select(s => s.AccuracyScore).ToList();
            var variance = CalculateVariance(scores);
            
            return Math.Max(0, 1.0 - variance);
        }

        private double CalculateEngagement(UserProfile profile)
        {
            var daysSinceStart = (DateTime.UtcNow - profile.CreatedAt).TotalDays;
            if (daysSinceStart < 1) return 1.0;
            
            var sessionsPerWeek = profile.Progress.TotalSessionsCompleted / Math.Max(1, daysSinceStart / 7);
            
            return Math.Min(1.0, sessionsPerWeek / 3.0); // 3 sessions per week = max engagement
        }

        private double CalculateRetention(UserProfile profile)
        {
            if (profile.Progress.LastTrainingDate == DateTime.MinValue) return 0.0;
            
            var daysSinceLastTraining = (DateTime.UtcNow - profile.Progress.LastTrainingDate).TotalDays;
            
            return Math.Max(0, 1.0 - (daysSinceLastTraining / 30.0));
        }

        private double CalculateAdaptability(UserProfile profile)
        {
            // Measure how well user adapts to increasing difficulty
            var sessions = profile.Progress.RecentSessions.OrderBy(s => s.StartTime).ToList();
            if (sessions.Count < 3) return 0.5;
            
            var recentPerformance = sessions.TakeLast(3).Average(s => s.AccuracyScore);
            var earlyPerformance = sessions.Take(3).Average(s => s.AccuracyScore);
            
            return Math.Max(0, Math.Min(1, (recentPerformance - earlyPerformance) / 2 + 0.5));
        }

        private double CalculateVariance(List<double> values)
        {
            if (values.Count == 0) return 0;
            
            var mean = values.Average();
            var squaredDiffs = values.Select(v => Math.Pow(v - mean, 2));
            
            return squaredDiffs.Average();
        }

        private async Task<List<string>> GenerateRiskPredictions(UserProfile profile)
        {
            var predictions = new List<string>();
            
            // Add risk predictions based on profile and behavior
            var riskLevel = profile.Personalization.RiskAssessment.RiskLevel;
            var recentScore = profile.Progress.Analytics.SecurityAwarenessScore;
            
            if (riskLevel == "high" && recentScore < 0.7)
            {
                predictions.Add("High risk of successful phishing attack within next 30 days");
                predictions.Add("Recommend immediate advanced email security training");
            }
            
            if (profile.Progress.RecentSessions.Count == 0)
            {
                predictions.Add("User disengagement risk - recommend personalized intervention");
            }
            
            if (profile.SelectedRoleId == "developer" && recentScore < 0.6)
            {
                predictions.Add("Elevated supply chain attack risk due to role and performance");
            }
            
            return predictions;
        }

        private async Task<List<string>> GenerateRecommendedFocus(UserProfile profile)
        {
            var focus = new List<string>();
            
            // Analyze weak areas
            var skillLevels = profile.Progress.SkillLevels;
            var weakSkills = skillLevels.Where(s => s.Value.CurrentLevel < 0.6).Select(s => s.Key).ToList();
            
            if (weakSkills.Contains("phishing") || weakSkills.Count == 0)
            {
                focus.Add("Email security and phishing recognition");
            }
            
            if (profile.SelectedRoleId == "developer")
            {
                focus.Add("Secure development practices");
            }
            
            if (profile.Industry == "Financial Services")
            {
                focus.Add("Fraud prevention and business email compromise");
            }
            
            focus.Add("Regular security awareness reinforcement");
            
            return focus;
        }

        private Dictionary<string, double> CalculateRiskFactors(UserProfile profile)
        {
            var factors = new Dictionary<string, double>();
            
            // Role-based risk
            factors["role"] = profile.SelectedRoleId switch
            {
                "executive" => 1.0,
                "finance" => 0.9,
                "hr" => 0.8,
                "marketing" => 0.7,
                "developer" => 0.6,
                _ => 0.5
            };
            
            // Industry-based risk
            factors["industry"] = profile.Industry switch
            {
                "Financial Services" => 1.0,
                "Healthcare" => 0.9,
                "Government" => 0.9,
                "Technology" => 0.7,
                _ => 0.6
            };
            
            // Company size risk
            factors["company_size"] = profile.CompanySize switch
            {
                "enterprise" => 0.8,
                "large" => 0.7,
                "medium" => 0.6,
                "small" => 0.5,
                "startup" => 0.4,
                _ => 0.5
            };
            
            return factors;
        }

        private string DetermineRiskLevel(Dictionary<string, double> riskFactors)
        {
            var averageRisk = riskFactors.Values.Average();
            
            return averageRisk switch
            {
                >= 0.8 => "high",
                >= 0.6 => "medium",
                _ => "low"
            };
        }

        private List<string> GetRoleBasedThreats(string? roleId)
        {
            return roleId switch
            {
                "developer" => new List<string> { "supply_chain_attacks", "malicious_dependencies", "code_injection", "social_engineering" },
                "marketing" => new List<string> { "social_media_attacks", "brand_impersonation", "customer_data_theft", "content_manipulation" },
                "finance" => new List<string> { "business_email_compromise", "invoice_fraud", "wire_transfer_scams", "financial_malware" },
                "hr" => new List<string> { "employee_data_theft", "fake_job_applications", "internal_threats", "recruitment_scams" },
                "executive" => new List<string> { "ceo_fraud", "strategic_information_theft", "advanced_persistent_threats", "board_impersonation" },
                "it" => new List<string> { "privileged_access_attacks", "system_compromise", "insider_threats", "infrastructure_attacks" },
                _ => new List<string> { "phishing", "social_engineering", "malware", "credential_theft" }
            };
        }

        private async Task<List<string>> IdentifyVulnerabilities(UserProfile profile)
        {
            var vulnerabilities = new List<string>();
            
            // Check experience level
            if (profile.ExperienceLevel == "beginner")
            {
                vulnerabilities.Add("Limited security awareness");
                vulnerabilities.Add("Difficulty recognizing sophisticated threats");
            }
            
            // Check training history
            if (profile.Progress.TotalSessionsCompleted < 3)
            {
                vulnerabilities.Add("Insufficient training exposure");
            }
            
            // Check role-specific vulnerabilities
            switch (profile.SelectedRoleId)
            {
                case "developer":
                    vulnerabilities.Add("Access to critical systems and code");
                    vulnerabilities.Add("Exposure to external repositories and tools");
                    break;
                case "finance":
                    vulnerabilities.Add("Access to financial systems and data");
                    vulnerabilities.Add("Authority to approve financial transactions");
                    break;
                case "marketing":
                    vulnerabilities.Add("High public profile and social media presence");
                    vulnerabilities.Add("Access to customer data and brand assets");
                    break;
            }
            
            return vulnerabilities;
        }

        private Dictionary<string, double> CalculateThreatScores(UserProfile profile)
        {
            var scores = new Dictionary<string, double>();
            var baseScore = 0.5;
            
            // Calculate scores for each threat type
            foreach (var threat in profile.Personalization.RiskAssessment.PrimaryThreats)
            {
                var score = baseScore;
                
                // Adjust based on role
                if (profile.SelectedRoleId == "developer" && threat.Contains("supply_chain"))
                    score += 0.3;
                else if (profile.SelectedRoleId == "finance" && threat.Contains("fraud"))
                    score += 0.3;
                else if (profile.SelectedRoleId == "marketing" && threat.Contains("social"))
                    score += 0.3;
                
                // Adjust based on experience
                if (profile.ExperienceLevel == "beginner")
                    score += 0.2;
                else if (profile.ExperienceLevel == "expert")
                    score -= 0.2;
                
                scores[threat] = Math.Min(1.0, Math.Max(0.0, score));
            }
            
            return scores;
        }

        private double GetPeerAverage(string? roleId, string? industry)
        {
            // In production, this would query actual peer data
            var baseScore = 0.65;
            
            // Adjust based on role
            if (roleId is "executive" or "it") baseScore += 0.1;
            else if (roleId is "developer" or "marketing") baseScore -= 0.05;
            
            // Adjust based on industry
            if (industry is "Financial Services" or "Healthcare") baseScore += 0.1;
            else if (industry == "Technology") baseScore += 0.05;
            
            return Math.Min(1.0, Math.Max(0.0, baseScore));
        }

        private double GetIndustryAverage(string? industry)
        {
            return industry switch
            {
                "Financial Services" => 0.75,
                "Healthcare" => 0.72,
                "Government" => 0.70,
                "Technology" => 0.68,
                "Education" => 0.65,
                _ => 0.62
            };
        }

        private double GetRoleAverage(string? roleId)
        {
            return roleId switch
            {
                "executive" => 0.72,
                "it" => 0.78,
                "finance" => 0.70,
                "hr" => 0.68,
                "developer" => 0.65,
                "marketing" => 0.63,
                _ => 0.62
            };
        }

        private string CalculatePerformanceRank(double userScore, UserProfile profile)
        {
            var peerAverage = GetPeerAverage(profile.SelectedRoleId, profile.Industry);
            
            if (userScore >= peerAverage + 0.2) return "top";
            if (userScore >= peerAverage + 0.1) return "above_average";
            if (userScore >= peerAverage - 0.1) return "average";
            if (userScore >= peerAverage - 0.2) return "below_average";
            return "needs_improvement";
        }

        private string BuildEmailGenerationPrompt(Dictionary<string, object> context, string scenarioType)
        {
            var company = context.GetValueOrDefault("company", "YourCompany");
            var role = context.GetValueOrDefault("role", "Employee");
            var industry = context.GetValueOrDefault("industry", "Technology");
            
            return $@"Generate 3 realistic phishing emails targeting a {role} at {company} in the {industry} industry. 
                     Scenario type: {scenarioType}
                     Make them sophisticated but identifiable with proper security awareness.
                     Include realistic sender addresses, subject lines, and body content.
                     Separate each email with ---EMAIL--- delimiter.";
        }

        private List<string> GetFallbackEmails(string scenarioType)
        {
            // Fallback emails if AI generation fails
            return new List<string>
            {
                "Your account will be suspended unless you verify your credentials immediately.",
                "Urgent: Please review and approve this wire transfer request.",
                "IT Security Alert: Click here to secure your account.",
                "Important document shared with you - please review ASAP.",
                "Your password expires today - update it now to avoid lockout."
            };
        }

        #endregion
    }
}