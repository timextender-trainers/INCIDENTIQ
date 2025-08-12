namespace IncidentIQ.Domain.Enums;

public enum SessionStatus
{
    NotStarted = 1,
    InProgress = 2,
    Paused = 3,
    Completed = 4,
    Abandoned = 5,
    Failed = 6
}

public enum CoachingType
{
    Hint = 1,
    Warning = 2,
    Encouragement = 3,
    Explanation = 4,
    BestPractice = 5,
    RealWorldExample = 6
}