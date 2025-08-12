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
    Business_Email_Compromise = 10
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