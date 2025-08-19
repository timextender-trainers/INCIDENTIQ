using IncidentIQ.Application.Interfaces;
using IncidentIQ.Application.Interfaces.AI;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace IncidentIQ.Infrastructure.Services;

public class SessionEvaluationService : ISessionEvaluationService
{
    private readonly IClaudeApiService _claudeApiService;
    private readonly ISemanticKernelService _kernelService;
    private readonly ILogger<SessionEvaluationService> _logger;

    public SessionEvaluationService(
        IClaudeApiService claudeApiService,
        ISemanticKernelService kernelService,
        ILogger<SessionEvaluationService> logger)
    {
        _claudeApiService = claudeApiService;
        _kernelService = kernelService;
        _logger = logger;
    }

    public async Task<SessionEvaluationResult> EvaluateSessionAsync(PhoneCallSession session)
    {
        try
        {
            _logger.LogInformation("Starting AI evaluation for session {SessionId}", session.Id);
            
            var conversationText = BuildConversationText(session);
            var prompt = BuildEvaluationPrompt(session, conversationText);
            
            // Try Claude first, fallback to OpenAI, then hardcoded
            string evaluationResponse = "";
            
            try
            {
                if (await _claudeApiService.IsApiAvailableAsync())
                {
                    // Create a temporary session for the evaluation
                    var tempSession = new PhoneCallSession { Exchanges = session.Exchanges.ToList() };
                    evaluationResponse = await _claudeApiService.GeneratePhishingResponseAsync(tempSession, prompt, new List<string>());
                    _logger.LogInformation("Used Claude for session evaluation");
                }
                else
                {
                    throw new Exception("Claude API not available");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Claude evaluation failed, trying OpenAI");
                
                var arguments = new KernelArguments
                {
                    ["conversationText"] = conversationText,
                    ["sessionId"] = session.Id.ToString()
                };
                
                evaluationResponse = await _kernelService.ExecutePromptAsync(prompt, arguments);
                _logger.LogInformation("Used OpenAI for session evaluation");
            }

            // Parse the AI response and create evaluation result
            var result = await ParseEvaluationResponseAsync(evaluationResponse, session);
            
            _logger.LogInformation("Completed AI evaluation for session {SessionId} with score {Score}", 
                session.Id, result.SecurityScore);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during AI evaluation, using fallback for session {SessionId}", session.Id);
            return CreateFallbackEvaluation(session);
        }
    }

    public async Task<List<SecurityBreach>> AnalyzeSecurityBreachesAsync(List<ConversationExchange> exchanges)
    {
        var breaches = new List<SecurityBreach>();
        
        for (int i = 0; i < exchanges.Count; i++)
        {
            var exchange = exchanges[i];
            var userResponse = exchange.UserResponse?.ToLower() ?? "";
            
            // Analyze common security breaches
            if (userResponse.Contains("right away") || userResponse.Contains("immediately") || userResponse.Contains("sure"))
            {
                breaches.Add(new SecurityBreach
                {
                    BreachType = "Urgency Compliance",
                    Description = "User agreed to urgent request without proper verification",
                    UserResponse = exchange.UserResponse ?? "",
                    TurnNumber = i + 1,
                    Severity = RiskLevel.High,
                    ImpactExplanation = "Bypassing security protocols due to perceived urgency can lead to unauthorized access",
                    PreventionAdvice = "Always follow verification procedures regardless of claimed urgency"
                });
            }
            
            if (userResponse.Contains("update") && !userResponse.Contains("verify") && !userResponse.Contains("security"))
            {
                breaches.Add(new SecurityBreach
                {
                    BreachType = "Information Modification Without Verification",
                    Description = "User offered to update information without proper identity verification",
                    UserResponse = exchange.UserResponse ?? "",
                    TurnNumber = i + 1,
                    Severity = RiskLevel.Critical,
                    ImpactExplanation = "Allowing account modifications without verification can lead to account takeover",
                    PreventionAdvice = "Always verify caller identity before making any account changes"
                });
            }
            
            if (userResponse.Contains("help") && !userResponse.Contains("verify") && !userResponse.Contains("security"))
            {
                breaches.Add(new SecurityBreach
                {
                    BreachType = "Overly Helpful Response",
                    Description = "User offered immediate assistance without security checks",
                    UserResponse = exchange.UserResponse ?? "",
                    TurnNumber = i + 1,
                    Severity = RiskLevel.Medium,
                    ImpactExplanation = "Being too helpful can bypass necessary security measures",
                    PreventionAdvice = "Balance helpfulness with security procedures"
                });
            }
        }
        
        return breaches;
    }

    public async Task<List<SecurityRecommendation>> GenerateDetailedRecommendationsAsync(SessionEvaluationResult evaluation)
    {
        var recommendations = new List<SecurityRecommendation>();
        
        // Generate recommendations based on security breaches
        foreach (var breach in evaluation.SecurityBreaches)
        {
            switch (breach.BreachType)
            {
                case "Urgency Compliance":
                    recommendations.Add(new SecurityRecommendation
                    {
                        Title = "Resist Urgency Pressure",
                        Description = "You were influenced by urgency tactics used by the caller",
                        ActionableAdvice = "When someone claims urgency, take a moment to follow standard verification procedures. Genuine emergencies can wait for proper security checks.",
                        Priority = RecommendationPriority.High,
                        RoleSpecificContext = "As customer service, you'll often face urgent requests. Your job is to help securely, not quickly."
                    });
                    break;
                    
                case "Information Modification Without Verification":
                    recommendations.Add(new SecurityRecommendation
                    {
                        Title = "Always Verify Before Modifying",
                        Description = "Account changes were offered without proper identity verification",
                        ActionableAdvice = "Never modify account information without completing full identity verification through approved security questions or callback procedures.",
                        Priority = RecommendationPriority.Critical,
                        RoleSpecificContext = "Account modifications are high-risk activities that require the strongest verification measures."
                    });
                    break;
                    
                case "Overly Helpful Response":
                    recommendations.Add(new SecurityRecommendation
                    {
                        Title = "Balance Helpfulness with Security",
                        Description = "Your helpful nature was exploited by the social engineer",
                        ActionableAdvice = "Maintain professional helpfulness while consistently applying security procedures. Being secure is part of being helpful.",
                        Priority = RecommendationPriority.Medium,
                        RoleSpecificContext = "Customer service excellence includes protecting customers through proper security practices."
                    });
                    break;
            }
        }
        
        // Add general recommendations if few breaches occurred
        if (evaluation.SecurityBreaches.Count <= 1)
        {
            recommendations.Add(new SecurityRecommendation
            {
                Title = "Strong Security Awareness",
                Description = "You demonstrated good resistance to social engineering tactics",
                ActionableAdvice = "Continue to maintain this security-first mindset and help train colleagues on these techniques.",
                Priority = RecommendationPriority.Low,
                RoleSpecificContext = "Your security awareness is a valuable asset to your organization."
            });
        }
        
        return recommendations;
    }

    private string BuildConversationText(PhoneCallSession session)
    {
        var conversationParts = new List<string>();
        
        // Add scenario context
        if (session.Scenario != null)
        {
            conversationParts.Add($"SCENARIO: {session.Scenario.Description}");
            conversationParts.Add($"TARGET ROLE: {session.Scenario.TargetRole}");
            conversationParts.Add($"CALLER PROFILE: {session.Scenario.CallerProfile?.Name} from {session.Scenario.CallerProfile?.Company}");
        }
        
        conversationParts.Add("\nCONVERSATION:");
        
        // Add initial message
        conversationParts.Add("CALLER: Hello, this is Jennifer from CustomerCorp. I need immediate access to update my account - we have a major client presentation in 10 minutes and I can't log in!");
        
        // Add all exchanges
        foreach (var exchange in session.Exchanges.OrderBy(e => e.Timestamp))
        {
            if (!string.IsNullOrEmpty(exchange.UserResponse))
                conversationParts.Add($"USER: {exchange.UserResponse}");
            
            if (!string.IsNullOrEmpty(exchange.HackerMessage))
                conversationParts.Add($"CALLER: {exchange.HackerMessage}");
        }
        
        return string.Join("\n", conversationParts);
    }

    private string BuildEvaluationPrompt(PhoneCallSession session, string conversationText)
    {
        return $@"You are a cybersecurity expert analyzing a phone-based social engineering training simulation. 

CONVERSATION TO ANALYZE:
{conversationText}

Please provide a comprehensive evaluation in the following JSON format:

{{
  ""securityScore"": 0-100,
  ""overallPerformance"": 0-100,
  ""summaryFeedback"": ""Brief overall assessment"",
  ""keyStrengths"": [""Strength 1"", ""Strength 2"", ""Strength 3""],
  ""growthAreas"": [""Area 1"", ""Area 2"", ""Area 3""],
  ""futureLearnings"": [
    {{
      ""title"": ""Learning topic title"",
      ""description"": ""Why this learning is important for the user"",
      ""estimatedTime"": ""15 minutes"",
      ""resourceType"": ""Interactive Training/Video/Article/Quiz"",
      ""priority"": ""High/Medium/Low""
    }}
  ],
  ""riskAssessment"": {{
    ""overallRiskLevel"": ""Low/Medium/High/Critical"",
    ""primaryVulnerabilities"": [""vulnerability1"", ""vulnerability2""],
    ""phishingResistanceScore"": 0-100,
    ""riskProfile"": ""Description of user's risk profile""
  }},
  ""securityBreaches"": [
    {{
      ""breachType"": ""Type of breach"",
      ""description"": ""What happened"",
      ""userResponse"": ""Specific user response"",
      ""turnNumber"": 1,
      ""severity"": ""Low/Medium/High/Critical"",
      ""impactExplanation"": ""Why this is risky"",
      ""preventionAdvice"": ""How to prevent this""
    }}
  ],
  ""tacticAnalysis"": [
    {{
      ""tactic"": ""Urgency/Authority/Fear/etc"",
      ""wasSuccessful"": true/false,
      ""howItWorked"": ""Explanation"",
      ""userVulnerability"": ""What made user susceptible"",
      ""counterStrategy"": ""How to resist this tactic""
    }}
  ],
  ""recommendations"": [
    {{
      ""title"": ""Recommendation title"",
      ""description"": ""What they need to improve"",
      ""actionableAdvice"": ""Specific steps to take"",
      ""priority"": ""Low/Medium/High/Critical"",
      ""roleSpecificContext"": ""How this applies to their role""
    }}
  ]
}}

Focus on:
1. Which security protocols were bypassed or properly followed
2. How social engineering tactics succeeded or failed
3. Specific vulnerabilities in the user's responses
4. Key strengths and positive security behaviors
5. Growth areas with specific improvement suggestions
6. Future learning recommendations based on performance gaps
7. Overall security posture assessment

Provide detailed, constructive feedback that helps the user understand both what they did well and how to improve.";
    }

    private async Task<SessionEvaluationResult> ParseEvaluationResponseAsync(string evaluationResponse, PhoneCallSession session)
    {
        try
        {
            // Try to parse JSON response
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(evaluationResponse);
            
            var result = new SessionEvaluationResult
            {
                SessionId = session.Id,
                SecurityScore = GetJsonDouble(jsonResponse, "securityScore"),
                OverallPerformance = GetJsonDouble(jsonResponse, "overallPerformance"),
                SummaryFeedback = GetJsonString(jsonResponse, "summaryFeedback"),
                KeyStrengths = GetJsonStringArray(jsonResponse, "keyStrengths"),
                GrowthAreas = GetJsonStringArray(jsonResponse, "growthAreas")
            };
            
            // Parse risk assessment
            if (jsonResponse.TryGetProperty("riskAssessment", out var riskElement))
            {
                result.RiskAssessment = new IncidentIQ.Application.Interfaces.RiskAssessment
                {
                    OverallRiskLevel = ParseRiskLevel(GetJsonString(riskElement, "overallRiskLevel")),
                    PrimaryVulnerabilities = GetJsonStringArray(riskElement, "primaryVulnerabilities"),
                    PhishingResistanceScore = GetJsonDouble(riskElement, "phishingResistanceScore"),
                    RiskProfile = GetJsonString(riskElement, "riskProfile")
                };
            }
            
            // Parse security breaches
            if (jsonResponse.TryGetProperty("securityBreaches", out var breachesElement) && breachesElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var breach in breachesElement.EnumerateArray())
                {
                    result.SecurityBreaches.Add(new SecurityBreach
                    {
                        BreachType = GetJsonString(breach, "breachType"),
                        Description = GetJsonString(breach, "description"),
                        UserResponse = GetJsonString(breach, "userResponse"),
                        TurnNumber = GetJsonInt(breach, "turnNumber"),
                        Severity = ParseRiskLevel(GetJsonString(breach, "severity")),
                        ImpactExplanation = GetJsonString(breach, "impactExplanation"),
                        PreventionAdvice = GetJsonString(breach, "preventionAdvice")
                    });
                }
            }
            
            // Parse recommendations
            if (jsonResponse.TryGetProperty("recommendations", out var recsElement) && recsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var rec in recsElement.EnumerateArray())
                {
                    result.Recommendations.Add(new SecurityRecommendation
                    {
                        Title = GetJsonString(rec, "title"),
                        Description = GetJsonString(rec, "description"),
                        ActionableAdvice = GetJsonString(rec, "actionableAdvice"),
                        Priority = ParseRecommendationPriority(GetJsonString(rec, "priority")),
                        RoleSpecificContext = GetJsonString(rec, "roleSpecificContext")
                    });
                }
            }
            
            // Parse future learnings
            if (jsonResponse.TryGetProperty("futureLearnings", out var learningsElement) && learningsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var learning in learningsElement.EnumerateArray())
                {
                    result.FutureLearnings.Add(new FutureLearning
                    {
                        Title = GetJsonString(learning, "title"),
                        Description = GetJsonString(learning, "description"),
                        EstimatedTime = GetJsonString(learning, "estimatedTime"),
                        ResourceType = GetJsonString(learning, "resourceType"),
                        Priority = GetJsonString(learning, "priority")
                    });
                }
            }
            
