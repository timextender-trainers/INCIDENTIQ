using IncidentIQ.Domain.Common;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public SecurityLevel SecurityLevel { get; set; } = SecurityLevel.Beginner;
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Standard;
    
    // AI-driven user profile data
    public SecurityProfile SecurityProfile { get; set; } = new();
    
    // Navigation properties
    public ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
    public ICollection<BehavioralAnalytics> BehavioralAnalytics { get; set; } = new List<BehavioralAnalytics>();
}

public class SecurityProfile
{
    public Dictionary<string, object> VulnerabilityPatterns { get; set; } = new();
    public Dictionary<string, object> LearningPreferences { get; set; } = new();
    public Dictionary<string, object> CompanyContext { get; set; } = new();
    public Dictionary<string, object> ToolsAndSystems { get; set; } = new();
    public List<string> ColleagueNames { get; set; } = new();
    public List<string> RecentProjects { get; set; } = new();
}