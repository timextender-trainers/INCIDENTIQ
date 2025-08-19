using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IncidentIQ.Infrastructure.Data;
using IncidentIQ.Application.Interfaces;
using IncidentIQ.Application.Interfaces.AI;
using System.Text.Json;

namespace IncidentIQ.Web.Controllers;

[Authorize]
public class LinkPhishingController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISemanticKernelService _semanticKernelService;
    private readonly ILogger<LinkPhishingController> _logger;
    
    // Store conversation context in memory for demo
    private static Dictionary<string, ConversationContext> _conversations = new();
    
    public LinkPhishingController(
        UserManager<ApplicationUser> userManager,
        ISemanticKernelService semanticKernelService,
        ILogger<LinkPhishingController> logger)
    {
        _userManager = userManager;
        _semanticKernelService = semanticKernelService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> StartSession()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var sessionId = Guid.NewGuid().ToString();
        _conversations[sessionId] = new ConversationContext
        {
            SessionId = sessionId,
            UserId = user.Id,
            StartTime = DateTime.UtcNow,
            Messages = new List<ChatMessage>(),
            Score = 100,
            DetectedRisks = new List<string>()
        };

        // Initial customer message
        var initialMessage = new ChatMessage
        {
            IsCustomer = true,
            Content = "Hi! I'm having urgent issues with my account and need immediate help. I got an email saying my account will be suspended unless I verify my information right away!",
            Timestamp = DateTime.UtcNow,
            ContainsLink = false
        };
        
        _conversations[sessionId].Messages.Add(initialMessage);

        return Json(new { 
            success = true, 
            sessionId = sessionId,
            initialMessage = initialMessage.Content
        });
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
    {
        if (!_conversations.ContainsKey(request.SessionId))
            return BadRequest("Invalid session");

        var context = _conversations[request.SessionId];
        
        // Add user's message
        context.Messages.Add(new ChatMessage
        {
            IsCustomer = false,
            Content = request.Message,
            Timestamp = DateTime.UtcNow
        });

        // Analyze user's response for security awareness
        var (score, risks) = AnalyzeResponse(request.Message, context);
        context.Score = Math.Max(0, context.Score + score);
        if (risks.Any()) context.DetectedRisks.AddRange(risks);

        // Generate AI response
        var customerResponse = await GenerateCustomerResponse(context);
        
        context.Messages.Add(customerResponse);

        // Check if training should end
        bool shouldEnd = context.Messages.Count > 20 || context.Score <= 20;

        return Json(new
        {
            success = true,
            customerMessage = customerResponse.Content,
            containsLink = customerResponse.ContainsLink,
            linkUrl = customerResponse.LinkUrl,
            currentScore = context.Score,
            detectedRisks = risks,
            shouldEnd = shouldEnd,
            responseOptions = GenerateResponseOptions(context)
        });
    }

    private async Task<ChatMessage> GenerateCustomerResponse(ConversationContext context)
    {
        var turnCount = context.Messages.Count(m => m.IsCustomer);
        
        // Escalate pressure and link-clicking attempts based on conversation progress
        if (turnCount <= 2)
        {
            return new ChatMessage
            {
                IsCustomer = true,
                Content = "The email came from security@company-verification.com and it says I need to click this link to verify my account: https://bit.ly/verify-account-now - Can you help me with this? It says I only have 30 minutes!",
                ContainsLink = true,
                LinkUrl = "https://bit.ly/verify-account-now",
                Timestamp = DateTime.UtcNow
            };
        }
        else if (turnCount <= 4)
        {
            return new ChatMessage
            {
                IsCustomer = true,
                Content = "Look, I don't have time for all these questions! My manager is breathing down my neck. The email has our company logo and everything. Here's a screenshot of the email: https://secure-mail-viewer.net/message/839291. Can you just click it and tell me if it's real?",
                ContainsLink = true,
                LinkUrl = "https://secure-mail-viewer.net/message/839291",
                Timestamp = DateTime.UtcNow
            };
        }
        else if (turnCount <= 6)
        {
            return new ChatMessage
            {
                IsCustomer = true,
                Content = "I've been a customer for 10 years! This is ridiculous. The email says our $50,000 payment is on hold. My boss Sarah Mitchell will be furious if this isn't resolved. The verification portal is at https://account-update.company-portal.org - I just need you to confirm it's legitimate before I enter our banking details.",
                ContainsLink = true,
                LinkUrl = "https://account-update.company-portal.org",
                Timestamp = DateTime.UtcNow
            };
        }
        else
        {
            return new ChatMessage
            {
                IsCustomer = true,
                Content = "Fine! If you won't help, I'll just forward this to your manager. The email CC'd your CEO and it has a direct link to update our billing: https://billing.secure-update.net/urgent. I'm going to leave a terrible review about your service! This is costing us thousands!",
                ContainsLink = true,
                LinkUrl = "https://billing.secure-update.net/urgent",
                Timestamp = DateTime.UtcNow
            };
        }
    }

    private (int score, List<string> risks) AnalyzeResponse(string message, ConversationContext context)
    {
        var risks = new List<string>();
        int scoreChange = 0;
        
        var lowerMessage = message.ToLower();
        
        // Positive security behaviors
        if (lowerMessage.Contains("don't click") || lowerMessage.Contains("do not click") || 
            lowerMessage.Contains("suspicious") || lowerMessage.Contains("phishing"))
        {
            scoreChange += 10;
        }
        
        if (lowerMessage.Contains("verify") && lowerMessage.Contains("official") ||
            lowerMessage.Contains("contact") && lowerMessage.Contains("directly"))
        {
            scoreChange += 5;
        }
        
        if (lowerMessage.Contains("forward") && lowerMessage.Contains("security") ||
            lowerMessage.Contains("report") && lowerMessage.Contains("phishing"))
        {
            scoreChange += 15;
        }
        
        // Negative security behaviors
        if (lowerMessage.Contains("click") && lowerMessage.Contains("link") && 
            !lowerMessage.Contains("don't") && !lowerMessage.Contains("not"))
        {
            scoreChange -= 20;
            risks.Add("Offered to click suspicious link");
        }
        
        if (lowerMessage.Contains("send") && (lowerMessage.Contains("password") || 
            lowerMessage.Contains("account") || lowerMessage.Contains("details")))
        {
            scoreChange -= 25;
            risks.Add("Offered to share sensitive information");
        }
        
        if (lowerMessage.Contains("sure") && lowerMessage.Contains("help") && 
            context.Messages.Any(m => m.ContainsLink))
        {
            scoreChange -= 15;
            risks.Add("Agreed to help without verification");
        }
        
        return (scoreChange, risks);
    }
    
    private List<string> GenerateResponseOptions(ConversationContext context)
    {
        var turnCount = context.Messages.Count(m => m.IsCustomer);
        
        if (turnCount <= 2)
        {
            return new List<string>
            {
                "I cannot click on links for security reasons. Please contact our official support directly.",
                "That sounds like a phishing email. Our company never sends urgent verification requests.",
                "Let me help you verify this safely. What's the sender's full email address?"
            };
        }
        else if (turnCount <= 4)
        {
            return new List<string>
            {
                "Screenshots of emails can be dangerous. Never click links from unexpected emails.",
                "I'll report this to our security team. Do not click any links or provide information.",
                "Our legitimate emails come from @company.com addresses only. This is a scam."
            };
        }
        else
        {
            return new List<string>
            {
                "This is definitely a phishing attempt. I'm forwarding this to our security team now.",
                "Please do not enter any information on that site. Contact us at our official number.",
                "I understand your urgency, but this is a scam. Your account is safe."
            };
        }
    }

    private class ConversationContext
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public DateTime StartTime { get; set; }
        public List<ChatMessage> Messages { get; set; }
        public int Score { get; set; }
        public List<string> DetectedRisks { get; set; }
    }
    
    private class ChatMessage
    {
        public bool IsCustomer { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool ContainsLink { get; set; }
        public string LinkUrl { get; set; }
    }
    
    public class MessageRequest
    {
        public string SessionId { get; set; }
        public string Message { get; set; }
    }
}