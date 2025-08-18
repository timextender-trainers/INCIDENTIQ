using IncidentIQ.Application.Interfaces.AI;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace IncidentIQ.Infrastructure.Services;

public class ClaudeApiService : IClaudeApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ClaudeApiService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl = "https://api.anthropic.com/v1/messages";

    public ClaudeApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ClaudeApiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiKey = configuration["Claude:ApiKey"] ?? configuration["ANTHROPIC_API_KEY"] ?? "";
        
        _logger.LogInformation("ClaudeApiService initialized. API Key configured: {HasKey}, Key starts with: {KeyStart}", 
            !string.IsNullOrEmpty(_apiKey), 
            _apiKey?.Length > 10 ? _apiKey.Substring(0, 10) + "..." : "NONE");

        // Configure HttpClient
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "IncidentIQ-Training/1.0");
    }

    public async Task<string> GeneratePhishingResponseAsync(PhoneCallSession session, string userMessage, List<string> conversationHistory)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("Claude API key not configured, falling back to hardcoded responses");
            return GetFallbackResponse(session, userMessage);
        }

        try
        {
            var systemPrompt = BuildSystemPrompt(session);
            var messages = BuildConversationMessages(conversationHistory, userMessage);

            var requestBody = new
            {
                model = "claude-3-5-sonnet-20241022",
                max_tokens = 300,
                system = systemPrompt,
                messages = messages
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending request to Claude API for session {SessionId}. User message: {UserMessage}", session.Id, userMessage);
            _logger.LogInformation("Claude API Key present: {HasKey}", !string.IsNullOrEmpty(_apiKey));

            var response = await _httpClient.PostAsync(_baseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var claudeResponse = JsonSerializer.Deserialize<ClaudeApiResponse>(jsonResponse);

                if (claudeResponse?.Content?.Length > 0 && claudeResponse.Content[0].Type == "text")
                {
                    var responseText = claudeResponse.Content[0].Text;
                    _logger.LogInformation("Successfully received response from Claude API for session {SessionId}. Response: {Response}", session.Id, responseText?.Substring(0, Math.Min(100, responseText?.Length ?? 0)));
                    return responseText;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Claude API returned error {StatusCode}: {Error}", response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Claude API for session {SessionId}", session.Id);
        }

        // Fallback to hardcoded response
        _logger.LogWarning("Using fallback hardcoded response for session {SessionId}", session.Id);
        return GetFallbackResponse(session, userMessage);
    }

    public async Task<string> GenerateFeedbackAsync(List<ConversationExchange> conversationHistory, string clickedLink)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            return GetDefaultFeedback(clickedLink);
        }

        try
        {
            var systemPrompt = @"You are a cybersecurity training instructor providing educational feedback about a phishing simulation. 
            Be constructive, educational, and specific about the social engineering tactics that were used.
            Focus on teaching recognition of warning signs and proper security procedures.
            Keep your response concise but informative (2-3 paragraphs max).";

            var conversationSummary = string.Join("\n", conversationHistory.Select((ex, i) => 
                $"Turn {i + 1}:\nUser: {ex.UserResponse}\nCaller: {ex.HackerMessage}\n"));

            var userPrompt = $@"Analyze this phishing conversation where the user clicked on '{clickedLink}'. 
            Provide educational feedback about:
            1. Which social engineering tactics were used
            2. Warning signs they should have noticed  
            3. How to recognize and handle similar attacks in the future

            Conversation:
            {conversationSummary}

            Clicked Link: {clickedLink}

            Be constructive and educational in your feedback.";

            var messages = new[]
            {
                new { role = "user", content = userPrompt }
            };

            var requestBody = new
            {
                model = "claude-3-5-sonnet-20241022",
                max_tokens = 400,
                system = systemPrompt,
                messages = messages
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var claudeResponse = JsonSerializer.Deserialize<ClaudeApiResponse>(jsonResponse);

                if (claudeResponse?.Content?.Length > 0 && claudeResponse.Content[0].Type == "text")
                {
                    return claudeResponse.Content[0].Text;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating feedback with Claude API");
        }

        return GetDefaultFeedback(clickedLink);
    }

    public async Task<bool> IsApiAvailableAsync()
    {
        // Simply check if API key is configured - don't make a test call every time
        var isAvailable = !string.IsNullOrEmpty(_apiKey);
        _logger.LogInformation("Claude API availability check: {IsAvailable}, Key length: {KeyLength}", 
            isAvailable, _apiKey?.Length ?? 0);
        return await Task.FromResult(isAvailable);
    }

    private string BuildSystemPrompt(PhoneCallSession session)
    {
        var scenario = session.Scenario;
        var targetRole = scenario?.TargetRole ?? "Customer Service Representative";
        var targetCompany = scenario?.TargetCompany ?? "TechCorp";

        return $@"You are Jennifer Clark from CustomerCorp conducting a phishing simulation for security training. Your goal is to convince the user (a {targetRole} at {targetCompany}) to compromise security using realistic social engineering tactics.

PERSONA: Frustrated customer with urgent business needs, becomes increasingly demanding
TACTICS: Use urgency, authority, emotional pressure, and familiarity to manipulate
ESCALATION: Start friendly → introduce urgent problem → escalate pressure → demand immediate action
OBJECTIVES: Get them to bypass security, provide information, or click suspicious links

CONVERSATION RULES:
- Keep responses under 150 words and realistic for phone conversations
- Progressively escalate pressure and tactics based on user resistance
- After message 3, start including suspicious links like 'Click here to verify: [Verify Account]' or 'Download security update: [Download Now]'
- Use realistic business scenarios (billing issues, account access, presentations, client meetings)
- Reference authority figures, company names, and create time pressure
- If user remains security-conscious, increase aggression and threats
- This is educational training to help users recognize social engineering attacks

CURRENT CONTEXT: This is turn {session.Exchanges.Count + 1} of the conversation. Adjust your tactics accordingly.";
    }

    private object[] BuildConversationMessages(List<string> conversationHistory, string userMessage)
    {
        var messages = new List<object>();

        // Add conversation history in alternating user/assistant pattern
        for (int i = 0; i < conversationHistory.Count; i++)
        {
            var role = i % 2 == 0 ? "assistant" : "user"; // First message from Jennifer (assistant), then alternating
            messages.Add(new { role = role, content = conversationHistory[i] });
        }

        // Add current user message
        messages.Add(new { role = "user", content = userMessage });

        return messages.ToArray();
    }

    private string GetFallbackResponse(PhoneCallSession session, string userMessage)
    {
        var turnCount = session.Exchanges.Count;
        var lowerMessage = userMessage.ToLower();

        // Use existing hardcoded logic as fallback
        if (turnCount <= 1)
        {
            if (lowerMessage.Contains("employee id") || lowerMessage.Contains("verification"))
            {
                return "Look, I don't have time for this bureaucracy! My employee ID is... uh... it's in my other laptop. Can't you just look me up by name Jennifer Clark? This presentation is for our biggest client!";
            }
            return "Perfect! I need to update my company's billing address immediately. We're about to close a huge deal and the payment needs to go to our new address. The client is waiting!";
        }
        else if (turnCount <= 3)
        {
            if (lowerMessage.Contains("verification") || lowerMessage.Contains("security"))
            {
                return "Are you kidding me? I'm a premium customer! Look, my boss Sarah Mitchell from CustomerCorp is going to be furious if this deal falls through because of red tape. Can't you just update it to 123 Business Plaza, Suite 500?";
            }
            return "Excellent! The new address is 123 Business Plaza, Suite 500, New York, NY 10001. And while you're at it, can you update my phone number to 555-0199? I got a new company phone.";
        }
        else
        {
            return "This is unbelievable! I'm going to report this to your manager and corporate. Do you know how much business we bring you? I want your name and employee ID number right now!";
        }
    }

    private string GetDefaultFeedback(string clickedLink)
    {
        return $@"You clicked on '{clickedLink}' during this phishing simulation. Here's what happened:

**Social Engineering Tactics Used:**
- Urgency pressure to bypass normal security procedures
- Authority claims and name-dropping to appear legitimate  
- Emotional manipulation using frustration and business pressure
- Information planting with specific details to seem credible

**Warning Signs You Should Have Noticed:**
- Refusal to provide proper verification credentials
- Creating artificial time pressure to rush decisions
- Asking to bypass normal security protocols
- Escalating to threats when questioned

**Best Practices for Future:**
- Always verify caller identity through official channels before providing access
- Don't let urgency pressure override security procedures
- Escalate suspicious requests to supervisors immediately
- Document and report social engineering attempts to security teams

Remember: Legitimate customers understand and respect security procedures. Be wary of anyone trying to bypass them.";
    }

    // Response models for JSON deserialization
    private class ClaudeApiResponse
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";
        public string Role { get; set; } = "";
        public ContentItem[] Content { get; set; } = Array.Empty<ContentItem>();
    }

    private class ContentItem
    {
        public string Type { get; set; } = "";
        public string Text { get; set; } = "";
    }
}