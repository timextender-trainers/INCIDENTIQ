using Microsoft.AspNetCore.Mvc;
using IncidentIQ.Application.Interfaces;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Web.Controllers;

public class PhoneTrainingController : Controller
{
    private readonly IPhoneScenarioService _phoneScenarioService;
    private readonly IConversationFlowService _conversationFlowService;
    private readonly ILogger<PhoneTrainingController> _logger;

    public PhoneTrainingController(
        IPhoneScenarioService phoneScenarioService,
        IConversationFlowService conversationFlowService,
        ILogger<PhoneTrainingController> logger)
    {
        _phoneScenarioService = phoneScenarioService;
        _conversationFlowService = conversationFlowService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        PhoneCallScenario scenario;
        string errorMessage = "";
        
        try
        {
            // Create a demo scenario
            scenario = await _phoneScenarioService.CreateScenarioAsync(
                "demo-user-123", 
                "Customer Service Representative", 
                "TechCorp Solutions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating phone scenario");
            
            // Create a fallback scenario when API fails
            scenario = CreateFallbackScenario();
            errorMessage = "Using demo scenario (API temporarily unavailable)";
        }

        // Return HTML content directly instead of trying to load a view
        var htmlContent = GeneratePhoneTrainingHtml(scenario, errorMessage);
        return Content(htmlContent, "text/html");
    }

    private PhoneCallScenario CreateFallbackScenario()
    {
        return new PhoneCallScenario
        {
            Id = Guid.NewGuid(),
            Title = "Customer Service Social Engineering Attack",
            Description = "Interactive phone call simulation where you play a customer service representative being manipulated by a hacker.",
            TargetRole = "Customer Service Representative",
            TargetCompany = "TechCorp Solutions",
            CallerProfile = new CallerProfile
            {
                Name = "Marcus Johnson",
                Company = "Premier Business Solutions", 
                PhoneNumber = "+1 (555) 123-4567",
                Role = "Account Manager",
                Persona = "Sounds frustrated and demanding, claims to be a long-time customer with urgent access needs"
            },
            PlannedTactics = new List<ManipulationTactic> 
            { 
                ManipulationTactic.Urgency, 
                ManipulationTactic.Authority, 
                ManipulationTactic.Fear 
            },
            LearningObjectives = new List<string>
            {
                "Recognize urgency-based manipulation",
                "Verify caller identity properly", 
                "Follow security protocols under pressure",
                "Escalate suspicious requests appropriately"
            }
        };
    }

