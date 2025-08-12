using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;
using IncidentIQ.Application.Interfaces;
using IncidentIQ.Application.Interfaces.AI;
using IncidentIQ.Application.Models;

namespace IncidentIQ.Infrastructure.Services
{
    public class PersonalizedScenarioService : IPersonalizedScenarioService
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ISemanticKernelService _semanticKernel;
        private readonly IDistributedCache _cache;
        private readonly ILogger<PersonalizedScenarioService> _logger;

        private const string RECOMMENDATIONS_CACHE_KEY = "recommendations_{0}";
        private const string SCENARIO_ANALYTICS_CACHE_KEY = "scenario_analytics_{0}";
        private const int CACHE_EXPIRATION_HOURS = 2;

        public PersonalizedScenarioService(
            IUserProfileService userProfileService,
            ISemanticKernelService semanticKernel,
            IDistributedCache cache,
            ILogger<PersonalizedScenarioService> logger)
        {
            _userProfileService = userProfileService;
            _semanticKernel = semanticKernel;
            _cache = cache;
            _logger = logger;
        }

        #region Scenario Generation

        public async Task<List<PersonalizedRecommendation>> GeneratePersonalizedRecommendationsAsync(string userId)
        {
            try
            {
                var cacheKey = string.Format(RECOMMENDATIONS_CACHE_KEY, userId);
                var cachedRecommendations = await _cache.GetStringAsync(cacheKey);
                
                if (!string.IsNullOrEmpty(cachedRecommendations))
                {
                    return JsonSerializer.Deserialize<List<PersonalizedRecommendation>>(cachedRecommendations) ?? new();
                }

                var profile = await _userProfileService.GetUserProfileAsync(userId);
                if (profile == null)
                {
                    _logger.LogWarning("No profile found for user {UserId}", userId);
                    return new List<PersonalizedRecommendation>();
                }

                var recommendations = new List<PersonalizedRecommendation>();
                var context = await BuildPersonalizationContextAsync(userId);

                // Get role-specific templates
                var templates = await GetTemplatesForRoleAsync(profile.SelectedRoleId ?? "general");
                
                // Filter templates by industry and experience level
                var filteredTemplates = FilterTemplatesByUserContext(templates, profile);
                
                // Generate recommendations from top templates
                foreach (var template in filteredTemplates.Take(6)) // Top 6 recommendations
                {
                    var recommendation = await CreateRecommendationFromTemplateAsync(userId, template, context, profile);
                    recommendations.Add(recommendation);
                }

                // Sort by priority and relevance
                recommendations = recommendations.OrderByDescending(r => CalculateRecommendationScore(r, profile)).ToList();

                // Cache the results
                await CacheRecommendationsAsync(userId, recommendations);
                
                _logger.LogInformation("Generated {Count} personalized recommendations for user {UserId}", 
                    recommendations.Count, userId);
                
                return recommendations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating personalized recommendations for user {UserId}", userId);
                return new List<PersonalizedRecommendation>();
            }
        }

        public async Task<PersonalizedRecommendation> GenerateScenarioForRoleAsync(string userId, string roleId, string scenarioType)
        {
            try
            {
                var profile = await _userProfileService.GetUserProfileAsync(userId);
                if (profile == null) throw new ArgumentException("User profile not found");

                var templates = await GetTemplatesForRoleAsync(roleId);
                var template = templates.FirstOrDefault(t => t.ScenarioType == scenarioType) 
                    ?? templates.FirstOrDefault();

                if (template == null) throw new ArgumentException("No suitable template found");

                var context = await BuildPersonalizationContextAsync(userId);
                return await CreateRecommendationFromTemplateAsync(userId, template, context, profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating scenario for user {UserId}, role {RoleId}, type {ScenarioType}", 
                    userId, roleId, scenarioType);
                throw;
            }
        }

