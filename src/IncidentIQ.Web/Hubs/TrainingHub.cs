using Microsoft.AspNetCore.SignalR;
using IncidentIQ.Application.Interfaces;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;
using System.Security.Claims;

namespace IncidentIQ.Web.Hubs;

public class TrainingHub : Hub
{
    private readonly IPhoneScenarioService _phoneScenarioService;
    private readonly IConversationFlowService _conversationFlowService;
    private readonly ILogger<TrainingHub> _logger;

    public TrainingHub(
        IPhoneScenarioService phoneScenarioService,
        IConversationFlowService conversationFlowService,
        ILogger<TrainingHub> logger)
    {
        _phoneScenarioService = phoneScenarioService;
        _conversationFlowService = conversationFlowService;
        _logger = logger;
    }

    public async Task JoinTrainingSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"training_{sessionId}");
        _logger.LogInformation("User {UserId} joined training session {SessionId}", 
            Context.UserIdentifier, sessionId);
    }

    public async Task LeaveTrainingSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"training_{sessionId}");
        _logger.LogInformation("User {UserId} left training session {SessionId}", 
            Context.UserIdentifier, sessionId);
    }

    public async Task StartPhoneCall(Guid scenarioId)
    {
        try
        {
            var userId = Context.UserIdentifier ?? "demo-user-123";
            var session = await _phoneScenarioService.StartSessionAsync(userId, scenarioId);
            
            await Clients.Caller.SendAsync("CallStarted", new
            {
                SessionId = session.Id,
                ScenarioId = scenarioId,
                CallState = session.CallState.ToString()
            });

            _logger.LogInformation("Phone call started for user {UserId}, session {SessionId}", 
                userId, session.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting phone call for scenario {ScenarioId}", scenarioId);
            await Clients.Caller.SendAsync("Error", "Failed to start phone call");
        }
    }

    public async Task AnswerCall(Guid sessionId)
    {
        try
        {
            var session = await _phoneScenarioService.UpdateSessionAsync(sessionId, CallState.Active);
            
            // Generate initial hacker message
            var initialMessage = await _conversationFlowService.GenerateHackerResponseAsync(session, "Hello, this is customer service. How can I help you today?");
            var responseOptions = await _conversationFlowService.GenerateResponseOptionsAsync(session);
            var alerts = await _conversationFlowService.AnalyzeManipulationTacticsAsync(session, initialMessage);
            var riskLevel = await _conversationFlowService.CalculateCurrentRiskLevelAsync(session);

            await Clients.Caller.SendAsync("CallAnswered", new
            {
                SessionId = sessionId,
                HackerMessage = initialMessage,
                ResponseOptions = responseOptions,
                SecurityAlerts = alerts,
                RiskLevel = riskLevel.ToString()
            });

            _logger.LogInformation("Call answered for session {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error answering call for session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("Error", "Failed to answer call");
        }
    }

    public async Task SendUserResponse(Guid sessionId, string responseId, string responseText)
    {
        try
        {
            var session = await _phoneScenarioService.GetActiveSessionAsync(Context.UserIdentifier ?? "demo-user-123");
            if (session == null || session.Id != sessionId)
            {
                await Clients.Caller.SendAsync("Error", "Invalid session");
                return;
            }

            // Process user response and generate hacker's next message
            var conversationNode = await _conversationFlowService.ProcessUserResponseAsync(session, responseId, responseText);
            var hackerResponse = await _conversationFlowService.GenerateHackerResponseAsync(session, responseText);
            var newResponseOptions = await _conversationFlowService.GenerateResponseOptionsAsync(session);
            var alerts = await _conversationFlowService.AnalyzeManipulationTacticsAsync(session, hackerResponse);
            var riskLevel = await _conversationFlowService.CalculateCurrentRiskLevelAsync(session);

            await Clients.Caller.SendAsync("ConversationUpdate", new
            {
                SessionId = sessionId,
                UserResponse = responseText,
                HackerResponse = hackerResponse,
                ResponseOptions = newResponseOptions,
                SecurityAlerts = alerts,
                RiskLevel = riskLevel.ToString(),
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("User response processed for session {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user response for session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("Error", "Failed to process response");
        }
    }

    public async Task EndCall(Guid sessionId, string reason = "user_ended")
    {
        try
        {
            await _phoneScenarioService.EndSessionAsync(sessionId);

            await Clients.Caller.SendAsync("CallEnded", new
            {
                SessionId = sessionId,
                Reason = reason,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("Call ended for session {SessionId}, reason: {Reason}", sessionId, reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending call for session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("Error", "Failed to end call");
        }
    }

    public async Task RequestCoaching(Guid sessionId, ManipulationTactic tactic)
    {
        try
        {
            var session = await _phoneScenarioService.GetActiveSessionAsync(Context.UserIdentifier ?? "demo-user-123");
            if (session == null || session.Id != sessionId)
            {
                await Clients.Caller.SendAsync("Error", "Invalid session");
                return;
            }

            // Generate contextual coaching tip
            var coachingTip = tactic switch
            {
                ManipulationTactic.Urgency => "⏰ Take your time! Legitimate urgent requests can wait for proper verification. Scammers create false urgency to bypass your security thinking.",
                ManipulationTactic.Authority => "👑 Always verify authority claims through official channels. Real executives understand security protocols and won't pressure you to bypass them.",
                ManipulationTactic.Fear => "😰 Don't let fear drive your decisions. Step back, breathe, and verify the claims through official channels before taking any action.",
                ManipulationTactic.Reciprocity => "🤝 Be wary of unsolicited 'favors' followed by requests. Legitimate business relationships don't operate on guilt or obligation.",
                _ => "🔒 Trust your instincts! When something feels wrong, pause and verify through official channels. It's always better to be cautious."
            };

            await Clients.Caller.SendAsync("CoachingTip", new
            {
                SessionId = sessionId,
                Tactic = tactic.ToString(),
                Tip = coachingTip,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("Coaching tip provided for session {SessionId}, tactic: {Tactic}", sessionId, tactic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error providing coaching for session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("Error", "Failed to provide coaching");
        }
    }

    public async Task EscalateCall(Guid sessionId)
    {
        try
        {
            await Clients.Caller.SendAsync("CallEscalated", new
            {
                SessionId = sessionId,
                Message = "Call has been escalated to your supervisor. This was the correct security action to take when feeling uncertain.",
                Timestamp = DateTime.UtcNow
            });

            await EndCall(sessionId, "escalated");
            
            _logger.LogInformation("Call escalated for session {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error escalating call for session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("Error", "Failed to escalate call");
        }
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected to training hub: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected from training hub: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}