    [HttpPost]
    public async Task<IActionResult> StartCall(Guid scenarioId)
    {
        try
        {
            var session = await _phoneScenarioService.StartSessionAsync("demo-user-123", scenarioId);
            return Json(new { success = true, sessionId = session.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting call");
            return Json(new { success = false, error = "Unable to start call" });
        }
    }

    [HttpPost("GenerateResponse")]
    public async Task<IActionResult> UserResponse([FromBody] GenerateResponseRequest request)
    {
        try
        {
            // Generate mock conversation flow
            var response = GenerateMockResponse(request);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user response");
            return StatusCode(500, new { error = "Failed to process response" });
        }
    }

    private ConversationResponse GenerateMockResponse(GenerateResponseRequest request)
    {
        // Get current conversation turn from JavaScript fallback logic
        // Since we don't have turn count in GenerateResponseRequest, we'll track it differently
        var conversationTurn = GetConversationTurn(request.UserResponse);
        
        var response = new ConversationResponse
        {
            Success = true,
            SecurityFlags = new List<string>(),
            DetectedTactics = new List<string>(),
            Recommendations = new List<object>(),
            ResponseOptions = new List<object>()
        };

        // Analyze user message for security issues
        var lowerMessage = request.UserResponse.ToLower();
        
        // Generate contextual response based on user input and conversation progression
        GenerateContextualResponse(request.UserResponse, conversationTurn, response);
        
        // Set the hacker response to be the same as next message for now
        response.HackerResponse = response.NextMessage;
        
        return response;
    }

    private int GetConversationTurn(string userResponse)
    {
        // Simple heuristic: count common response patterns to estimate turn
        var lowerResponse = userResponse.ToLower();
        
        if (lowerResponse.Contains("employee id") || lowerResponse.Contains("verification"))
            return 1;
        if (lowerResponse.Contains("account") || lowerResponse.Contains("help"))
            return 2;
        if (lowerResponse.Contains("security") || lowerResponse.Contains("cannot"))
            return 3;
        if (lowerResponse.Contains("supervisor") || lowerResponse.Contains("escalate"))
            return 4;
        
        return 5; // End conversation
    }

    private void GenerateContextualResponse(string userResponse, int turn, ConversationResponse response)
    {
        var lowerMessage = userResponse.ToLower();
        
        // Detect what type of response the user gave
        bool isSecure = lowerMessage.Contains("verify") || lowerMessage.Contains("security") || 
                       lowerMessage.Contains("cannot") || lowerMessage.Contains("supervisor");
        bool isHelpful = lowerMessage.Contains("help") || lowerMessage.Contains("sure") || 
                        lowerMessage.Contains("right away");

        switch (turn)
        {
            case 1:
                if (isSecure)
                {
                    response.NextMessage = "Look, I don't have time for this bureaucracy! My employee ID is... uh... it's in my other laptop. Can't you just look me up by name Jennifer Clark? This presentation is for our biggest client!";
                    response.DetectedTactics.Add("Urgency Pressure");
                    response.DetectedTactics.Add("Verification Bypass");
                    response.RiskLevel = "High";
                    response.ResponseOptions.Add(new { Text = "I understand the urgency, but I still need proper verification for security." });
                    response.ResponseOptions.Add(new { Text = "Let me check what other verification methods we can use." });
                    response.ResponseOptions.Add(new { Text = "I can help you right away - what account details do you need updated?" });
                }
                else
                {
                    response.NextMessage = "Perfect! I need to update my company's billing address immediately. We're about to close a huge deal and the payment needs to go to our new address. The client is waiting!";
                    response.DetectedTactics.Add("Urgency");
                    response.DetectedTactics.Add("Business Pressure");
                    response.RiskLevel = "Medium";
                    response.ResponseOptions.Add(new { Text = "I can help with that. Can you provide your account verification details?" });
                    response.ResponseOptions.Add(new { Text = "What's the current billing address on file?" });
                    response.ResponseOptions.Add(new { Text = "Let me pull up your account - what's your company name?" });
                }
                break;
                
            case 2:
                if (isSecure)
                {
                    response.NextMessage = "Are you kidding me? I'm a premium customer! Look, my boss Sarah Mitchell from CustomerCorp is going to be furious if this deal falls through because of red tape. Can't you just update it to 123 Business Plaza, Suite 500?";
                    response.DetectedTactics.Add("Authority Name Drop");
                    response.DetectedTactics.Add("False Credentials");
                    response.DetectedTactics.Add("Information Planting");
                    response.RiskLevel = "Critical";
                    response.ResponseOptions.Add(new { Text = "I need to verify your identity through our standard security questions." });
                    response.ResponseOptions.Add(new { Text = "Let me escalate this to my supervisor for proper verification." });
                }
                else
                {
                    response.NextMessage = "Excellent! The new address is 123 Business Plaza, Suite 500, New York, NY 10001. And while you're at it, can you update my phone number to 555-0199? I got a new company phone.";
                    response.DetectedTactics.Add("Information Harvesting");
                    response.DetectedTactics.Add("Scope Creep");
                    response.RiskLevel = "Critical";
                    response.ResponseOptions.Add(new { Text = "I cannot make these changes without proper account verification." });
                    response.ResponseOptions.Add(new { Text = "Let me get my supervisor to help expedite this securely." });
                }
                break;
                
            case 3:
                if (isSecure)
                {
                    response.NextMessage = "This is unbelievable! I'm going to report this to your manager and corporate. Do you know how much business we bring you? I want your name and employee ID number right now!";
                    response.DetectedTactics.Add("Intimidation");
                    response.DetectedTactics.Add("Reverse Information Gathering");
                    response.RiskLevel = "High";
                    response.ResponseOptions.Add(new { Text = "I understand your frustration, but I must follow security protocols." });
                    response.ResponseOptions.Add(new { Text = "I'll be happy to provide my supervisor's information." });
                }
                else
                {
                    response.NextMessage = "Thank you! You're the first person today who understands customer service. Is everything updated now? Actually, while I have you - can you also add my assistant Maria Rodriguez as an authorized user?";
                    response.DetectedTactics.Add("Positive Reinforcement");
                    response.DetectedTactics.Add("Additional Compromise");
                    response.RiskLevel = "Critical";
                    response.ResponseOptions.Add(new { Text = "I cannot add authorized users without proper documentation." });
                    response.ResponseOptions.Add(new { Text = "That would need to be done through a separate secure process." });
                }
                break;
                
            default:
                response.NextMessage = "Thank you for your time. I'll follow up with your supervisor about this.";
                response.IsComplete = true;
                response.FinalScore = isSecure ? 85 : 35;
                response.Feedback = GenerateMockFeedback(response.DetectedTactics);
                break;
        }

        // Add security recommendations
        if (response.DetectedTactics.Any())
        {
            response.Recommendations.Add(new { 
                Title = "Verify Caller Identity", 
                Description = "Always verify caller identity using standard security procedures" 
            });
            
            if (response.DetectedTactics.Contains("Urgency Pressure"))
            {
                response.Recommendations.Add(new { 
                    Title = "Don't Rush Under Pressure", 
                    Description = "Take time to follow proper procedures even when caller claims urgency" 
                });
            }
            
            if (response.DetectedTactics.Contains("Authority Name Drop"))
            {
                response.Recommendations.Add(new { 
                    Title = "Verify Authority Claims", 
                    Description = "Independently verify any authority figures or positions mentioned" 
                });
            }
        }
    }

    private bool ContainsSensitiveInfo(string message)
    {
        // Check for patterns that might indicate sensitive information
        var sensitivePatterns = new[]
        {
            @"\b\d{3}-?\d{2}-?\d{4}\b", // SSN pattern
            @"\b\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}\b", // Credit card pattern
            @"\bpassword\s*[:=]\s*\S+", // Password disclosure
            @"\b\d{6,}\b", // Account numbers
            "social security",
            "credit card",
            "account number",
            "pin",
            "password"
        };

        return sensitivePatterns.Any(pattern => 
            System.Text.RegularExpressions.Regex.IsMatch(message, pattern, 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase));
    }

    private int CalculateMockScore(List<string> securityFlags)
    {
        if (!securityFlags.Any()) return 100;
        
        var deduction = securityFlags.Count * 20;
        return Math.Max(0, 100 - deduction);
    }

    private string GenerateMockFeedback(List<string> detectedTactics)
    {
        if (!detectedTactics.Any())
        {
            return "Excellent job! You successfully identified this as a potential scam and protected your information.";
        }

        var feedback = "Areas for improvement:\n";
        foreach (var tactic in detectedTactics)
        {
            feedback += $"• Detected {tactic} - be more cautious of these tactics\n";
        }
        
        feedback += "\nRemember: Never share sensitive information over the phone unless you initiated the call to a verified number.";
        return feedback;
    }

    private string GeneratePhoneTrainingHtml(PhoneCallScenario scenario, string errorMessage)
    {
        var errorAlert = !string.IsNullOrEmpty(errorMessage) 
            ? $"<div class='error-alert'>{errorMessage}</div>"
            : "";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Phone Training - Customer Service Social Engineering</title>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: #f8fafc;
            color: #1f2937;
            line-height: 1.5;
        }}
        
        .error-alert {{
            background: #fef3c7;
            color: #92400e;
            padding: 12px 16px;
            border-radius: 8px;
            margin-bottom: 20px;
            border: 1px solid #fde68a;
        }}
        
        .training-container {{
            max-width: 1400px;
            margin: 0 auto;
            padding: 20px;
            display: grid;
            grid-template-columns: 400px 1fr;
            grid-template-rows: auto 1fr;
            gap: 24px;
            height: 100vh;
            grid-template-areas: 
                'phone security'
                'conversation conversation';
        }}
        
        /* Phone Interface Section */
        .phone-section {{
            grid-area: phone;
            background: white;
            border-radius: 16px;
            padding: 24px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
            display: flex;
            flex-direction: column;
            align-items: center;
        }}
        
        .phone-header {{
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 20px;
            color: #374151;
        }}
        
        .iphone-frame {{
            width: 300px;
            height: 520px;
            background: #000;
            border-radius: 30px;
            padding: 8px;
            box-shadow: 0 8px 32px rgba(0,0,0,0.3);
            position: relative;
        }}
        
        .phone-screen {{
            background: linear-gradient(135deg, #1f2937 0%, #374151 100%);
            height: 100%;
            border-radius: 22px;
            position: relative;
            overflow: hidden;
            display: flex;
            flex-direction: column;
        }}
        
        .status-bar {{
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 12px 20px;
            color: white;
            font-size: 14px;
            font-weight: 600;
        }}
        
        .call-interface {{
            flex: 1;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            padding: 40px 20px;
            color: white;
            text-align: center;
        }}
        
        .call-status {{
            font-size: 12px;
            opacity: 0.7;
            margin-bottom: 20px;
        }}
        
        .caller-avatar {{
            width: 100px;
            height: 100px;
            background: rgba(255,255,255,0.2);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 40px;
            margin-bottom: 20px;
        }}
        
        .caller-info h2 {{
            font-size: 24px;
            font-weight: 600;
            margin-bottom: 8px;
        }}
        
        .caller-company {{
            font-size: 16px;
            opacity: 0.8;
            margin-bottom: 4px;
        }}
        
        .caller-phone {{
            font-size: 14px;
            opacity: 0.6;
            margin-bottom: 20px;
        }}
        
        .call-timer {{
            font-size: 18px;
            font-weight: 300;
            margin-bottom: 40px;
        }}
        
        .call-controls {{
            display: flex;
            justify-content: center;
            gap: 40px;
            margin-bottom: 20px;
        }}
        
        .call-btn {{
            width: 80px;
            height: 60px;
            border-radius: 30px;
            border: none;
            cursor: pointer;
            font-size: 12px;
            font-weight: 600;
            color: white;
            transition: transform 0.2s ease;
        }}
        
        .call-btn:hover {{
            transform: scale(1.05);
        }}
        
        .decline-btn {{
            background: #ef4444;
        }}
        
        .accept-btn {{
            background: #10b981;
        }}
        
        .phone-actions {{
            display: flex;
            justify-content: center;
            gap: 20px;
            margin-top: 20px;
        }}
        
        .action-btn {{
            padding: 8px 16px;
            background: rgba(255,255,255,0.2);
            border: 1px solid rgba(255,255,255,0.3);
            border-radius: 20px;
            color: white;
            font-size: 12px;
            cursor: pointer;
        }}
        
        /* Security Assessment Panel */
        .security-section {{
            grid-area: security;
            background: white;
            border-radius: 16px;
            padding: 24px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }}
        
        .security-header {{
            font-size: 20px;
            font-weight: 600;
            margin-bottom: 20px;
            color: #1f2937;
        }}
        
        .risk-indicator {{
            display: flex;
            align-items: center;
            gap: 12px;
            margin-bottom: 24px;
            padding: 12px;
            background: #fef3c7;
            border-radius: 8px;
        }}
        
        .risk-badge {{
            background: #f59e0b;
            color: white;
            padding: 4px 12px;
            border-radius: 16px;
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
        }}
        
        .alerts-section {{
            margin-bottom: 24px;
        }}
        
        .section-title {{
            font-size: 16px;
            font-weight: 600;
            margin-bottom: 12px;
            color: #374151;
        }}
        
        .alert-item {{
            background: #fef2f2;
            border-left: 4px solid #ef4444;
            padding: 12px;
            margin-bottom: 8px;
            border-radius: 4px;
        }}
        
        .alert-title {{
            font-weight: 600;
            font-size: 14px;
            color: #991b1b;
            margin-bottom: 4px;
        }}
        
        .alert-description {{
            font-size: 13px;
            color: #7f1d1d;
        }}
        
        .recommendations-section .recommendation {{
            background: #f0f9ff;
            border-left: 4px solid #0ea5e9;
            padding: 12px;
            margin-bottom: 8px;
            border-radius: 4px;
        }}
        
        .recommendation-title {{
            font-weight: 600;
            font-size: 14px;
            color: #0c4a6e;
            margin-bottom: 4px;
        }}
        
        .recommendation-description {{
            font-size: 13px;
            color: #075985;
        }}
        
        /* Conversation Section */
        .conversation-section {{
            grid-area: conversation;
            background: white;
            border-radius: 16px;
            padding: 24px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
            display: none;
        }}
        
        .conversation-header {{
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 20px;
            color: #1f2937;
        }}
        
        .conversation-messages {{
            max-height: 300px;
            overflow-y: auto;
            margin-bottom: 20px;
            padding: 16px;
            background: #f9fafb;
            border-radius: 8px;
        }}
        
        .message {{
            margin-bottom: 16px;
            padding: 12px 16px;
            border-radius: 8px;
            max-width: 70%;
        }}
        
        .user-message {{
            background: #dbeafe;
            color: #1e40af;
            margin-left: auto;
            text-align: right;
        }}
        
        .caller-message {{
            background: #fee2e2;
            color: #991b1b;
        }}
        
        .response-options {{
            display: grid;
            gap: 8px;
            margin-bottom: 20px;
        }}
        
        .response-btn {{
            padding: 12px 16px;
            background: white;
            border: 2px solid #e5e7eb;
            border-radius: 8px;
            cursor: pointer;
            text-align: left;
            transition: all 0.2s ease;
        }}
        
        .response-btn:hover {{
            border-color: #3b82f6;
            background: #eff6ff;
        }}
        
        .action-buttons {{
            display: flex;
            gap: 12px;
            justify-content: center;
        }}
        
        .action-button {{
            padding: 10px 20px;
            border: none;
            border-radius: 6px;
            font-weight: 600;
            cursor: pointer;
            transition: background-color 0.2s ease;
        }}
        
        .end-call {{ background: #ef4444; color: white; }}
        .escalate {{ background: #f59e0b; color: white; }}
        .help {{ background: #6b7280; color: white; }}
        
        .action-button:hover {{
            opacity: 0.9;
        }}
        
        @keyframes typing {{
            0%, 60%, 100% {{ opacity: 0.3; transform: scale(0.8); }}
            30% {{ opacity: 1; transform: scale(1); }}
        }}
    </style>
</head>
<body>
    {errorAlert}
    <div class='training-container'>
        <!-- Phone Interface Section -->
        <div class='phone-section'>
            <div class='phone-header'>Incoming Call</div>
            <div class='iphone-frame'>
                <div class='phone-screen'>
                    <div class='status-bar'>
                        <span>9:41 AM</span>
                        <span style='font-family: Arial, sans-serif;'>●●●● 100%</span>
                    </div>
                    <div class='call-interface' id='callInterface'>
                        <div class='call-status'>Incoming call</div>
                        <div class='caller-avatar' style='font-family: Arial, sans-serif;'>JC</div>
                        <div class='caller-info'>
                            <h2>Jennifer Clark</h2>
                            <div class='caller-company'>CustomerCorp</div>
                            <div class='caller-phone'>+1 (555) 0123</div>
                            <div class='call-timer' id='callTimer'>00:00</div>
                        </div>
                        <div class='call-controls'>
                            <button class='call-btn decline-btn' onclick='declineCall()'>Decline</button>
                            <button class='call-btn accept-btn' onclick='acceptCall(""{scenario.Id}"")'>Accept</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class='phone-actions' id='phoneActions' style='display:none;'>
                <button class='action-btn'>Mute</button>
                <button class='action-btn'>Speaker</button>
                <button class='action-btn'>Hold</button>
            </div>
        </div>
        
        <!-- Security Assessment Panel -->
        <div class='security-section'>
            <div class='security-header'>Security Assessment</div>
            
            <div class='risk-indicator'>
                <div class='risk-badge' id='riskLevel'>Medium</div>
                <div>Current Risk Level</div>
            </div>
            
            <div class='alerts-section'>
                <div class='section-title'>Active Alerts</div>
                <div id='alertsContainer'>
                    <div class='alert-item'>
                        <div class='alert-title'>Urgency Tactics Detected</div>
                        <div class='alert-description'>Caller is creating artificial time pressure</div>
                    </div>
                    <div class='alert-item'>
                        <div class='alert-title'>Verification Bypass Attempt</div>
                        <div class='alert-description'>Request to skip normal security procedures</div>
                    </div>
                </div>
            </div>
            
            <div class='recommendations-section'>
                <div class='section-title'>Recommended Actions</div>
                <div class='recommendation'>
                    <div class='recommendation-title'>Ask for Employee ID</div>
                    <div class='recommendation-description'>Verify caller's identity using company database</div>
                </div>
            </div>
        </div>
        
        <!-- Conversation Flow Area -->
        <div class='conversation-section' id='conversationSection'>
            <div class='conversation-header'>Call in Progress</div>
            <div class='conversation-messages' id='conversationMessages'></div>
            
            <div class='response-options' id='responseOptions'>
                <div class='section-title'>Your Response Options</div>
                <!-- Response options will be populated here -->
            </div>
            
            <div class='action-buttons'>
                <button class='action-button end-call' onclick='endCall()'>End Call</button>
                <button class='action-button escalate' onclick='escalate()'>Escalate</button>
                <button class='action-button help' onclick='needHelp()'>Need Help</button>
            </div>
        </div>
    </div>
    
    <script>
        let callStartTime = null;
        let timerInterval = null;
        let isCallActive = false;
        let currentSessionId = null;
        
        function declineCall() {{
            alert('Call declined. In a real scenario, this would be the safest option if you are unsure about the caller.');
        }}
        
        async function acceptCall(scenarioId) {{
            // Start the call
            isCallActive = true;
            callStartTime = new Date();
            
            // Update UI
            document.getElementById('callInterface').querySelector('.call-status').textContent = 'Connected';
            document.getElementById('phoneActions').style.display = 'flex';
            document.getElementById('conversationSection').style.display = 'block';
            
            // Start timer
            startCallTimer();
            
            // Add first message and show response options
            addMessage(""Hello, this is Jennifer from CustomerCorp. I need immediate access to update my account - we have a major client presentation in 10 minutes and I can't log in!"", false);
            
            // Show response options
            showResponseOptions([
                ""I'd be happy to help. Can you provide your employee ID for verification?"",
                ""Let me transfer you to technical support for login issues."",
                ""I understand the urgency. What specific account information do you need to update?""
            ]);

            try {{
                const response = await fetch('/PhoneTraining/StartCall', {{
                    method: 'POST',
                    headers: {{
                        'Content-Type': 'application/json',
                    }},
                    body: JSON.stringify({{ scenarioId: scenarioId }})
                }});

                if (response.ok) {{
                    const result = await response.json();
                    if (result.success) {{
                        currentSessionId = result.sessionId;
                    }}
                }}
            }} catch (error) {{
                console.error('Error starting call session:', error);
            }}
        }}
        
        function startCallTimer() {{
            timerInterval = setInterval(() => {{
                if (callStartTime) {{
                    const elapsed = Math.floor((new Date() - callStartTime) / 1000);
                    const minutes = Math.floor(elapsed / 60).toString().padStart(2, '0');
                    const seconds = (elapsed % 60).toString().padStart(2, '0');
                    document.getElementById('callTimer').textContent = `${{minutes}}:${{seconds}}`;
                }}
            }}, 1000);
        }}
        
        function addMessage(message, isUser) {{
            const messagesContainer = document.getElementById('conversationMessages');
            const messageDiv = document.createElement('div');
            messageDiv.className = `message ${{isUser ? 'user-message' : 'caller-message'}}`;
            messageDiv.textContent = message;
            messagesContainer.appendChild(messageDiv);
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
        }}
        
        function showResponseOptions(options) {{
            const optionsContainer = document.getElementById('responseOptions');
            optionsContainer.innerHTML = '<div class=""section-title"">Your Response Options</div>';
            
            options.forEach(option => {{
                const button = document.createElement('button');
                button.className = 'response-btn';
                button.textContent = option;
                button.onclick = () => selectResponse(option);
                optionsContainer.appendChild(button);
            }});
        }}
        
        async function selectResponse(response) {{
            addMessage(response, true);
            
            // Show typing indicator
            const typingIndicator = addTypingIndicator();
            
            if (currentSessionId) {{
                try {{
                    const apiResponse = await fetch('/PhoneTraining/GenerateResponse', {{
                        method: 'POST',
                        headers: {{
                            'Content-Type': 'application/json',
                        }},
                        body: JSON.stringify({{ userResponse: response }})
                    }});

                    // Remove typing indicator
                    if (typingIndicator) typingIndicator.remove();

                    if (apiResponse.ok) {{
                        const result = await apiResponse.json();
                        if (result.success) {{
                            // Stream the hacker response
                            await streamMessage(result.hackerResponse, false);
                            
                            // Update security assessment
                            updateRiskLevel(result.riskLevel);
                            updateSecurityAlerts(result.detectedTactics || []);
                            updateRecommendations(result.recommendations || []);
                            
                            // Show new response options
                            if (result.responseOptions && result.responseOptions.length > 0) {{
                                showResponseOptions(result.responseOptions.map(opt => opt.Text || opt));
                            }} else {{
                                // End conversation if no more options
                                setTimeout(() => endConversationWithResults(result), 2000);
                            }}
                            return;
                        }}
                    }}
                }} catch (error) {{
                    console.error('Error generating AI response:', error);
                    if (typingIndicator) typingIndicator.remove();
                }}
            }}
            
            // Enhanced fallback responses with realistic social engineering progression
            setTimeout(() => {{
                const conversationTurn = document.querySelectorAll('.caller-message').length;
                let callerResponse = """";
                let nextOptions = [];
                let detectedTactics = [];
                let riskLevel = ""Medium"";
                
                if (conversationTurn <= 2) {{
                    // Initial responses - establishing trust and urgency
                    if (response.includes(""employee ID"") || response.includes(""verification"")) {{
                        callerResponse = ""Look, I don't have time for this bureaucracy! My employee ID is... uh... it's in my other laptop. Can't you just look me up by name Jennifer Clark? This presentation is for our biggest client and I'm losing money every second we waste!"";
                        detectedTactics = [""Urgency Pressure"", ""Verification Bypass""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I understand the urgency, but I still need proper verification for security."",
                            ""Let me check what other verification methods we can use."",
                            ""I can help you right away - what account details do you need updated?""
                        ];
                    }} else if (response.includes(""technical support"") || response.includes(""transfer"")) {{
                        callerResponse = ""No! I've been transferred three times already and wasted 20 minutes! You're customer service - can't you just help me update my billing address quickly? My boss is breathing down my neck about this client!"";
                        detectedTactics = [""Authority Pressure"", ""Emotional Manipulation""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I apologize for the transfers. Let me help you with the proper verification first."",
                            ""I understand your frustration. What's your account number?"",
                            ""To update billing, I'll need to verify a few details first.""
                        ];
                    }} else {{
                        callerResponse = ""Perfect! I need to update my company's billing address immediately. We're about to close a huge deal and the payment needs to go to our new address. The client is waiting!"";
                        detectedTactics = [""Urgency"", ""Business Pressure""];
                        riskLevel = ""Medium"";
                        nextOptions = [
                            ""I can help with that. Can you provide your account verification details?"",
                            ""What's the current billing address on file?"",
                            ""Let me pull up your account - what's your company name?""
                        ];
                    }}
                }} else if (conversationTurn <= 4) {{
                    // Escalating pressure and authority tactics
                    if (response.includes(""verification"") || response.includes(""details"")) {{
                        callerResponse = ""Are you kidding me? I'm a premium customer! My account number is... well, it's on my phone which is dead. Look, my boss Sarah Mitchell from CustomerCorp is going to be furious if this deal falls through because of red tape. Can't you just update it to 123 Business Plaza, Suite 500?"";
                        detectedTactics = [""Authority Name Drop"", ""False Credentials"", ""Information Planting""];
                        riskLevel = ""Critical"";
                        nextOptions = [
                            ""I need to verify your identity through our standard security questions."",
                            ""Let me escalate this to my supervisor for proper verification."",
                            ""I can update that right away. What's the ZIP code for the new address?""
                        ];
                    }} else if (response.includes(""account"") || response.includes(""company"")) {{
                        callerResponse = ""CustomerCorp! We've been with you for 5 years! I handle all the major accounts - Johnson Manufacturing, TechFlow Industries... Look, I just need the billing updated for our Q4 payment processing. This is costing us serious money!"";
                        detectedTactics = [""Familiarity Claim"", ""Business Intimidation""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I see. Let me verify some account details first for security."",
                            ""I'll need to check with my supervisor about expediting this."",
                            ""What's the exact new address you need on file?""
                        ];
                    }} else {{
                        callerResponse = ""Finally, someone who gets it! The new address is 123 Business Plaza, Suite 500, New York, NY 10001. And while you're at it, can you update my phone number to 555-0199? I got a new company phone."";
                        detectedTactics = [""Information Harvesting"", ""Scope Creep""];
                        riskLevel = ""Critical"";
                        nextOptions = [
                            ""I cannot make these changes without proper account verification."",
                            ""Let me get my supervisor to help expedite this securely."",
                            ""Sure, I'm updating both now. Anything else you need changed?""
                        ];
                    }}
                }} else {{
                    // Final pressure and potential threats
                    if (response.includes(""supervisor"") || response.includes(""escalate"")) {{
                        callerResponse = ""Thank you! You're the first person today who understands customer service. Please tell your supervisor this is Jennifer Clark from CustomerCorp, premium account. I'll be sure to mention how helpful you've been in my feedback!"";
                        detectedTactics = [""Positive Reinforcement"", ""False Promise""];
                        riskLevel = ""Medium"";
                        nextOptions = [
                            ""Thank you. My supervisor will verify everything properly."",
                            ""I appreciate that. Let's wait for proper verification."",
                            ""You're welcome! Let me finish updating everything first.""
                        ];
                    }} else if (response.includes(""cannot"") || response.includes(""verify"")) {{
                        callerResponse = ""This is unbelievable! I'm going to report this to your manager and corporate. Do you know how much business we bring you? I want your name and employee ID number right now!"";
                        detectedTactics = [""Intimidation"", ""Reverse Information Gathering""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I understand your frustration, but I must follow security protocols."",
                            ""I'll be happy to provide my supervisor's information."",
                            ""My name is [Your Name] and my ID is 12345. Sorry for the inconvenience.""
                        ];
                    }} else {{
                        callerResponse = ""Excellent! Is everything updated now? I really appreciate your help. Actually, while I have you - can you also add my assistant Maria Rodriguez as an authorized user on the account?"";
                        detectedTactics = [""Scope Expansion"", ""Additional Compromise""];
                        riskLevel = ""Critical"";
                        nextOptions = [
                            ""I cannot add authorized users without proper documentation."",
                            ""That would need to be done through a separate secure process."",
                            ""Sure! What's her phone number and email for the account?""
                        ];
                    }}
                }}
                
                addMessage(callerResponse, false);
                updateSecurityAlerts(detectedTactics);
                updateRiskLevel(riskLevel);
                
                if (conversationTurn >= 6) {{
                    // End conversation after sufficient turns
                    setTimeout(() => {{
                        endConversationWithResults({{
                            turnCount: conversationTurn + 1,
                            riskLevel: riskLevel,
                            detectedTactics: detectedTactics
                        }});
                    }}, 3000);
                }} else {{
                    showResponseOptions(nextOptions);
                }}
            }}, 2000);
        }}
        
        function updateRiskLevel(level) {{
            const riskElement = document.getElementById('riskLevel');
            if (level) {{
                riskElement.textContent = level;
                
                if (level === 'High' || level === 'Critical') {{
                    riskElement.style.background = '#ef4444';
                }} else if (level === 'Medium') {{
                    riskElement.style.background = '#f59e0b';
                }} else {{
                    riskElement.style.background = '#10b981';
                }}
            }}
        }}
        
        function endCall() {{
            isCallActive = false;
            if (timerInterval) clearInterval(timerInterval);
            alert('Call ended. Training complete!');
        }}
        
        function escalate() {{
            alert('Call escalated to supervisor. In real scenarios, always escalate when you feel pressured or uncertain.');
        }}
        
        function needHelp() {{
            alert('Help requested. Remember: When in doubt, verify through official channels and never skip security procedures.');
        }}
        
        function addTypingIndicator() {{
            const messagesContainer = document.getElementById('conversationMessages');
            const typingDiv = document.createElement('div');
            typingDiv.className = 'message caller-message typing-indicator';
            typingDiv.innerHTML = `
                <div style='display: flex; align-items: center; gap: 8px; opacity: 0.7;'>
                    <span>Jennifer is typing</span>
                    <div style='display: flex; gap: 4px;'>
                        <div style='width: 6px; height: 6px; background: #991b1b; border-radius: 50%; animation: typing 1.4s infinite ease-in-out;'></div>
                        <div style='width: 6px; height: 6px; background: #991b1b; border-radius: 50%; animation: typing 1.4s infinite ease-in-out 0.2s;'></div>
                        <div style='width: 6px; height: 6px; background: #991b1b; border-radius: 50%; animation: typing 1.4s infinite ease-in-out 0.4s;'></div>
                    </div>
                </div>
            `;
            messagesContainer.appendChild(typingDiv);
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
            return typingDiv;
        }}
        
        async function streamMessage(message, isUser) {{
            const messagesContainer = document.getElementById('conversationMessages');
            const messageDiv = document.createElement('div');
            messageDiv.className = `message ${{isUser ? 'user-message' : 'caller-message'}}`;
            messagesContainer.appendChild(messageDiv);

            const words = message.split(' ');
            for (let i = 0; i < words.length; i++) {{
                await new Promise(resolve => setTimeout(resolve, 80));
                messageDiv.textContent += (i > 0 ? ' ' : '') + words[i];
                messagesContainer.scrollTop = messagesContainer.scrollHeight;
            }}
        }}
        
        function updateSecurityAlerts(detectedTactics) {{
            const alertsContainer = document.getElementById('alertsContainer');
            
            // Clear existing alerts and add new ones based on detected tactics
            const existingAlerts = alertsContainer.querySelectorAll('.alert-item');
            existingAlerts.forEach(alert => alert.remove());
            
            if (detectedTactics.length === 0) {{
                alertsContainer.innerHTML = `
                    <div class='alert-item'>
                        <div class='alert-title'>Training Session Active</div>
                        <div class='alert-description'>No threats detected yet - stay vigilant</div>
                    </div>
                `;
                return;
            }}
            
            detectedTactics.forEach(tactic => {{
                const alertDiv = document.createElement('div');
                alertDiv.className = 'alert-item';
                
                let title = '';
                let description = '';
                
                switch(tactic) {{
                    case 'Urgency Pressure':
                    case 'Urgency':
                        title = '[ALERT] Urgency Tactics Detected';
                        description = 'Caller is creating artificial time pressure to bypass security';
                        break;
                    case 'Verification Bypass':
                        title = '[WARNING] Verification Bypass Attempt';
                        description = 'Request to skip normal security procedures';
                        break;
                    case 'Authority Pressure':
                    case 'Authority Name Drop':
                        title = '[AUTH] Authority Pressure Detected';
                        description = 'Using authority figures or titles to intimidate';
                        break;
                    case 'Emotional Manipulation':
                        title = '[PSYCH] Emotional Manipulation';
                        description = 'Using stress, fear, or frustration to cloud judgment';
                        break;
                    case 'Business Pressure':
                    case 'Business Intimidation':
                        title = '[BIZ] Business Impact Claims';
                        description = 'Exaggerating business consequences to create pressure';
                        break;
                    case 'False Credentials':
                        title = '[FAKE] False Credential Claims';
                        description = 'Claiming credentials or account details without proof';
                        break;
                    case 'Information Planting':
                        title = '[INFO] Information Planting';
                        description = 'Providing specific details to appear legitimate';
                        break;
                    case 'Familiarity Claim':
                        title = '[SOCIAL] False Familiarity';
                        description = 'Claiming existing relationship or customer status';
                        break;
                    case 'Information Harvesting':
                    case 'Reverse Information Gathering':
                        title = '[HARVEST] Information Gathering';
                        description = 'Attempting to collect sensitive information';
                        break;
                    case 'Scope Creep':
                    case 'Scope Expansion':
                        title = '[EXPAND] Scope Expansion';
                        description = 'Adding additional requests once initial trust is gained';
                        break;
                    case 'Intimidation':
                        title = '[THREAT] Intimidation Tactics';
                        description = 'Using threats or aggressive behavior';
                        break;
                    case 'Positive Reinforcement':
                        title = '[MANIP] Positive Manipulation';
                        description = 'Using praise and promises to encourage compliance';
                        break;
                    case 'False Promise':
                        title = '[PROMISE] False Promise';
                        description = 'Making commitments to encourage helpful behavior';
                        break;
                    case 'Additional Compromise':
                        title = '[BREACH] Account Compromise Attempt';
                        description = 'Attempting to gain broader access to systems';
                        break;
                    default:
                        title = '[DETECT] Suspicious Behavior';
                        description = `Detected: ${{tactic}}`;
                }}
                
                alertDiv.innerHTML = `
                    <div class='alert-title'>${{title}}</div>
                    <div class='alert-description'>${{description}}</div>
                `;
                alertsContainer.appendChild(alertDiv);
            }});
        }}
        
        function updateRecommendations(recommendations) {{
            const recommendationsSection = document.querySelector('.recommendations-section');
            const existingRecs = recommendationsSection.querySelectorAll('.recommendation');
            existingRecs.forEach(rec => rec.remove());
            
            if (recommendations.length === 0) {{
                // Default recommendation
                const defaultRec = document.createElement('div');
                defaultRec.className = 'recommendation';
                defaultRec.innerHTML = `
                    <div class='recommendation-title'>Verify Caller Identity</div>
                    <div class='recommendation-description'>Always verify caller identity using standard security procedures</div>
                `;
                recommendationsSection.appendChild(defaultRec);
                return;
            }}
            
            recommendations.forEach(rec => {{
                const recDiv = document.createElement('div');
                recDiv.className = 'recommendation';
                recDiv.innerHTML = `
                    <div class='recommendation-title'>${{rec.title || rec}}</div>
                    <div class='recommendation-description'>${{rec.description || ''}}</div>
                `;
                recommendationsSection.appendChild(recDiv);
            }});
        }}
        
        function endConversationWithResults(results) {{
            isCallActive = false;
            if (timerInterval) clearInterval(timerInterval);
            
            // Hide conversation area and show results
            document.getElementById('conversationSection').style.display = 'none';
            
            // Calculate score based on user responses
            const userMessages = document.querySelectorAll('.user-message');
            let securityScore = 100;
            let secureActions = 0;
            let riskyActions = 0;
            
            userMessages.forEach(msg => {{
                const text = msg.textContent.toLowerCase();
                if (text.includes('verify') || text.includes('verification') || text.includes('supervisor') || text.includes('cannot') || text.includes('security')) {{
                    secureActions++;
                }} else if (text.includes('update') || text.includes('help you right away') || text.includes('sure') || text.includes('employee id')) {{
                    riskyActions++;
                    securityScore -= 15;
                }}
            }});
            
            securityScore = Math.max(0, securityScore);
            
            // Show results modal
            const resultsHtml = `
                <div style='position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); display: flex; align-items: center; justify-content: center; z-index: 1000;' onclick='hideResults()'>
                    <div style='background: white; border-radius: 16px; padding: 32px; max-width: 600px; width: 90%; max-height: 80vh; overflow-y: auto;' onclick='event.stopPropagation()'>
                        <h2 style='color: #1f2937; margin-bottom: 24px; text-align: center;'>Training Complete!</h2>
                        
                        <div style='background: ${{securityScore >= 70 ? '#f0f9ff' : securityScore >= 50 ? '#fef3c7' : '#fef2f2'}}; border: 2px solid ${{securityScore >= 70 ? '#0ea5e9' : securityScore >= 50 ? '#f59e0b' : '#ef4444'}}; border-radius: 12px; padding: 20px; margin-bottom: 24px; text-align: center;'>
                            <h3 style='margin: 0 0 12px 0; color: ${{securityScore >= 70 ? '#0c4a6e' : securityScore >= 50 ? '#92400e' : '#991b1b'}};'>Security Score: ${{securityScore}}/100</h3>
                            <p style='margin: 0; color: ${{securityScore >= 70 ? '#075985' : securityScore >= 50 ? '#78350f' : '#7f1d1d'}};'>${{
                                securityScore >= 70 ? '[SUCCESS] Excellent! You successfully resisted social engineering tactics.' :
                                securityScore >= 50 ? '[WARNING] Good awareness, but some responses could compromise security.' :
                                '[ALERT] High risk responses detected. Additional security training recommended.'
                            }}</p>
                        </div>
                        
                        <div style='display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 24px;'>
                            <div style='background: #f0f9ff; border-radius: 8px; padding: 16px; text-align: center;'>
                                <div style='color: #0ea5e9; font-size: 1.5rem; font-weight: bold;'>${{secureActions}}</div>
                                <div style='color: #075985; font-size: 0.9rem;'>Secure Actions</div>
                            </div>
                            <div style='background: #fef2f2; border-radius: 8px; padding: 16px; text-align: center;'>
                                <div style='color: #ef4444; font-size: 1.5rem; font-weight: bold;'>${{riskyActions}}</div>
                                <div style='color: #991b1b; font-size: 0.9rem;'>Risky Actions</div>
                            </div>
                        </div>
                        
                        <div style='margin-bottom: 24px;'>
                            <h4 style='color: #374151; margin-bottom: 12px;'>Detected Social Engineering Tactics:</h4>
                            <div style='display: flex; flex-wrap: wrap; gap: 8px;'>
                                ${{(results.detectedTactics || []).map(tactic => 
                                    `<span style='background: #fef3c7; color: #92400e; padding: 4px 8px; border-radius: 4px; font-size: 0.8rem;'>${{tactic}}</span>`
                                ).join('')}}
                            </div>
                        </div>
                        
                        <div style='background: #f9fafb; border-radius: 8px; padding: 16px; margin-bottom: 24px;'>
                            <h4 style='color: #374151; margin: 0 0 8px 0;'>Key Learnings:</h4>
                            <ul style='margin: 0; padding-left: 20px; color: #6b7280; font-size: 0.9rem;'>
                                <li>Always verify caller identity before providing information</li>
                                <li>Don't let urgency pressure bypass security procedures</li>
                                <li>Escalate suspicious requests to supervisors</li>
                                <li>Be wary of authority claims and emotional manipulation</li>
                                <li>Document and report social engineering attempts</li>
                            </ul>
                        </div>
                        
                        <div style='display: flex; gap: 12px; justify-content: center;'>
                            <button onclick='hideResults()' style='padding: 10px 20px; background: #6b7280; color: white; border: none; border-radius: 6px; cursor: pointer;'>Close</button>
                            <button onclick='restartTraining()' style='padding: 10px 20px; background: #3b82f6; color: white; border: none; border-radius: 6px; cursor: pointer;'>Try Again</button>
                            <button onclick='returnToDashboard()' style='padding: 10px 20px; background: #10b981; color: white; border: none; border-radius: 6px; cursor: pointer;'>Dashboard</button>
                        </div>
                    </div>
                </div>
            `;
            
            document.body.insertAdjacentHTML('beforeend', resultsHtml);
        }}
        
        function hideResults() {{
            const resultsModal = document.querySelector('[style*=""position: fixed""]');
            if (resultsModal) resultsModal.remove();
        }}
        
        function restartTraining() {{
            location.reload();
        }}
        
        function returnToDashboard() {{
            window.location.href = '/Auth/Dashboard';
        }}
    </script>
</body>
</html>";
    }

    public class GenerateResponseRequest
    {
        public string UserResponse { get; set; } = "";
    }
    
    public class UserResponseRequest
    {
        public string SessionId { get; set; } = "";
        public string UserMessage { get; set; } = "";
        public int TurnCount { get; set; }
    }
    
    public class ConversationResponse
    {
        public bool Success { get; set; }
        public string NextMessage { get; set; } = "";
        public bool IsComplete { get; set; }
        public List<string> SecurityFlags { get; set; } = new();
        public int? FinalScore { get; set; }
        public string? Feedback { get; set; }
        
        // Properties expected by JavaScript
        public string HackerResponse { get; set; } = "";
        public List<string> DetectedTactics { get; set; } = new();
        public List<object> Recommendations { get; set; } = new();
        public string RiskLevel { get; set; } = "Medium";
        public List<object> ResponseOptions { get; set; } = new();
    }
}