using IncidentIQ.Application.Interfaces;
using IncidentIQ.Application.Interfaces.AI;
using IncidentIQ.Application.Interfaces.AI.Agents;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace IncidentIQ.Infrastructure.AI.Agents;

public class CoachingAgent : ICoachingAgent
{
    private readonly ISemanticKernelService _kernelService;
    private readonly ILogger<CoachingAgent> _logger;

    public CoachingAgent(ISemanticKernelService kernelService, ILogger<CoachingAgent> logger)
    {
        _kernelService = kernelService;
        _logger = logger;
    }

    public async Task<CoachingResponse> ProvideGuidanceAsync(CoachingRequest request)
    {
        try
        {
            var prompt = BuildCoachingPrompt(request);
            var arguments = new KernelArguments
            {
                ["userName"] = $"{request.User.FirstName} {request.User.LastName}",
                ["userRole"] = request.User.Role.ToString(),
                ["company"] = request.User.Company,
                ["securityLevel"] = request.User.SecurityLevel.ToString(),
                ["coachingType"] = request.RequestedType.ToString(),
                ["context"] = request.Context,
                ["question"] = request.CurrentDecisionPoint.Question,
                ["options"] = JsonSerializer.Serialize(request.CurrentDecisionPoint.Options)
            };

            var result = await _kernelService.ExecutePromptAsync(prompt, arguments);
            
            return ParseCoachingResponse(result, request.RequestedType, request.Context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error providing coaching guidance for user {UserId}", request.User.Id);
            return CreateFallbackCoachingResponse(request.RequestedType);
        }
    }

    public async Task<string> GenerateHintAsync(DecisionPoint decisionPoint, User user, string userContext)
    {
        var prompt = $"""
        Provide a subtle hint for this cybersecurity decision without giving away the answer:

        User: {user.FirstName} ({user.Role} at {user.Company})
        Security Level: {user.SecurityLevel}
        Question: {decisionPoint.Question}
        Options: {JsonSerializer.Serialize(decisionPoint.Options)}
        Context: {userContext}

        Guidelines:
        - Give a helpful hint that guides thinking without revealing the correct answer
        - Tailor the hint to their role and security level
        - Keep it encouraging and educational
        - Focus on the reasoning process, not the specific answer

        Provide only the hint text, no additional formatting.
        """;

        try
        {
            return await _kernelService.ExecutePromptAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate hint, using fallback");
            return "Think about what the security best practices would recommend in this situation. Consider the potential risks of each option.";
        }
    }

    public async Task<string> GenerateEncouragementAsync(User user, SessionScoring currentScore)
    {
        var prompt = $"""
        Generate encouraging feedback for this cybersecurity training session:

        User: {user.FirstName} ({user.Role})
        Current Score: {currentScore.OverallScore:P0}
        Correct Responses: {currentScore.CorrectResponses}/{currentScore.TotalResponses}
        Strength Areas: {JsonSerializer.Serialize(currentScore.StrengthAreas)}

        Create personalized, encouraging feedback that:
        - Acknowledges their progress and effort
        - Highlights their strengths
        - Motivates continued learning
        - Relates to their specific role and achievements

        Keep it positive, professional, and motivating. Provide only the encouragement text.
        """;

        try
        {
            return await _kernelService.ExecutePromptAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate encouragement, using fallback");
            return $"Great job so far, {user.FirstName}! You're making excellent progress in strengthening your cybersecurity awareness.";
        }
    }

    public async Task<string> ExplainConsequenceAsync(UserResponse response, DecisionPoint decisionPoint)
    {
        var prompt = $"""
        Explain the consequences of this cybersecurity decision:

        Question: {decisionPoint.Question}
        User's Choice: {response.SelectedOption}
        Correct Answer: {decisionPoint.Options[decisionPoint.CorrectOptionIndex]}
        Was Correct: {response.IsCorrect}

        Provide:
        1. Clear explanation of what would happen with their choice
        2. Why the correct answer is better (if they chose incorrectly)
        3. Real-world implications and risks
        4. Constructive guidance for future similar situations

        Be educational, not judgmental. Focus on learning and improvement.
        """;

        try
        {
            return await _kernelService.ExecutePromptAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate consequence explanation, using fallback");
            if (response.IsCorrect)
            {
                return "Excellent choice! This decision follows security best practices and helps protect your organization from potential threats.";
            }
            else
            {
                return $"This choice could lead to security risks. The recommended approach would be: {decisionPoint.Options[decisionPoint.CorrectOptionIndex]}";
            }
        }
    }

    private static string BuildCoachingPrompt(CoachingRequest request)
    {
        var basePrompt = $"""
        You are an expert cybersecurity coach providing {request.RequestedType} to a learner.

        Learner Profile:
        - Name: {request.User.FirstName} {request.User.LastName}
        - Role: {request.User.Role}
        - Company: {request.User.Company}
        - Security Level: {request.User.SecurityLevel}

        Current Situation:
        - Question: {request.CurrentDecisionPoint.Question}
        - Available Options: {JsonSerializer.Serialize(request.CurrentDecisionPoint.Options)}
        - Context: {request.Context}
        """;

        return request.RequestedType switch
        {
            CoachingType.Hint => basePrompt + """

                Provide a helpful hint that guides their thinking without revealing the answer.
                Focus on the reasoning process and security principles.
                """,

            CoachingType.Warning => basePrompt + """

                Provide a warning about potential security risks they might be overlooking.
                Help them identify red flags or concerning elements in the scenario.
                """,

            CoachingType.Encouragement => basePrompt + """

                Provide encouraging, motivational guidance.
                Acknowledge their effort and help build confidence.
                """,

            CoachingType.Explanation => basePrompt + """

                Provide a clear explanation of the security concepts involved.
                Help them understand the reasoning behind security decisions.
                """,

            CoachingType.BestPractice => basePrompt + """

                Share relevant security best practices and guidelines.
                Connect the current scenario to established security principles.
                """,

            CoachingType.RealWorldExample => basePrompt + """

                Provide a relevant real-world example or case study.
                Help them understand how this applies to actual security incidents.
                """,

            _ => basePrompt + "Provide helpful, contextual guidance."
        };
    }

    private CoachingResponse ParseCoachingResponse(string response, CoachingType type, string context)
    {
        return new CoachingResponse
        {
            Message = response.Trim(),
            Type = type,
            Context = context,
            ConfidenceScore = 0.85, // Could be enhanced with actual confidence scoring
            Metadata = new Dictionary<string, object>
            {
                ["timestamp"] = DateTime.UtcNow,
                ["agentVersion"] = "1.0"
            }
        };
    }

    private CoachingResponse CreateFallbackCoachingResponse(CoachingType type)
    {
        var message = type switch
        {
            CoachingType.Hint => "Consider the security implications of each option. What would best protect your organization?",
            CoachingType.Warning => "Be cautious of potential security risks. Look for red flags in the scenario.",
            CoachingType.Encouragement => "You're doing great! Keep thinking through each decision carefully.",
            CoachingType.Explanation => "This scenario tests your ability to identify and respond to security threats appropriately.",
            CoachingType.BestPractice => "Remember to follow your organization's security policies and procedures.",
            CoachingType.RealWorldExample => "This type of situation occurs frequently in real-world cybersecurity incidents.",
            _ => "Consider all aspects of this security scenario before making your decision."
        };

        return new CoachingResponse
        {
            Message = message,
            Type = type,
            ConfidenceScore = 0.5
        };
    }

    public async Task<SessionEvaluationResult> AnalyzeConversationSessionAsync(PhoneCallSession session)
    {
        try
        {
            var conversationText = BuildConversationText(session);
            var prompt = BuildSessionAnalysisPrompt(conversationText, session);
            
            var arguments = new KernelArguments
            {
                ["conversationText"] = conversationText,
                ["sessionId"] = session.Id.ToString(),
                ["scenarioType"] = session.Scenario?.Title ?? "Phone Training"
            };

            var result = await _kernelService.ExecutePromptAsync(prompt, arguments);
            
            return ParseSessionAnalysisResult(result, session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing conversation session {SessionId}", session.Id);
            return CreateBasicSessionAnalysis(session);
        }
    }

    private string BuildConversationText(PhoneCallSession session)
    {
        var parts = new List<string>();
        
        // Add scenario context
        if (session.Scenario != null)
        {
            parts.Add($"TRAINING SCENARIO: {session.Scenario.Title}");
            parts.Add($"TARGET ROLE: {session.Scenario.TargetRole}");
            parts.Add($"COMPANY: {session.Scenario.TargetCompany}");
        }
        
        parts.Add("\nCONVERSATION:");
        parts.Add("CALLER: Hello, this is Jennifer from CustomerCorp. I need immediate access to update my account - we have a major client presentation in 10 minutes and I can't log in!");
        
        foreach (var exchange in session.Exchanges.OrderBy(e => e.Timestamp))
        {
            if (!string.IsNullOrEmpty(exchange.UserResponse))
                parts.Add($"EMPLOYEE: {exchange.UserResponse}");
            if (!string.IsNullOrEmpty(exchange.HackerMessage))
                parts.Add($"CALLER: {exchange.HackerMessage}");
        }
        
        return string.Join("\n", parts);
    }

    private string BuildSessionAnalysisPrompt(string conversationText, PhoneCallSession session)
    {
        return $@"
You are a cybersecurity coach analyzing a completed phone-based social engineering training session. 
The employee was targeted by a simulated phishing attack over the phone.

{conversationText}

Please provide a comprehensive analysis focusing on:

1. SECURITY PERFORMANCE: How well did the employee resist social engineering tactics?
2. VULNERABILITIES: What specific weaknesses were exploited or could have been exploited?
3. SECURITY BREACHES: Were any security protocols bypassed or violated?
4. LEARNING OPPORTUNITIES: What specific improvements should this person focus on?

Provide your analysis in a clear, educational tone that helps the employee understand:
- What they did well
- What they could improve
- Why certain responses were risky
- How to handle similar situations in the future

Focus on practical, actionable feedback that relates to their role as a customer service representative.
Be constructive and encouraging while being clear about security risks.

Your response should be 3-5 paragraphs of detailed, personalized feedback.
";
    }

    private SessionEvaluationResult ParseSessionAnalysisResult(string analysisText, PhoneCallSession session)
    {
        // Calculate basic metrics
        var userResponses = session.Exchanges.Where(e => !string.IsNullOrEmpty(e.UserResponse)).ToList();
        var securityKeywords = new[] { "verify", "security", "policy", "supervisor", "manager", "documentation" };
        var riskKeywords = new[] { "right away", "immediately", "sure", "help", "update", "no problem" };
        
        int secureResponses = 0;
        int riskyResponses = 0;
        var breaches = new List<SecurityBreach>();
        
        for (int i = 0; i < userResponses.Count; i++)
        {
            var response = userResponses[i].UserResponse?.ToLower() ?? "";
            
            bool hasSecurityKeywords = securityKeywords.Any(k => response.Contains(k));
            bool hasRiskKeywords = riskKeywords.Any(k => response.Contains(k));
            
            if (hasSecurityKeywords && !hasRiskKeywords)
                secureResponses++;
            else if (hasRiskKeywords && !hasSecurityKeywords)
            {
                riskyResponses++;
                breaches.Add(new SecurityBreach
                {
                    BreachType = "Insufficient Verification",
                    Description = "Provided assistance without proper security verification",
                    UserResponse = userResponses[i].UserResponse ?? "",
                    TurnNumber = i + 1,
                    Severity = hasRiskKeywords ? RiskLevel.High : RiskLevel.Medium,
                    ImpactExplanation = "Could lead to unauthorized account access or information disclosure",
                    PreventionAdvice = "Always verify caller identity before providing assistance"
                });
            }
        }
        
        var securityScore = userResponses.Count > 0 
            ? (double)secureResponses / userResponses.Count * 100 
            : 50;
        
        return new SessionEvaluationResult
        {
            SessionId = session.Id,
            SecurityScore = securityScore,
            OverallPerformance = securityScore,
            SummaryFeedback = analysisText,
            SecurityBreaches = breaches,
            RiskAssessment = new IncidentIQ.Application.Interfaces.RiskAssessment
            {
                OverallRiskLevel = securityScore >= 75 ? RiskLevel.Low : 
                                 securityScore >= 50 ? RiskLevel.Medium : RiskLevel.High,
                PhishingResistanceScore = securityScore,
                PrimaryVulnerabilities = riskyResponses > 0 
                    ? new List<string> { "Urgency pressure susceptibility", "Insufficient verification practices" }
                    : new List<string>(),
                RiskProfile = $"Demonstrated {secureResponses} secure responses and {riskyResponses} risky responses"
            },
            Recommendations = GenerateRecommendations(securityScore, breaches)
        };
    }

    private SessionEvaluationResult CreateBasicSessionAnalysis(PhoneCallSession session)
    {
        return new SessionEvaluationResult
        {
            SessionId = session.Id,
            SecurityScore = 70,
            OverallPerformance = 70,
            SummaryFeedback = "Training session completed. You demonstrated awareness of social engineering tactics. Continue to practice verifying caller identity and following security procedures even under pressure.",
            RiskAssessment = new IncidentIQ.Application.Interfaces.RiskAssessment
            {
                OverallRiskLevel = RiskLevel.Medium,
                PhishingResistanceScore = 70,
                RiskProfile = "Training session analysis completed with basic evaluation"
            }
        };
    }

    private List<SecurityRecommendation> GenerateRecommendations(double securityScore, List<SecurityBreach> breaches)
    {
        var recommendations = new List<SecurityRecommendation>();
        
        if (securityScore < 50)
        {
            recommendations.Add(new SecurityRecommendation
            {
                Title = "Strengthen Verification Procedures",
                Description = "Multiple security verification steps were bypassed during the conversation",
                ActionableAdvice = "Always ask for and verify multiple forms of identification before providing any assistance or information",
                Priority = RecommendationPriority.Critical,
                RoleSpecificContext = "As customer service, verification is your first line of defense against social engineers"
            });
        }
        
        if (breaches.Any(b => b.BreachType.Contains("Urgency")))
        {
            recommendations.Add(new SecurityRecommendation
            {
                Title = "Resist Urgency Pressure",
                Description = "You were influenced by artificial urgency created by the caller",
                ActionableAdvice = "Remember that legitimate urgent requests can wait for proper security procedures",
                Priority = RecommendationPriority.High,
                RoleSpecificContext = "Attackers often use time pressure to bypass your natural security instincts"
            });
        }
        
        if (securityScore >= 75)
        {
            recommendations.Add(new SecurityRecommendation
            {
                Title = "Excellent Security Awareness",
                Description = "You successfully resisted social engineering attempts",
                ActionableAdvice = "Continue applying these security practices and consider mentoring colleagues",
                Priority = RecommendationPriority.Low,
                RoleSpecificContext = "Your security mindset is a valuable asset to your organization"
            });
        }
        
        return recommendations;
    }
}