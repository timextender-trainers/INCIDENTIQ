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
}