            // Calculate training metrics
            result.Metrics = CalculateTrainingMetrics(session, result.SecurityScore);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse AI evaluation response, using fallback");
            return CreateFallbackEvaluation(session);
        }
    }

    private SessionEvaluationResult CreateFallbackEvaluation(PhoneCallSession session)
    {
        var securityBreaches = AnalyzeSecurityBreachesAsync(session.Exchanges.ToList()).Result;
        var securityScore = Math.Max(0, 100 - (securityBreaches.Count * 20));
        
        var result = new SessionEvaluationResult
        {
            SessionId = session.Id,
            SecurityScore = securityScore,
            OverallPerformance = securityScore,
            SummaryFeedback = securityScore >= 70 
                ? "Good security awareness demonstrated. Continue following best practices."
                : "Several security concerns identified. Additional training recommended.",
            SecurityBreaches = securityBreaches,
            RiskAssessment = new IncidentIQ.Application.Interfaces.RiskAssessment
            {
                OverallRiskLevel = securityScore >= 70 ? RiskLevel.Low : securityScore >= 50 ? RiskLevel.Medium : RiskLevel.High,
                PhishingResistanceScore = securityScore,
                RiskProfile = "Evaluation completed with limited AI analysis"
            },
            Recommendations = GenerateDetailedRecommendationsAsync(new SessionEvaluationResult { SecurityBreaches = securityBreaches }).Result,
            KeyStrengths = GenerateFallbackStrengths(session, securityBreaches),
            GrowthAreas = GenerateFallbackGrowthAreas(securityBreaches),
            FutureLearnings = GenerateFallbackFutureLearnings(securityBreaches),
            Metrics = CalculateTrainingMetrics(session, securityScore)
        };
        
        return result;
    }

    private string GetJsonString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) ? prop.GetString() ?? "" : "";
    }

    private double GetJsonDouble(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) ? prop.GetDouble() : 0.0;
    }

    private int GetJsonInt(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) ? prop.GetInt32() : 0;
    }

    private List<string> GetJsonStringArray(JsonElement element, string propertyName)
    {
        var list = new List<string>();
        if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in prop.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                    list.Add(item.GetString() ?? "");
            }
        }
        return list;
    }

    private RiskLevel ParseRiskLevel(string level)
    {
        return level.ToLower() switch
        {
            "low" => RiskLevel.Low,
            "medium" => RiskLevel.Medium,
            "high" => RiskLevel.High,
            "critical" => RiskLevel.Critical,
            _ => RiskLevel.Medium
        };
    }

    private RecommendationPriority ParseRecommendationPriority(string priority)
    {
        return priority.ToLower() switch
        {
            "low" => RecommendationPriority.Low,
            "medium" => RecommendationPriority.Medium,
            "high" => RecommendationPriority.High,
            "critical" => RecommendationPriority.Critical,
            _ => RecommendationPriority.Medium
        };
    }

    private TrainingMetrics CalculateTrainingMetrics(PhoneCallSession session, double securityScore)
    {
        var completionTime = (session.CallEndedAt != null && session.CallStartedAt != null) 
            ? session.CallEndedAt.Value.Subtract(session.CallStartedAt.Value) 
            : TimeSpan.Zero;
        var threatCount = session.AlertsTriggered?.Count ?? 0;
        var exchangeCount = session.Exchanges?.Count ?? 0;
        
        // Calculate grade based on security score
        string grade = securityScore switch
        {
            >= 90 => "A+",
            >= 85 => "A",
            >= 80 => "A-",
            >= 75 => "B+",
            >= 70 => "B",
            >= 65 => "B-",
            >= 60 => "C+",
            >= 55 => "C",
            >= 50 => "C-",
            >= 40 => "D",
            _ => "F"
        };

        return new TrainingMetrics
        {
            CompletionTime = completionTime,
            ThreatsDetected = threatCount,
            TotalExchanges = exchangeCount,
            ResponseTime = exchangeCount > 0 ? completionTime.TotalSeconds / exchangeCount : 0,
            Grade = grade
        };
    }

    private List<string> GenerateFallbackStrengths(PhoneCallSession session, List<SecurityBreach> breaches)
    {
        var strengths = new List<string>();
        
        // Analyze positive behaviors from exchanges
        var exchanges = session.Exchanges.ToList();
        var hasVerificationAttempt = exchanges.Any(e => e.UserResponse?.ToLower().Contains("verify") == true);
        var hasSecurityMention = exchanges.Any(e => e.UserResponse?.ToLower().Contains("security") == true);
        var hasSupervisorEscalation = exchanges.Any(e => e.UserResponse?.ToLower().Contains("supervisor") == true);
        
        if (hasVerificationAttempt)
            strengths.Add("Attempted identity verification");
        if (hasSecurityMention)
            strengths.Add("Demonstrated security awareness");
        if (hasSupervisorEscalation)
            strengths.Add("Proper escalation procedures");
        if (breaches.Count <= 1)
            strengths.Add("Strong resistance to manipulation tactics");
        if (exchanges.Count >= 4)
            strengths.Add("Maintained professional engagement throughout call");
        
        if (strengths.Count == 0)
            strengths.Add("Completed training session");
            
        return strengths;
    }

    private List<string> GenerateFallbackGrowthAreas(List<SecurityBreach> breaches)
    {
        var growthAreas = new List<string>();
        
        foreach (var breach in breaches)
        {
            switch (breach.BreachType)
            {
                case "Urgency Compliance":
                    growthAreas.Add("Resist urgency pressure tactics");
                    break;
                case "Information Modification Without Verification":
                    growthAreas.Add("Always verify before account changes");
                    break;
                case "Overly Helpful Response":
                    growthAreas.Add("Balance helpfulness with security protocols");
                    break;
            }
        }
        
        if (growthAreas.Count == 0)
        {
            growthAreas.Add("Continue practicing verification procedures");
            growthAreas.Add("Stay alert to new social engineering tactics");
        }
        
        return growthAreas;
    }

    private List<FutureLearning> GenerateFallbackFutureLearnings(List<SecurityBreach> breaches)
    {
        var learnings = new List<FutureLearning>();
        
        if (breaches.Any(b => b.BreachType.Contains("Urgency")))
        {
            learnings.Add(new FutureLearning
            {
                Title = "Advanced Urgency Tactics Recognition",
                Description = "Learn to identify and counter sophisticated urgency-based manipulation",
                EstimatedTime = "20 minutes",
                ResourceType = "Interactive Training",
                Priority = "High"
            });
        }
        
        if (breaches.Any(b => b.BreachType.Contains("Verification")))
        {
            learnings.Add(new FutureLearning
            {
                Title = "Customer Verification Best Practices",
                Description = "Master secure customer verification procedures",
                EstimatedTime = "15 minutes",
                ResourceType = "Video",
                Priority = "High"
            });
        }
        
        learnings.Add(new FutureLearning
        {
            Title = "Social Engineering Attack Patterns",
            Description = "Understand common social engineering attack patterns and defense strategies",
            EstimatedTime = "25 minutes",
            ResourceType = "Quiz",
            Priority = "Medium"
        });
        
        return learnings;
    }
}