        public async Task<TrainingScenario> CreatePersonalizedScenarioAsync(string userId, ScenarioTemplate template)
        {
            try
            {
                var profile = await _userProfileService.GetUserProfileAsync(userId);
                if (profile == null) throw new ArgumentException("User profile not found");

                var context = await BuildPersonalizationContextAsync(userId);
                
                var scenario = new TrainingScenario
                {
                    Title = await PersonalizeContentAsync(template.BaseTitle, context),
                    Description = await PersonalizeContentAsync(template.BaseDescription, context),
                    ScenarioType = template.ScenarioType,
                    TargetRole = profile.SelectedRoleId ?? "general",
                    Industry = profile.Industry ?? "Technology",
                    DifficultyLevel = await DetermineOptimalDifficultyAsync(userId, template.ScenarioType),
                    EstimatedDurationMinutes = CalculateEstimatedDuration(template, profile),
                    IsPersonalized = true,
                    CompanyContext = context.GetValueOrDefault("COMPANY_NAME", "Your Company"),
                    Tags = GenerateScenarioTags(template, profile)
                };

                // Generate personalized content
                scenario.Content = await GeneratePersonalizedContentAsync(template, context, profile);
                
                // Set learning objectives
                scenario.LearningObjectives = await GenerateLearningObjectivesAsync(template, context, profile);
                
                _logger.LogInformation("Created personalized scenario {ScenarioId} for user {UserId}", 
                    scenario.Id, userId);
                
                return scenario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating personalized scenario for user {UserId} from template {TemplateId}", 
                    userId, template.Id);
                throw;
            }
        }

        #endregion

        #region Template Management

        public async Task<List<ScenarioTemplate>> GetTemplatesForRoleAsync(string roleId)
        {
            try
            {
                var allTemplates = RoleBasedScenarios.GetScenarioTemplates();
                
                if (allTemplates.ContainsKey(roleId))
                {
                    return allTemplates[roleId];
                }

                // Return general scenarios if role not found
                return allTemplates.Values.SelectMany(t => t).Where(t => t.TargetRoles.Contains("general")).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting templates for role {RoleId}", roleId);
                return new List<ScenarioTemplate>();
            }
        }

        public async Task<List<ScenarioTemplate>> GetTemplatesForIndustryAsync(string industry)
        {
            try
            {
                var allTemplates = RoleBasedScenarios.GetScenarioTemplates();
                var industryTemplates = new List<ScenarioTemplate>();

                foreach (var roleTemplates in allTemplates.Values)
                {
                    var matching = roleTemplates.Where(t => 
                        t.ApplicableIndustries.Contains(industry) || 
                        t.ApplicableIndustries.Contains("All")).ToList();
                    industryTemplates.AddRange(matching);
                }

                return industryTemplates.DistinctBy(t => t.Id).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting templates for industry {Industry}", industry);
                return new List<ScenarioTemplate>();
            }
        }

        public async Task<ScenarioTemplate?> GetTemplateAsync(string templateId)
        {
            try
            {
                var allTemplates = RoleBasedScenarios.GetScenarioTemplates();
                
                foreach (var roleTemplates in allTemplates.Values)
                {
                    var template = roleTemplates.FirstOrDefault(t => t.Id == templateId);
                    if (template != null) return template;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting template {TemplateId}", templateId);
                return null;
            }
        }

        #endregion

        #region Personalization

        public async Task<Dictionary<string, string>> BuildPersonalizationContextAsync(string userId)
        {
            try
            {
                var profile = await _userProfileService.GetUserProfileAsync(userId);
                if (profile == null) return new Dictionary<string, string>();

                var context = new Dictionary<string, string>
                {
                    ["USER_NAME"] = $"{profile.FirstName} {profile.LastName}",
                    ["FIRST_NAME"] = profile.FirstName,
                    ["LAST_NAME"] = profile.LastName,
                    ["COMPANY_NAME"] = profile.Company,
                    ["JOB_TITLE"] = profile.JobTitle,
                    ["INDUSTRY"] = profile.Industry ?? "Technology",
                    ["COMPANY_SIZE"] = GetCompanySizeDescription(profile.CompanySize),
                    ["EXPERIENCE_LEVEL"] = profile.ExperienceLevel ?? "intermediate",
                    ["EMAIL_DOMAIN"] = ExtractEmailDomain(profile.Email),
                    ["DEPARTMENT"] = ExtractDepartmentFromRole(profile.SelectedRoleId),
                    ["CURRENT_DATE"] = DateTime.Now.ToString("MMMM dd, yyyy"),
                    ["CURRENT_YEAR"] = DateTime.Now.Year.ToString()
                };

                // Add industry-specific context
                var industryTerms = await GetIndustryTerminologyAsync(profile.Industry ?? "Technology");
                foreach (var term in industryTerms)
                {
                    context[term.Key] = term.Value;
                }

                // Add role-specific tools
                var tools = await GetRoleSpecificToolsAsync(profile.SelectedRoleId ?? "general");
                if (tools.Any())
                {
                    context["PRIMARY_TOOL"] = tools.First();
                    context["SECONDARY_TOOL"] = tools.ElementAtOrDefault(1) ?? tools.First();
                }

                // Add generated elements
                context["REPOSITORY_NAME"] = GenerateRepositoryName(profile.Company, profile.Industry);
                context["PROJECT_NAME"] = GenerateProjectName(profile.Industry);
                context["CLIENT_NAME"] = GenerateClientName(profile.Industry);
                context["VENDOR_NAME"] = GenerateVendorName(profile.Industry);

                return context;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building personalization context for user {UserId}", userId);
                return new Dictionary<string, string>();
            }
        }

        public async Task<string> PersonalizeContentAsync(string content, Dictionary<string, string> context)
        {
            try
            {
                var personalizedContent = content;

                // Replace all placeholder variables
                foreach (var kvp in context)
                {
                    var placeholder = "{" + kvp.Key + "}";
                    personalizedContent = personalizedContent.Replace(placeholder, kvp.Value);
                }

                // Handle any remaining placeholders with AI generation
                var remainingPlaceholders = Regex.Matches(personalizedContent, @"\{([A-Z_]+)\}");
                
                foreach (Match match in remainingPlaceholders)
                {
                    var placeholder = match.Groups[1].Value;
                    var aiGenerated = await GeneratePlaceholderValueAsync(placeholder, context);
                    personalizedContent = personalizedContent.Replace(match.Value, aiGenerated);
                }

                return personalizedContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error personalizing content");
                return content; // Return original content if personalization fails
            }
        }

        public async Task<List<string>> GenerateCompanySpecificExamplesAsync(string userId, string scenarioType)
        {
            try
            {
                var context = await BuildPersonalizationContextAsync(userId);
                var companyName = context.GetValueOrDefault("COMPANY_NAME", "Your Company");
                var industry = context.GetValueOrDefault("INDUSTRY", "Technology");

                var examples = scenarioType switch
                {
                    "phishing" => GeneratePhishingExamples(companyName, industry),
                    "social_engineering" => GenerateSocialEngineeringExamples(companyName, industry),
                    "malware" => GenerateMalwareExamples(companyName, industry),
                    "data_breach" => GenerateDataBreachExamples(companyName, industry),
                    _ => GenerateGeneralExamples(companyName, industry)
                };

                return examples.Take(3).ToList(); // Return top 3 examples
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating company-specific examples for user {UserId}, scenario {ScenarioType}", 
                    userId, scenarioType);
                return new List<string>();
            }
        }

        #endregion

        #region Difficulty & Progression

        public async Task<string> DetermineOptimalDifficultyAsync(string userId, string scenarioType)
        {
            try
            {
                var profile = await _userProfileService.GetUserProfileAsync(userId);
                if (profile == null) return "intermediate";

                var baseLevel = profile.ExperienceLevel switch
                {
                    "beginner" => 0.3,
                    "intermediate" => 0.6,
                    "advanced" => 0.8,
                    "expert" => 1.0,
                    _ => 0.6
                };

                // Adjust based on scenario type performance
                if (profile.Progress.SkillLevels.ContainsKey(scenarioType))
                {
                    var skillLevel = profile.Progress.SkillLevels[scenarioType].CurrentLevel;
                    baseLevel = (baseLevel + skillLevel) / 2.0; // Average of base and skill level
                }

                // Adjust based on recent performance
                var recentSessions = profile.Progress.RecentSessions
                    .Where(s => s.TrainingType == scenarioType)
                    .TakeLast(3)
                    .ToList();

                if (recentSessions.Any())
                {
                    var recentPerformance = recentSessions.Average(s => s.AccuracyScore);
                    baseLevel = (baseLevel * 0.7) + (recentPerformance * 0.3); // Weight toward recent performance
                }

                return baseLevel switch
                {
                    < 0.4 => "easy",
                    < 0.7 => "intermediate",
                    < 0.9 => "challenging",
                    _ => "expert"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error determining optimal difficulty for user {UserId}, scenario {ScenarioType}", 
                    userId, scenarioType);
                return "intermediate";
            }
        }

        public async Task<List<PersonalizedRecommendation>> GetProgressiveRecommendationsAsync(string userId)
        {
            try
            {
                var profile = await _userProfileService.GetUserProfileAsync(userId);
                if (profile == null) return new List<PersonalizedRecommendation>();

                var learningPath = await _userProfileService.GetLearningPathAsync(userId);
                var recommendations = new List<PersonalizedRecommendation>();
                var context = await BuildPersonalizationContextAsync(userId);

                foreach (var trainingType in learningPath.Take(3)) // Next 3 in learning path
                {
                    var templates = await GetTemplatesForRoleAsync(profile.SelectedRoleId ?? "general");
                    var template = templates.FirstOrDefault(t => t.ScenarioType == trainingType);
                    
                    if (template != null)
                    {
                        var recommendation = await CreateRecommendationFromTemplateAsync(userId, template, context, profile);
                        recommendation.Priority = "high"; // Progressive recommendations are high priority
                        recommendation.ReasonForRecommendation = $"Next step in your {profile.SelectedRoleId} learning path";
                        recommendations.Add(recommendation);
                    }
                }

                return recommendations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting progressive recommendations for user {UserId}", userId);
                return new List<PersonalizedRecommendation>();
            }
        }

        public async Task<List<PersonalizedRecommendation>> GetReinforcementScenariosAsync(string userId)
        {
            try
            {
                var profile = await _userProfileService.GetUserProfileAsync(userId);
                if (profile == null) return new List<PersonalizedRecommendation>();

                var weakAreas = profile.Progress.SkillLevels
                    .Where(s => s.Value.CurrentLevel < 0.7)
                    .OrderBy(s => s.Value.CurrentLevel)
                    .Take(2)
                    .Select(s => s.Key)
                    .ToList();

                var recommendations = new List<PersonalizedRecommendation>();
                var context = await BuildPersonalizationContextAsync(userId);

                foreach (var scenarioType in weakAreas)
                {
                    var templates = await GetTemplatesForRoleAsync(profile.SelectedRoleId ?? "general");
                    var template = templates.FirstOrDefault(t => t.ScenarioType == scenarioType);
                    
                    if (template != null)
                    {
                        var recommendation = await CreateRecommendationFromTemplateAsync(userId, template, context, profile);
                        recommendation.Priority = "medium";
                        recommendation.ReasonForRecommendation = "Reinforcement training for improvement area";
                        recommendations.Add(recommendation);
                    }
                }

                return recommendations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reinforcement scenarios for user {UserId}", userId);
                return new List<PersonalizedRecommendation>();
            }
        }

        #endregion

        #region Industry-Specific Content

        public async Task<List<string>> GetIndustryThreatsAsync(string industry)
        {
            var industryThreats = new Dictionary<string, List<string>>
            {
                ["Technology"] = new() { "Supply chain attacks", "API vulnerabilities", "Code injection", "Insider threats", "Cloud misconfigurations" },
                ["Financial Services"] = new() { "Wire fraud", "Regulatory compliance attacks", "Customer data theft", "Payment card fraud", "Cryptocurrency theft" },
                ["Healthcare"] = new() { "HIPAA violations", "Medical device attacks", "Patient data theft", "Ransomware", "Research IP theft" },
                ["Manufacturing"] = new() { "Industrial espionage", "Supply chain disruption", "IoT device attacks", "Trade secret theft", "Operational technology attacks" },
                ["Education"] = new() { "Student data privacy", "Research theft", "Campus network attacks", "Grant fraud", "Online learning platform attacks" },
                ["Government"] = new() { "Nation-state attacks", "Classified data theft", "Election security", "Critical infrastructure attacks", "Espionage" },
                ["Retail"] = new() { "POS system attacks", "Customer data breaches", "E-commerce fraud", "Supply chain attacks", "Brand impersonation" }
            };

            return industryThreats.GetValueOrDefault(industry, industryThreats["Technology"]);
        }

        public async Task<List<string>> GetRoleSpecificToolsAsync(string roleId)
        {
            var roleTools = new Dictionary<string, List<string>>
            {
                ["developer"] = new() { "GitHub", "Docker", "AWS", "Visual Studio", "Jira", "Slack", "Jenkins" },
                ["marketing"] = new() { "HubSpot", "Salesforce", "Google Analytics", "Mailchimp", "Hootsuite", "Canva", "Slack" },
                ["hr"] = new() { "Workday", "BambooHR", "ADP", "LinkedIn", "Zoom", "Microsoft Teams", "DocuSign" },
                ["finance"] = new() { "QuickBooks", "SAP", "Excel", "Tableau", "Concur", "NetSuite", "Workiva" },
                ["sales"] = new() { "Salesforce", "HubSpot", "Zoom", "LinkedIn Sales Navigator", "Outreach", "Gong", "Slack" },
                ["executive"] = new() { "Tableau", "Microsoft Teams", "Zoom", "Office 365", "Slack", "Board portals", "CRM systems" },
                ["operations"] = new() { "ERP systems", "Supply chain software", "Inventory management", "Logistics platforms", "Quality management", "Microsoft Office" },
                ["it"] = new() { "Active Directory", "VMware", "Cisco", "Splunk", "ServiceNow", "AWS", "Microsoft Azure" }
            };

            return roleTools.GetValueOrDefault(roleId, roleTools["developer"]);
        }

        public async Task<Dictionary<string, string>> GetIndustryTerminologyAsync(string industry)
        {
            var terminology = new Dictionary<string, List<KeyValuePair<string, string>>>
            {
                ["Technology"] = new()
                {
                    new("TECH_TERM", "API"),
                    new("COMPLIANCE_FRAMEWORK", "SOC 2"),
                    new("DATA_TYPE", "user credentials"),
                    new("SYSTEM_TYPE", "cloud infrastructure")
                },
                ["Financial Services"] = new()
                {
                    new("FINANCE_TERM", "wire transfer"),
                    new("COMPLIANCE_FRAMEWORK", "PCI DSS"),
                    new("DATA_TYPE", "financial records"),
                    new("SYSTEM_TYPE", "trading platform")
                },
                ["Healthcare"] = new()
                {
                    new("HEALTH_TERM", "PHI"),
                    new("COMPLIANCE_FRAMEWORK", "HIPAA"),
                    new("DATA_TYPE", "patient records"),
                    new("SYSTEM_TYPE", "EHR system")
                }
            };

            var terms = terminology.GetValueOrDefault(industry, terminology["Technology"]);
            return terms.ToDictionary(t => t.Key, t => t.Value);
        }

        #endregion

        #region Analytics & Performance

        public async Task UpdateScenarioPerformanceAsync(string userId, string scenarioId, double score, TimeSpan completionTime)
        {
            try
            {
                var profile = await _userProfileService.GetUserProfileAsync(userId);
                if (profile == null) return;

                // Update scenario-specific metrics would go here
                // For now, we'll invalidate the cache to force refresh
                var cacheKey = string.Format(RECOMMENDATIONS_CACHE_KEY, userId);
                await _cache.RemoveAsync(cacheKey);
                
                var analyticsCacheKey = string.Format(SCENARIO_ANALYTICS_CACHE_KEY, userId);
                await _cache.RemoveAsync(analyticsCacheKey);

                _logger.LogInformation("Updated scenario performance for user {UserId}, scenario {ScenarioId}, score {Score}", 
                    userId, scenarioId, score);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating scenario performance for user {UserId}", userId);
            }
        }

        public async Task<List<PersonalizedRecommendation>> RefreshRecommendationsAsync(string userId)
        {
            try
            {
                // Clear cache to force regeneration
                var cacheKey = string.Format(RECOMMENDATIONS_CACHE_KEY, userId);
                await _cache.RemoveAsync(cacheKey);
                
                return await GeneratePersonalizedRecommendationsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing recommendations for user {UserId}", userId);
                return new List<PersonalizedRecommendation>();
            }
        }

        public async Task<PersonalizedRecommendation> AdaptScenarioDifficultyAsync(string userId, PersonalizedRecommendation recommendation)
        {
            try
            {
                var optimalDifficulty = await DetermineOptimalDifficultyAsync(userId, recommendation.ScenarioType);
                
                // Adjust the recommendation based on new difficulty
                recommendation.EstimatedDurationMinutes = optimalDifficulty switch
                {
                    "easy" => recommendation.EstimatedDurationMinutes - 5,
                    "challenging" => recommendation.EstimatedDurationMinutes + 5,
                    "expert" => recommendation.EstimatedDurationMinutes + 10,
                    _ => recommendation.EstimatedDurationMinutes
                };

                recommendation.Description += $" (Adapted for {optimalDifficulty} difficulty level)";
                
                return recommendation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adapting scenario difficulty for user {UserId}", userId);
                return recommendation;
            }
        }

        public async Task<ScenarioAnalytics> GetScenarioAnalyticsAsync(string userId)
        {
            try
            {
                var cacheKey = string.Format(SCENARIO_ANALYTICS_CACHE_KEY, userId);
                var cachedAnalytics = await _cache.GetStringAsync(cacheKey);
                
                if (!string.IsNullOrEmpty(cachedAnalytics))
                {
                    return JsonSerializer.Deserialize<ScenarioAnalytics>(cachedAnalytics) ?? new();
                }

                var profile = await _userProfileService.GetUserProfileAsync(userId);
                if (profile == null) return new ScenarioAnalytics { UserId = userId };

                var analytics = new ScenarioAnalytics
                {
                    UserId = userId,
                    OverallScore = profile.Progress.OverallAccuracy,
                    TotalScenariosCompleted = profile.Progress.TotalScenariosCompleted,
                    ScenarioTypeScores = profile.Progress.SkillLevels.ToDictionary(s => s.Key, s => s.Value.CurrentLevel),
                    PersonalizationScore = CalculatePersonalizationEffectiveness(profile),
                    LearningVelocity = CalculateLearningVelocity(profile)
                };

                // Analyze strengths and improvement areas
                analytics.StrengthAreas = profile.Progress.SkillLevels
                    .Where(s => s.Value.CurrentLevel >= 0.8)
                    .Select(s => FormatScenarioTypeName(s.Key))
                    .ToList();

                analytics.ImprovementAreas = profile.Progress.SkillLevels
                    .Where(s => s.Value.CurrentLevel < 0.6)
                    .Select(s => FormatScenarioTypeName(s.Key))
                    .ToList();

                // Cache analytics
                var serializedAnalytics = JsonSerializer.Serialize(analytics);
                await _cache.SetStringAsync(cacheKey, serializedAnalytics, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CACHE_EXPIRATION_HOURS)
                });

                return analytics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting scenario analytics for user {UserId}", userId);
                return new ScenarioAnalytics { UserId = userId };
            }
        }

        public async Task<List<string>> GetPersonalizedLearningInsightsAsync(string userId)
        {
            try
            {
                var analytics = await GetScenarioAnalyticsAsync(userId);
                var profile = await _userProfileService.GetUserProfileAsync(userId);
                var insights = new List<string>();

                if (profile == null) return insights;

                // Performance insights
                if (analytics.OverallScore >= 0.85)
                    insights.Add($"🎯 Excellent performance! You're in the top 10% of {profile.SelectedRoleId} professionals.");
                else if (analytics.OverallScore >= 0.70)
                    insights.Add($"👍 Good progress! You're performing above average for {profile.Industry} professionals.");
                else
                    insights.Add($"📈 Keep practicing! Focus on your improvement areas to boost your security awareness.");

                // Learning velocity insights
                if (analytics.LearningVelocity > 0.7)
                    insights.Add("🚀 You're learning quickly! Consider trying more challenging scenarios.");
                else if (analytics.LearningVelocity < 0.3)
                    insights.Add("🎯 Take your time to master each scenario before moving to the next level.");

                // Role-specific insights
                if (profile.SelectedRoleId == "developer" && analytics.ScenarioTypeScores.GetValueOrDefault("supply_chain", 0) < 0.6)
                    insights.Add("⚠️ Supply chain security is critical for developers. Focus on this area to protect your code repositories.");

                if (profile.SelectedRoleId == "finance" && analytics.ScenarioTypeScores.GetValueOrDefault("wire_fraud", 0) < 0.7)
                    insights.Add("💰 Wire fraud detection is essential for finance roles. Practice identifying BEC attacks.");

                // Improvement suggestions
                if (analytics.ImprovementAreas.Any())
                    insights.Add($"🎯 Focus areas: {string.Join(", ", analytics.ImprovementAreas.Take(2))}");

                return insights;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting personalized learning insights for user {UserId}", userId);
                return new List<string>();
            }
        }

        public async Task<Dictionary<string, double>> CalculateRoleProficiencyAsync(string userId)
        {
            try
            {
                var profile = await _userProfileService.GetUserProfileAsync(userId);
                if (profile == null) return new Dictionary<string, double>();

                var roleSkills = GetRequiredSkillsForRole(profile.SelectedRoleId ?? "general");
                var proficiency = new Dictionary<string, double>();

                foreach (var skill in roleSkills)
                {
                    var skillLevel = profile.Progress.SkillLevels.GetValueOrDefault(skill);
                    proficiency[skill] = skillLevel?.CurrentLevel ?? 0.0;
                }

                return proficiency;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating role proficiency for user {UserId}", userId);
                return new Dictionary<string, double>();
            }
        }

        #endregion

        #region Private Helper Methods

        private async Task<PersonalizedRecommendation> CreateRecommendationFromTemplateAsync(
            string userId, ScenarioTemplate template, Dictionary<string, string> context, UserProfile profile)
        {
            var recommendation = new PersonalizedRecommendation
            {
                UserId = userId,
                Title = await PersonalizeContentAsync(template.BaseTitle, context),
                Description = await PersonalizeContentAsync(template.BaseDescription, context),
                ScenarioType = template.ScenarioType,
                EstimatedDurationMinutes = CalculateEstimatedDuration(template, profile),
                CompanySpecificContext = context.GetValueOrDefault("COMPANY_NAME", "your organization"),
                RoleSpecificDetails = GenerateRoleSpecificDetails(template, profile),
                IndustryContext = context.GetValueOrDefault("INDUSTRY", "Technology"),
                PersonalizationFactors = BuildPersonalizationFactors(template, profile),
                Priority = DeterminePriority(template, profile)
            };

            recommendation.LearningOutcomes = await GenerateLearningObjectivesAsync(template, context, profile);
            recommendation.ReasonForRecommendation = GenerateReasonForRecommendation(template, profile);

            return recommendation;
        }

        private List<ScenarioTemplate> FilterTemplatesByUserContext(List<ScenarioTemplate> templates, UserProfile profile)
        {
            return templates
                .Where(t => t.ApplicableIndustries.Contains(profile.Industry) || 
                           t.ApplicableIndustries.Contains("All") ||
                           !t.ApplicableIndustries.Any())
                .OrderBy(t => GetDifficultyNumericValue(t.DifficultyLevel))
                .ToList();
        }

        private double CalculateRecommendationScore(PersonalizedRecommendation recommendation, UserProfile profile)
        {
            var score = 0.0;

            // Role relevance (40%)
            if (recommendation.ScenarioType == GetPrimaryThreatForRole(profile.SelectedRoleId))
                score += 0.4;

            // Experience level match (30%)
            var userExperienceValue = GetDifficultyNumericValue(profile.ExperienceLevel ?? "intermediate");
            var scenarioDifficulty = GetScenarioDifficulty(recommendation.ScenarioType);
            var difficultyMatch = 1.0 - Math.Abs(userExperienceValue - scenarioDifficulty);
            score += difficultyMatch * 0.3;

            // Industry relevance (20%)
            if (recommendation.IndustryContext == profile.Industry)
                score += 0.2;

            // Freshness and learning path position (10%)
            score += 0.1;

            return score;
        }

        private async Task<ScenarioContent> GeneratePersonalizedContentAsync(
            ScenarioTemplate template, Dictionary<string, string> context, UserProfile profile)
        {
            var content = new ScenarioContent();

            if (template.ContentTemplate.EmailSubjectTemplate != null)
            {
                content.EmailSubject = await PersonalizeContentAsync(template.ContentTemplate.EmailSubjectTemplate, context);
            }

            if (template.ContentTemplate.EmailSenderTemplate != null)
            {
                content.EmailSender = await PersonalizeContentAsync(template.ContentTemplate.EmailSenderTemplate, context);
            }

            if (template.ContentTemplate.EmailBodyTemplate != null)
            {
                content.EmailBody = await PersonalizeContentAsync(template.ContentTemplate.EmailBodyTemplate, context);
            }

            content.SuspiciousElements = new List<string>();
            foreach (var element in template.ContentTemplate.SuspiciousElementsTemplate)
            {
                content.SuspiciousElements.Add(await PersonalizeContentAsync(element, context));
            }

            content.RedFlags = new List<string>();
            foreach (var flag in template.ContentTemplate.RedFlagsTemplate)
            {
                content.RedFlags.Add(await PersonalizeContentAsync(flag, context));
            }

            if (template.ContentTemplate.CorrectActionTemplate != null)
            {
                content.CorrectAction = await PersonalizeContentAsync(template.ContentTemplate.CorrectActionTemplate, context);
            }

            if (template.ContentTemplate.ConsequenceTemplate != null)
            {
                content.IncorrectConsequence = await PersonalizeContentAsync(template.ContentTemplate.ConsequenceTemplate, context);
            }

            return content;
        }

        private async Task<List<string>> GenerateLearningObjectivesAsync(
            ScenarioTemplate template, Dictionary<string, string> context, UserProfile profile)
        {
            var objectives = new List<string>();

            switch (template.ScenarioType)
            {
                case "phishing":
                    objectives.Add($"Identify phishing attempts targeting {context.GetValueOrDefault("JOB_TITLE", "your role")}");
                    objectives.Add($"Recognize suspicious elements in {context.GetValueOrDefault("INDUSTRY", "business")} communications");
                    objectives.Add($"Practice safe email handling procedures for {context.GetValueOrDefault("COMPANY_NAME", "your organization")}");
                    break;
                case "social_engineering":
                    objectives.Add($"Detect social engineering tactics used against {context.GetValueOrDefault("DEPARTMENT", "your department")}");
                    objectives.Add($"Apply verification procedures for {context.GetValueOrDefault("INDUSTRY", "industry")}-specific requests");
                    break;
                default:
                    objectives.Add($"Enhance security awareness for {context.GetValueOrDefault("JOB_TITLE", "your role")}");
                    objectives.Add($"Protect {context.GetValueOrDefault("COMPANY_NAME", "organizational")} assets and data");
                    break;
            }

            return objectives;
        }

        private int CalculateEstimatedDuration(ScenarioTemplate template, UserProfile profile)
        {
            var baseDuration = 15; // minutes

            // Adjust based on difficulty
            baseDuration += template.DifficultyLevel switch
            {
                "easy" => -5,
                "challenging" => +5,
                "advanced" => +8,
                "expert" => +12,
                _ => 0
            };

            // Adjust based on user experience
            baseDuration += profile.ExperienceLevel switch
            {
                "beginner" => +5,
                "advanced" => -2,
                "expert" => -5,
                _ => 0
            };

            return Math.Max(5, Math.Min(30, baseDuration)); // Clamp between 5-30 minutes
        }

        private List<string> GenerateScenarioTags(ScenarioTemplate template, UserProfile profile)
        {
            var tags = new List<string>
            {
                template.ScenarioType,
                profile.SelectedRoleId ?? "general",
                profile.Industry ?? "technology",
                template.DifficultyLevel,
                "personalized"
            };

            return tags.Where(t => !string.IsNullOrEmpty(t)).ToList();
        }

        private string GenerateRoleSpecificDetails(ScenarioTemplate template, UserProfile profile)
        {
            return $"This scenario is specifically designed for {profile.JobTitle} professionals at {profile.Company}, " +
                   $"incorporating common {profile.Industry} industry practices and {profile.ExperienceLevel}-level challenges.";
        }

        private List<string> BuildPersonalizationFactors(ScenarioTemplate template, UserProfile profile)
        {
            return new List<string>
            {
                $"Role: {profile.SelectedRoleId}",
                $"Industry: {profile.Industry}",
                $"Experience: {profile.ExperienceLevel}",
                $"Company: {profile.Company}",
                $"Scenario Type: {template.ScenarioType}"
            };
        }

        private string DeterminePriority(ScenarioTemplate template, UserProfile profile)
        {
            // High priority for primary threats in user's role
            if (GetPrimaryThreatForRole(profile.SelectedRoleId) == template.ScenarioType)
                return "high";

            // Medium priority for secondary threats
            if (GetSecondaryThreatsForRole(profile.SelectedRoleId).Contains(template.ScenarioType))
                return "medium";

            return "low";
        }

        private string GenerateReasonForRecommendation(ScenarioTemplate template, UserProfile profile)
        {
            var reasons = new List<string>();

            if (GetPrimaryThreatForRole(profile.SelectedRoleId) == template.ScenarioType)
                reasons.Add($"Primary threat for {profile.SelectedRoleId} professionals");

            if (template.ApplicableIndustries.Contains(profile.Industry))
                reasons.Add($"Common in {profile.Industry} industry");

            if (reasons.Any())
                return string.Join(". ", reasons) + ".";

            return $"Relevant training for {profile.ExperienceLevel} level security awareness.";
        }

        private async Task CacheRecommendationsAsync(string userId, List<PersonalizedRecommendation> recommendations)
        {
            try
            {
                var cacheKey = string.Format(RECOMMENDATIONS_CACHE_KEY, userId);
                var serializedRecommendations = JsonSerializer.Serialize(recommendations);
                
                await _cache.SetStringAsync(cacheKey, serializedRecommendations, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CACHE_EXPIRATION_HOURS)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching recommendations for user {UserId}", userId);
            }
        }

        private async Task<string> GeneratePlaceholderValueAsync(string placeholder, Dictionary<string, string> context)
        {
            try
            {
                // Use AI to generate contextually appropriate values
                var prompt = $"Generate a realistic {placeholder.ToLower().Replace("_", " ")} " +
                           $"for a {context.GetValueOrDefault("INDUSTRY", "technology")} company " +
                           $"called {context.GetValueOrDefault("COMPANY_NAME", "the company")}. " +
                           $"Provide only the value, no explanation.";

                return await _semanticKernel.ExecutePromptAsync(prompt);
            }
            catch
            {
                // Fallback values if AI generation fails
                return placeholder switch
                {
                    "REPOSITORY_NAME" => "core-system",
                    "PROJECT_NAME" => "main-project",
                    "CLIENT_NAME" => "Important Client",
                    "VENDOR_NAME" => "Service Provider",
                    _ => $"[{placeholder}]"
                };
            }
        }

        // Additional helper methods for content generation
        private List<string> GeneratePhishingExamples(string companyName, string industry)
        {
            return new List<string>
            {
                $"Fake security alert claiming {companyName}'s {industry} systems are compromised",
                $"Impersonated CEO email requesting urgent wire transfer for {industry} acquisition",
                $"Fraudulent vendor invoice for {companyName} services with payment redirect"
            };
        }

        private List<string> GenerateSocialEngineeringExamples(string companyName, string industry)
        {
            return new List<string>
            {
                $"Phone call impersonating IT support requesting {companyName} credentials",
                $"Fake {industry} compliance audit requiring immediate data access",
                $"Pretexting attack claiming to be from {companyName}'s insurance provider"
            };
        }

        private List<string> GenerateMalwareExamples(string companyName, string industry)
        {
            return new List<string>
            {
                $"Malicious attachment disguised as {companyName} policy update",
                $"Infected software download targeting {industry} professionals",
                $"Compromised website hosting {industry}-specific resources"
            };
        }

        private List<string> GenerateDataBreachExamples(string companyName, string industry)
        {
            return new List<string>
            {
                $"Unauthorized access to {companyName}'s {industry} customer database",
                $"Insider threat exposing {companyName}'s confidential {industry} data",
                $"Third-party breach affecting {companyName}'s {industry} partnerships"
            };
        }

        private List<string> GenerateGeneralExamples(string companyName, string industry)
        {
            return new List<string>
            {
                $"Generic security threat targeting {companyName}",
                $"Common {industry} security vulnerability",
                $"Standard social engineering attempt"
            };
        }

        // Utility methods
        private string GetCompanySizeDescription(string? companySize)
        {
            return companySize switch
            {
                "startup" => "startup",
                "small" => "small business",
                "medium" => "mid-size company",
                "large" => "large enterprise",
                "enterprise" => "global enterprise",
                _ => "organization"
            };
        }

        private string ExtractEmailDomain(string email)
        {
            try
            {
                var parts = email.Split('@');
                return parts.Length > 1 ? parts[1] : "company.com";
            }
            catch
            {
                return "company.com";
            }
        }

        private string ExtractDepartmentFromRole(string? roleId)
        {
            return roleId switch
            {
                "developer" => "Engineering",
                "marketing" => "Marketing",
                "hr" => "Human Resources",
                "finance" => "Finance",
                "sales" => "Sales",
                "executive" => "Executive",
                "operations" => "Operations",
                "it" => "Information Technology",
                _ => "General"
            };
        }

        private string GenerateRepositoryName(string companyName, string? industry)
        {
            var prefix = companyName.ToLower().Replace(" ", "-").Replace(".", "");
            var suffix = industry switch
            {
                "Technology" => "core-api",
                "Financial Services" => "trading-platform",
                "Healthcare" => "patient-portal",
                "E-commerce" => "web-store",
                _ => "main-app"
            };
            return $"{prefix}-{suffix}";
        }

        private string GenerateProjectName(string? industry)
        {
            return industry switch
            {
                "Technology" => "CloudMigration2024",
                "Financial Services" => "TradingUpgrade",
                "Healthcare" => "PatientPortalV2",
                "Manufacturing" => "SupplyChainOpt",
                _ => "ProjectAlpha"
            };
        }

        private string GenerateClientName(string? industry)
        {
            return industry switch
            {
                "Technology" => "TechCorp Solutions",
                "Financial Services" => "Global Investment Partners",
                "Healthcare" => "Regional Medical Center",
                "Manufacturing" => "Industrial Systems Inc",
                _ => "Strategic Business Partner"
            };
        }

        private string GenerateVendorName(string? industry)
        {
            return industry switch
            {
                "Technology" => "CloudTech Services",
                "Financial Services" => "Financial Data Solutions",
                "Healthcare" => "MedTech Support",
                "Manufacturing" => "Industrial Equipment Co",
                _ => "Business Solutions Provider"
            };
        }

        private double GetDifficultyNumericValue(string difficulty)
        {
            return difficulty switch
            {
                "beginner" or "easy" => 0.2,
                "intermediate" => 0.5,
                "advanced" or "challenging" => 0.8,
                "expert" => 1.0,
                _ => 0.5
            };
        }

        private string GetPrimaryThreatForRole(string? roleId)
        {
            return roleId switch
            {
                "developer" => "phishing",
                "marketing" => "social_engineering",
                "hr" => "social_engineering",
                "finance" => "wire_fraud",
                "sales" => "client_impersonation",
                "executive" => "strategic_theft",
                "operations" => "supply_chain",
                "it" => "privilege_escalation",
                _ => "phishing"
            };
        }

        private List<string> GetSecondaryThreatsForRole(string? roleId)
        {
            return roleId switch
            {
                "developer" => new() { "supply_chain", "code_review" },
                "marketing" => new() { "invoice_fraud", "account_takeover" },
                "hr" => new() { "resume_malware", "payroll_phishing" },
                "finance" => new() { "payment_fraud", "invoice_fraud" },
                "sales" => new() { "phishing", "social_engineering" },
                "executive" => new() { "phishing", "social_engineering" },
                "operations" => new() { "vendor_impersonation", "phishing" },
                "it" => new() { "phishing", "malware" },
                _ => new() { "social_engineering", "malware" }
            };
        }

        private double GetScenarioDifficulty(string scenarioType)
        {
            return scenarioType switch
            {
                "phishing" => 0.3,
                "social_engineering" => 0.6,
                "wire_fraud" => 0.8,
                "strategic_theft" => 1.0,
                "privilege_escalation" => 1.0,
                _ => 0.5
            };
        }

        private double CalculatePersonalizationEffectiveness(UserProfile profile)
        {
            var score = 0.0;
            
            // Score based on data completeness
            if (!string.IsNullOrEmpty(profile.Company)) score += 0.2;
            if (!string.IsNullOrEmpty(profile.Industry)) score += 0.2;
            if (!string.IsNullOrEmpty(profile.SelectedRoleId)) score += 0.3;
            if (profile.SelectedTools.Any()) score += 0.2;
            if (!string.IsNullOrEmpty(profile.ExperienceLevel)) score += 0.1;
            
            return score;
        }

        private double CalculateLearningVelocity(UserProfile profile)
        {
            if (profile.Progress.RecentSessions.Count < 2) return 0.5;
            
            var sessions = profile.Progress.RecentSessions.OrderBy(s => s.StartTime).ToList();
            var improvements = 0.0;
            var count = 0;
            
            for (int i = 1; i < sessions.Count; i++)
            {
                if (sessions[i].TrainingType == sessions[i-1].TrainingType)
                {
                    improvements += sessions[i].AccuracyScore - sessions[i-1].AccuracyScore;
                    count++;
                }
            }
            
            return count > 0 ? Math.Max(0, Math.Min(1, improvements / count + 0.5)) : 0.5;
        }

        private string FormatScenarioTypeName(string scenarioType)
        {
            return scenarioType switch
            {
                "phishing" => "Email Phishing",
                "social_engineering" => "Social Engineering",
                "wire_fraud" => "Wire Fraud",
                "invoice_fraud" => "Invoice Fraud",
                "account_takeover" => "Account Takeover",
                "supply_chain" => "Supply Chain Attacks",
                "privilege_escalation" => "Privilege Escalation",
                "strategic_theft" => "Strategic Information Theft",
                _ => scenarioType.Replace("_", " ").ToTitleCase()
            };
        }

        private List<string> GetRequiredSkillsForRole(string roleId)
        {
            return roleId switch
            {
                "developer" => new() { "phishing", "supply_chain", "code_review", "malware" },
                "marketing" => new() { "social_engineering", "invoice_fraud", "account_takeover", "phishing" },
                "hr" => new() { "social_engineering", "resume_malware", "payroll_phishing", "phishing" },
                "finance" => new() { "wire_fraud", "payment_fraud", "invoice_fraud", "phishing" },
                "sales" => new() { "client_impersonation", "phishing", "social_engineering" },
                "executive" => new() { "strategic_theft", "phishing", "social_engineering", "wire_fraud" },
                "operations" => new() { "supply_chain", "vendor_impersonation", "phishing" },
                "it" => new() { "privilege_escalation", "malware", "phishing", "supply_chain" },
                _ => new() { "phishing", "social_engineering", "malware" }
            };
        }

        #endregion
    }

    // Extension method for string formatting
    public static class StringExtensions
    {
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
            
            return string.Join(" ", words);
        }
    }
}