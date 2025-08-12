using IncidentIQ.Application.Interfaces.AI;
using IncidentIQ.Application.Interfaces.AI.Agents;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace IncidentIQ.Infrastructure.AI.Agents;

public class ScenarioGeneratorAgent : IScenarioGeneratorAgent
{
    private readonly ISemanticKernelService _kernelService;
    private readonly ILogger<ScenarioGeneratorAgent> _logger;

    public ScenarioGeneratorAgent(ISemanticKernelService kernelService, ILogger<ScenarioGeneratorAgent> logger)
    {
        _kernelService = kernelService;
        _logger = logger;
    }

    public async Task<TrainingScenario> GenerateScenarioAsync(ScenarioGenerationRequest request)
    {
        try
        {
            var prompt = BuildScenarioPrompt(request);
            var arguments = new KernelArguments
            {
                ["scenarioType"] = request.Type.ToString(),
                ["userRole"] = request.TargetRole.ToString(),
                ["difficulty"] = request.Difficulty.ToString(),
                ["securityLevel"] = request.TargetSecurityLevel.ToString(),
                ["companyName"] = request.User.Company,
                ["userName"] = $"{request.User.FirstName} {request.User.LastName}",
                ["department"] = request.User.Department,
                ["userEmail"] = request.User.Email
            };

            var result = await _kernelService.ExecutePromptAsync(prompt, arguments);
            
            return ParseScenarioFromResponse(result, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating scenario for user {UserId}", request.User.Id);
            throw;
        }
    }

    public async Task<ScenarioContent> GenerateContentAsync(TrainingScenario scenario, User user)
    {
        var prompt = BuildContentPrompt(scenario, user);
        var arguments = new KernelArguments
        {
            ["scenarioTitle"] = scenario.Title,
            ["scenarioDescription"] = scenario.Description,
            ["userRole"] = user.Role.ToString(),
            ["companyName"] = user.Company,
            ["department"] = user.Department
        };

        var result = await _kernelService.ExecutePromptAsync(prompt, arguments);
        return ParseContentFromResponse(result);
    }

    public async Task<List<DecisionPoint>> GenerateDecisionPointsAsync(ScenarioContent content, DifficultyLevel difficulty)
    {
        var prompt = BuildDecisionPointsPrompt(content, difficulty);
        var arguments = new KernelArguments
        {
            ["primaryContent"] = content.PrimaryContent,
            ["difficulty"] = difficulty.ToString(),
            ["learningObjectives"] = JsonSerializer.Serialize(content.LearningObjectives)
        };

        var result = await _kernelService.ExecutePromptAsync(prompt, arguments);
        return ParseDecisionPointsFromResponse(result);
    }

    private static string BuildScenarioPrompt(ScenarioGenerationRequest request)
    {
        return $@"You are an expert cybersecurity training specialist. Generate a personalized {request.Type} training scenario.

User Profile:
- Role: {request.TargetRole}
- Security Level: {request.TargetSecurityLevel}
- Company: {request.User.Company}
- Department: {request.User.Department}
- Name: {request.User.FirstName} {request.User.LastName}

Requirements:
- Difficulty: {request.Difficulty}
- Make it highly personalized to the user's role and company
- Include realistic details that would apply to their specific job function
- Create scenarios that feel authentic and relevant to their daily work

Generate a comprehensive training scenario with:
1. A compelling title
2. Detailed description
3. Learning objectives
4. Estimated duration

Format the response as JSON with the following structure:
{{
    ""title"": ""scenario title"",
    ""description"": ""detailed description"",
    ""learningObjectives"": [""objective1"", ""objective2""],
    ""estimatedDurationMinutes"": 15
}}";
    }

    private static string BuildContentPrompt(TrainingScenario scenario, User user)
    {
        return $"""
        Create the primary content for this cybersecurity training scenario:

        Scenario: {scenario.Title}
        Description: {scenario.Description}
        User Role: {user.Role}
        Company: {user.Company}
        Department: {user.Department}

        Generate realistic, personalized content that:
        1. Uses the user's actual company name and department context
        2. References tools and systems relevant to their role
        3. Includes colleague names and business scenarios they would encounter
        4. Feels completely authentic to their work environment

        The content should be the primary interaction (email, phone call script, etc.) that the user will encounter.
        Make it sophisticated enough to be challenging but realistic enough to be believable.

        Return only the content text, no additional formatting or explanations.
        """;
    }

    private static string BuildDecisionPointsPrompt(ScenarioContent content, DifficultyLevel difficulty)
    {
        return $@"Create decision points for this cybersecurity training content:

Content: {content.PrimaryContent}
Difficulty: {difficulty}

Generate 3-5 decision points that test the user's cybersecurity awareness.
Each decision point should have:
- A clear question about how to respond
- 3-4 multiple choice options
- The correct answer index (0-based)
- A detailed explanation of why the correct answer is right
- A coaching tip for improvement

Format as JSON array:
[
    {{
        ""question"": ""What should you do next?"",
        ""options"": [""Option 1"", ""Option 2"", ""Option 3""],
        ""correctOptionIndex"": 1,
        ""explanation"": ""Explanation of correct answer"",
        ""coachingTip"": ""Practical tip for similar situations""
    }}
]";
    }

    private TrainingScenario ParseScenarioFromResponse(string response, ScenarioGenerationRequest request)
    {
        try
        {
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response);
            
            return new TrainingScenario
            {
                Type = request.Type,
                Title = jsonResponse.GetProperty("title").GetString() ?? "Untitled Scenario",
                Description = jsonResponse.GetProperty("description").GetString() ?? "",
                Difficulty = request.Difficulty,
                TargetSecurityLevel = request.TargetSecurityLevel,
                TargetRole = request.TargetRole,
                Configuration = new ScenarioConfiguration
                {
                    EstimatedDurationMinutes = jsonResponse.TryGetProperty("estimatedDurationMinutes", out var duration) 
                        ? duration.GetInt32() : 15
                },
                PersonalizationContext = new PersonalizationData
                {
                    CompanyName = request.User.Company,
                    ColleagueNames = request.User.SecurityProfile.ColleagueNames,
                    RoleSpecificContext = new Dictionary<string, object>
                    {
                        ["role"] = request.TargetRole.ToString(),
                        ["department"] = request.User.Department,
                        ["securityLevel"] = request.TargetSecurityLevel.ToString()
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse scenario response, using fallback");
            return CreateFallbackScenario(request);
        }
    }

    private ScenarioContent ParseContentFromResponse(string response)
    {
        return new ScenarioContent
        {
            PrimaryContent = response.Trim(),
            LearningObjectives = new List<string>
            {
                "Identify security threats in realistic business scenarios",
                "Apply appropriate security protocols and procedures",
                "Make informed decisions under pressure"
            }
        };
    }

    private List<DecisionPoint> ParseDecisionPointsFromResponse(string response)
    {
        try
        {
            var decisionPoints = JsonSerializer.Deserialize<List<DecisionPoint>>(response);
            return decisionPoints ?? new List<DecisionPoint>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse decision points, using fallback");
            return CreateFallbackDecisionPoints();
        }
    }

    private TrainingScenario CreateFallbackScenario(ScenarioGenerationRequest request)
    {
        return new TrainingScenario
        {
            Type = request.Type,
            Title = $"Security Awareness Training - {request.Type}",
            Description = "A personalized cybersecurity training scenario designed for your role and organization.",
            Difficulty = request.Difficulty,
            TargetSecurityLevel = request.TargetSecurityLevel,
            TargetRole = request.TargetRole
        };
    }

    private List<DecisionPoint> CreateFallbackDecisionPoints()
    {
        return new List<DecisionPoint>
        {
            new()
            {
                Question = "What is your immediate response to this situation?",
                Options = new List<string>
                {
                    "Ignore it and continue working",
                    "Report it to the security team immediately",
                    "Handle it yourself without involving others"
                },
                CorrectOptionIndex = 1,
                Explanation = "Security incidents should always be reported to the appropriate security team for proper handling.",
                CoachingTip = "When in doubt, always escalate security concerns to trained professionals."
            }
        };
    }
}