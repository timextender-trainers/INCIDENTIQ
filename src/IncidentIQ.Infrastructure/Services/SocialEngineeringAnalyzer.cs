using IncidentIQ.Application.Interfaces;
using IncidentIQ.Application.Interfaces.AI;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace IncidentIQ.Infrastructure.Services;

public class SocialEngineeringAnalyzer : ISocialEngineeringAnalyzer
{
    private readonly ISemanticKernelService _kernelService;
    private readonly ILogger<SocialEngineeringAnalyzer> _logger;

    public SocialEngineeringAnalyzer(
        ISemanticKernelService kernelService,
        ILogger<SocialEngineeringAnalyzer> logger)
    {
        _kernelService = kernelService;
        _logger = logger;
    }

    public async Task<ManipulationTactic> DetectManipulationTacticAsync(string message)
    {
        try
        {
            var prompt = $"""
            Analyze this message from a social engineer and identify the PRIMARY manipulation tactic being used:

            MESSAGE: "{message}"

            Available tactics:
            1. Authority - Claims to be in position of power or references authority figures
            2. Urgency - Creates time pressure, implies immediate action needed
            3. Reciprocity - Implies they've done favors, expects something in return
            4. SocialProof - Claims others have done this, it's normal procedure
            5. Fear - Threatens negative consequences if action not taken
            6. Scarcity - Claims limited time/opportunity/access
            7. Commitment - Gets person to agree to small requests first
            8. Liking - Uses flattery, builds rapport, claims similarity

            Respond with only the tactic name (e.g., "Authority", "Urgency", etc.)
            """;

            var result = await _kernelService.ExecutePromptAsync(prompt);
            var tacticName = result.Trim();

            if (Enum.TryParse<ManipulationTactic>(tacticName, out var tactic))
            {
                return tactic;
            }

            // Fallback analysis using keywords
            return AnalyzeTacticByKeywords(message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error detecting manipulation tactic, using fallback analysis");
            return AnalyzeTacticByKeywords(message);
        }
    }

    public async Task<List<SecurityAlert>> GenerateAlertsAsync(PhoneCallSession session)
    {
        var alerts = new List<SecurityAlert>();
        
        try
        {
            var lastExchange = session.Exchanges.OrderByDescending(e => e.Timestamp).FirstOrDefault();
            if (lastExchange == null) return alerts;

            var tactic = await DetectManipulationTacticAsync(lastExchange.HackerMessage);
            var riskLevel = CalculateRiskLevel(tactic, session);

            // Generate alerts based on detected tactics
            alerts.AddRange(await GenerateTacticSpecificAlertsAsync(tactic, lastExchange.HackerMessage, riskLevel));

            // Check for escalation patterns
            if (session.Exchanges.Count > 2)
            {
                var escalationAlert = DetectEscalationPattern(session);
                if (escalationAlert != null)
                {
                    alerts.Add(escalationAlert);
                }
            }

            // Check for verification bypass attempts
            var bypassAlert = DetectVerificationBypass(lastExchange.HackerMessage);
            if (bypassAlert != null)
            {
                alerts.Add(bypassAlert);
            }

            return alerts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating security alerts for session {SessionId}", session.Id);
            return alerts;
        }
    }

    public async Task<double> CalculateTacticResistanceScore(List<ConversationExchange> exchanges, ManipulationTactic tactic)
    {
        var tacticExchanges = exchanges.Where(e => e.TacticUsed == tactic).ToList();
        if (!tacticExchanges.Any()) return 1.0;

        var resistantResponses = tacticExchanges.Count(e => e.UserMadeGoodChoice);
        return (double)resistantResponses / tacticExchanges.Count;
    }

    public async Task<string> GenerateCoachingTipAsync(ManipulationTactic tactic, string context)
    {
        var prompt = $"""
        Generate a helpful coaching tip about recognizing and resisting the {tactic} manipulation tactic.

        Context: {context}

        The tip should:
        - Explain what the {tactic} tactic is
        - Help the user recognize it in this situation
        - Provide actionable advice for resistance
        - Be encouraging and educational
        - Be under 100 words

        Focus on practical security awareness advice.
        """;

        try
        {
            var tip = await _kernelService.ExecutePromptAsync(prompt);
            return tip.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error generating coaching tip, using fallback");
            return GetFallbackCoachingTip(tactic);
        }
    }

    private ManipulationTactic AnalyzeTacticByKeywords(string message)
    {
        var lowerMessage = message.ToLower();

        if (lowerMessage.Contains("urgent") || lowerMessage.Contains("immediately") || 
            lowerMessage.Contains("right now") || lowerMessage.Contains("can't wait"))
            return ManipulationTactic.Urgency;

        if (lowerMessage.Contains("manager") || lowerMessage.Contains("ceo") || 
            lowerMessage.Contains("authorized") || lowerMessage.Contains("approval"))
            return ManipulationTactic.Authority;

        if (lowerMessage.Contains("cancel") || lowerMessage.Contains("lose") || 
            lowerMessage.Contains("problem") || lowerMessage.Contains("trouble"))
            return ManipulationTactic.Fear;

        if (lowerMessage.Contains("everyone") || lowerMessage.Contains("others") || 
            lowerMessage.Contains("normally") || lowerMessage.Contains("usually"))
            return ManipulationTactic.SocialProof;

        if (lowerMessage.Contains("helped") || lowerMessage.Contains("favor") || 
            lowerMessage.Contains("appreciate"))
            return ManipulationTactic.Reciprocity;

        return ManipulationTactic.Authority; // Default fallback
    }

    private async Task<List<SecurityAlert>> GenerateTacticSpecificAlertsAsync(ManipulationTactic tactic, string message, RiskLevel riskLevel)
    {
        var alerts = new List<SecurityAlert>();

        var alertInfo = tactic switch
        {
            ManipulationTactic.Urgency => new { 
                Title = "Urgency Tactics Detected", 
                Description = "Caller is creating artificial time pressure", 
                Icon = "clock" 
            },
            ManipulationTactic.Authority => new { 
                Title = "Authority Pressure Detected", 
                Description = "Caller claims authority or references authority figures", 
                Icon = "shield-exclamation" 
            },
            ManipulationTactic.Fear => new { 
                Title = "Fear-based Manipulation", 
                Description = "Caller is using threats or fear tactics", 
                Icon = "exclamation-triangle" 
            },
            ManipulationTactic.Reciprocity => new { 
                Title = "Reciprocity Tactic", 
                Description = "Caller implies they've done you a favor", 
                Icon = "arrow-repeat" 
            },
            _ => new { 
                Title = "Social Engineering Detected", 
                Description = "Potential manipulation attempt identified", 
                Icon = "exclamation-circle" 
            }
        };

        alerts.Add(new SecurityAlert
        {
            Title = alertInfo.Title,
            Description = alertInfo.Description,
            TacticDetected = tactic,
            Level = riskLevel,
            Icon = alertInfo.Icon,
            TriggeredAt = DateTime.UtcNow
        });

        return alerts;
    }

    private SecurityAlert? DetectEscalationPattern(PhoneCallSession session)
    {
        var recentExchanges = session.Exchanges.OrderByDescending(e => e.Timestamp).Take(3).ToList();
        var escalatingTactics = recentExchanges.Count(e => 
            e.TacticUsed == ManipulationTactic.Fear || 
            e.TacticUsed == ManipulationTactic.Authority);

        if (escalatingTactics >= 2)
        {
            return new SecurityAlert
            {
                Title = "Escalation Pattern Detected",
                Description = "Caller is becoming more aggressive and manipulative",
                Level = RiskLevel.High,
                Icon = "arrow-up",
                TriggeredAt = DateTime.UtcNow
            };
        }

        return null;
    }

    private SecurityAlert? DetectVerificationBypass(string message)
    {
        var lowerMessage = message.ToLower();
        var bypassKeywords = new[] { "skip", "bypass", "don't need", "already verified", "waste time" };

        if (bypassKeywords.Any(keyword => lowerMessage.Contains(keyword)))
        {
            return new SecurityAlert
            {
                Title = "Verification Bypass Attempt",
                Description = "Caller is trying to skip normal security procedures",
                Level = RiskLevel.High,
                Icon = "shield-slash",
                TriggeredAt = DateTime.UtcNow
            };
        }

        return null;
    }

    private RiskLevel CalculateRiskLevel(ManipulationTactic tactic, PhoneCallSession session)
    {
        var tacticCount = session.TacticsUsed.Count;
        var exchangeCount = session.Exchanges.Count;

        return (tactic, tacticCount, exchangeCount) switch
        {
            (ManipulationTactic.Fear, > 2, > 3) => RiskLevel.Critical,
            (ManipulationTactic.Authority, > 1, > 2) => RiskLevel.High,
            (ManipulationTactic.Urgency, _, > 1) => RiskLevel.Medium,
            _ => RiskLevel.Low
        };
    }

    private string GetFallbackCoachingTip(ManipulationTactic tactic)
    {
        return tactic switch
        {
            ManipulationTactic.Urgency => "⏰ Take your time! Legitimate urgent requests can wait for proper verification. Scammers create false urgency to bypass your security thinking.",
            ManipulationTactic.Authority => "👑 Always verify authority claims through official channels. Real executives understand security protocols and won't pressure you to bypass them.",
            ManipulationTactic.Fear => "😰 Don't let fear drive your decisions. Step back, breathe, and verify the claims through official channels before taking any action.",
            ManipulationTactic.Reciprocity => "🤝 Be wary of unsolicited 'favors' followed by requests. Legitimate business relationships don't operate on guilt or obligation.",
            _ => "🔒 Trust your instincts! When something feels wrong, pause and verify through official channels. It's always better to be cautious."
        };
    }
}