using System.ComponentModel.DataAnnotations;

namespace IncidentIQ.Application.Models
{
    public class UserProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Basic Information
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Company { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public string? Phone { get; set; }

        // Role and Experience
        public string? SelectedRoleId { get; set; }
        public bool IsCustomRole { get; set; }
        public string? CustomRoleDescription { get; set; }
        public string? ExperienceLevel { get; set; }

        // Company Context
        public string? Industry { get; set; }
        public string? CompanySize { get; set; }
        public List<string> SelectedTools { get; set; } = new();
        public string? SecurityConcerns { get; set; }

        // Training Preferences
        public string? TrainingFrequency { get; set; }
        public string? DifficultyPreference { get; set; }

        // AI Personalization Data
        public PersonalizationProfile Personalization { get; set; } = new();
        
        // Progress Tracking
        public ProgressData Progress { get; set; } = new();
    }

    public class PersonalizationProfile
    {
        // Risk Assessment
        public RiskProfile RiskAssessment { get; set; } = new();
        
        // Learning Path
        public List<string> RecommendedTrainingTypes { get; set; } = new();
        public string CurrentLearningPhase { get; set; } = "foundation";
        
        // AI Coach Calibration
        public CoachingSettings CoachSettings { get; set; } = new();
        
        // Scenario Generation Parameters
        public ScenarioParameters ScenarioConfig { get; set; } = new();
    }

    public class RiskProfile
    {
        public string RiskLevel { get; set; } = "medium"; // low, medium, high
        public List<string> PrimaryThreats { get; set; } = new();
        public List<string> VulnerabilityAreas { get; set; } = new();
        public Dictionary<string, double> ThreatScores { get; set; } = new();
    }

    public class CoachingSettings
    {
        public string CoachingStyle { get; set; } = "supportive"; // supportive, direct, adaptive
        public string InterventionLevel { get; set; } = "moderate"; // minimal, moderate, high
        public bool EnableRealTimeHints { get; set; } = true;
        public bool EnableProgressCelebration { get; set; } = true;
        public string PreferredFeedbackTiming { get; set; } = "immediate"; // immediate, delayed, summary
    }

    public class ScenarioParameters
    {
        // Content Difficulty
        public double DifficultyMultiplier { get; set; } = 1.0;
        public bool EnableAdvancedThreats { get; set; } = false;
        public bool EnableIndustrySpecificContent { get; set; } = true;
        
        // Scenario Types
        public List<string> PreferredScenarioTypes { get; set; } = new() { "phishing", "social_engineering" };
        public Dictionary<string, double> ScenarioWeights { get; set; } = new();
        
        // Personalization Factors
        public bool UseCompanyBranding { get; set; } = true;
        public bool UseRoleSpecificLanguage { get; set; } = true;
        public bool IncludeCurrentEvents { get; set; } = true;
    }

    public class ProgressData
    {
        public DateTime LastTrainingDate { get; set; } = DateTime.MinValue;
        public int TotalSessionsCompleted { get; set; } = 0;
        public int TotalScenariosCompleted { get; set; } = 0;
        public double OverallAccuracy { get; set; } = 0.0;
        
        // Skill Assessments
        public Dictionary<string, SkillLevel> SkillLevels { get; set; } = new();
        
        // Training History
        public List<TrainingSession> RecentSessions { get; set; } = new();
        
        // Analytics
        public AnalyticsData Analytics { get; set; } = new();
    }

    public class SkillLevel
    {
        public string SkillName { get; set; } = "";
        public double CurrentLevel { get; set; } = 0.0; // 0.0 - 1.0
        public double TargetLevel { get; set; } = 0.8;
        public DateTime LastAssessed { get; set; } = DateTime.UtcNow;
        public List<string> StrengthAreas { get; set; } = new();
        public List<string> ImprovementAreas { get; set; } = new();
    }

