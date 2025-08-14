namespace IncidentIQ.Domain.Enums;

public enum ScenarioType
{
    PhishingEmail = 1,
    SocialEngineeringCall = 2,
    DataRequest = 3,
    USB_Device = 4,
    Password_Attack = 5,
    Insider_Threat = 6,
    Ransomware_Simulation = 7,
    Executive_Impersonation = 8,
    Credential_Harvesting = 9,
    Business_Email_Compromise = 10,
    CustomerServiceCall = 11
}

public enum CallState
{
    Incoming = 1,
    Active = 2,
    OnHold = 3,
    Transferred = 4,
    Ended = 5
}

public enum ManipulationTactic
{
    Authority = 1,
    Urgency = 2,
    Reciprocity = 3,
    SocialProof = 4,
    Fear = 5,
    Scarcity = 6,
    Commitment = 7,
    Liking = 8
}

public enum RiskLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum DifficultyLevel
{
    VeryEasy = 1,
    Easy = 2,
    Medium = 3,
    Hard = 4,
    VeryHard = 5,
    Expert = 6
}