    public class TrainingSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        public string TrainingType { get; set; } = "";
        public List<string> ScenariosCompleted { get; set; } = new();
        public double AccuracyScore { get; set; } = 0.0;
        public Dictionary<string, object> SessionMetrics { get; set; } = new();
        public List<string> LearningOutcomes { get; set; } = new();
    }

    public class AnalyticsData
    {
        public DateTime LastCalculated { get; set; } = DateTime.UtcNow;
        
        // Performance Metrics
        public double SecurityAwarenessScore { get; set; } = 0.0;
        public double ThreatDetectionRate { get; set; } = 0.0;
        public double LearningVelocity { get; set; } = 0.0;
        
        // Behavioral Patterns
        public Dictionary<string, double> BehavioralMetrics { get; set; } = new();
        public List<string> LearningSimilarities { get; set; } = new();
        
        // Predictive Insights
        public List<string> RiskPredictions { get; set; } = new();
        public List<string> RecommendedFocus { get; set; } = new();
        
        // Comparative Data
        public ComparisonMetrics Comparisons { get; set; } = new();
    }

    public class ComparisonMetrics
    {
        public double PeerAverageScore { get; set; } = 0.0;
        public double IndustryAverageScore { get; set; } = 0.0;
        public double RoleAverageScore { get; set; } = 0.0;
        public string PerformanceRank { get; set; } = "average"; // top, above_average, average, below_average, needs_improvement
    }

    // Helper classes for UI binding
    public class OnboardingFormData
    {
        // Step 1 - Basic Info
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Company name is required")]
        public string Company { get; set; } = "";

        [Required(ErrorMessage = "Job title is required")]
        public string JobTitle { get; set; } = "";

        public string? Phone { get; set; }

        // Step 2 - Role Selection
        public string? SelectedRoleId { get; set; }
        public bool IsCustomRole { get; set; }
        public string? CustomRoleDescription { get; set; }

        // Step 3 - Experience and Context
        [Required(ErrorMessage = "Please select your experience level")]
        public string? ExperienceLevel { get; set; }

        [Required(ErrorMessage = "Please select your industry")]
        public string? Industry { get; set; }

        [Required(ErrorMessage = "Please select your company size")]
        public string? CompanySize { get; set; }

        public HashSet<string> SelectedTools { get; set; } = new();
        public string? SecurityConcerns { get; set; }

        [Required(ErrorMessage = "Please select your preferred training frequency")]
        public string? TrainingFrequency { get; set; }

        [Required(ErrorMessage = "Please select your difficulty preference")]
        public string? DifficultyPreference { get; set; }

        public UserProfile ToUserProfile(string userId)
        {
            var profile = new UserProfile
            {
                UserId = userId,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Company = Company,
                JobTitle = JobTitle,
                Phone = Phone,
                SelectedRoleId = SelectedRoleId,
                IsCustomRole = IsCustomRole,
                CustomRoleDescription = CustomRoleDescription,
                ExperienceLevel = ExperienceLevel,
                Industry = Industry,
                CompanySize = CompanySize,
                SelectedTools = SelectedTools.ToList(),
                SecurityConcerns = SecurityConcerns,
                TrainingFrequency = TrainingFrequency,
                DifficultyPreference = DifficultyPreference
            };

            // Initialize personalization based on form data
            profile.Personalization = CreatePersonalizationProfile(profile);
            
            return profile;
        }

        private PersonalizationProfile CreatePersonalizationProfile(UserProfile profile)
        {
            var personalization = new PersonalizationProfile();

            // Set up risk assessment based on role and industry
            personalization.RiskAssessment = AssessRiskProfile(profile);
            
            // Configure coaching settings based on experience level
            personalization.CoachSettings = ConfigureCoachingSettings(profile);
            
            // Set up scenario parameters
            personalization.ScenarioConfig = ConfigureScenarioParameters(profile);
            
            // Determine recommended training types
            personalization.RecommendedTrainingTypes = GetRecommendedTraining(profile);
            
            return personalization;
        }

        private RiskProfile AssessRiskProfile(UserProfile profile)
        {
            var riskProfile = new RiskProfile();
            
            // Assess risk level based on role and industry
            var highRiskRoles = new[] { "executive", "finance", "hr", "it" };
            var highRiskIndustries = new[] { "Financial Services", "Healthcare", "Government" };
            
            if ((profile.SelectedRoleId != null && highRiskRoles.Contains(profile.SelectedRoleId)) ||
                (profile.Industry != null && highRiskIndustries.Contains(profile.Industry)))
            {
                riskProfile.RiskLevel = "high";
            }
            else if (profile.SelectedRoleId == "marketing" || profile.Industry == "Technology")
            {
                riskProfile.RiskLevel = "medium";
            }
            else
            {
                riskProfile.RiskLevel = "low";
            }

            // Set primary threats based on role
            riskProfile.PrimaryThreats = profile.SelectedRoleId switch
            {
                "developer" => new List<string> { "supply_chain", "malicious_code", "social_engineering" },
                "marketing" => new List<string> { "social_media_attacks", "brand_impersonation", "customer_data_theft" },
                "finance" => new List<string> { "business_email_compromise", "invoice_fraud", "wire_transfer_scams" },
                "hr" => new List<string> { "employee_data_theft", "fake_applications", "internal_threats" },
                "executive" => new List<string> { "ceo_fraud", "strategic_info_theft", "advanced_persistent_threats" },
                _ => new List<string> { "phishing", "social_engineering", "malware" }
            };

            return riskProfile;
        }

        private CoachingSettings ConfigureCoachingSettings(UserProfile profile)
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
                "adaptive" => "adaptive",
                _ => "moderate"
            };

            return settings;
        }

        private ScenarioParameters ConfigureScenarioParameters(UserProfile profile)
        {
            var parameters = new ScenarioParameters();
            
            parameters.DifficultyMultiplier = profile.ExperienceLevel switch
            {
                "beginner" => 0.5,
                "intermediate" => 1.0,
                "advanced" => 1.5,
                "expert" => 2.0,
                _ => 1.0
            };

            parameters.EnableAdvancedThreats = profile.ExperienceLevel is "advanced" or "expert";
            
            // Set scenario preferences based on role
            parameters.PreferredScenarioTypes = profile.SelectedRoleId switch
            {
                "developer" => new List<string> { "phishing", "supply_chain", "social_engineering" },
                "marketing" => new List<string> { "social_media", "brand_protection", "customer_impersonation" },
                "finance" => new List<string> { "business_email_compromise", "invoice_fraud", "wire_fraud" },
                _ => new List<string> { "phishing", "social_engineering", "malware" }
            };

            return parameters;
        }

        private List<string> GetRecommendedTraining(UserProfile profile)
        {
            var training = new List<string> { "email_security" }; // Everyone starts with email
            
            // Add role-specific training
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
                case "executive":
                    training.AddRange(new[] { "advanced_threats", "strategic_security" });
                    break;
                default:
                    training.AddRange(new[] { "social_engineering", "password_security" });
                    break;
            }

            return training;
        }
    }
}