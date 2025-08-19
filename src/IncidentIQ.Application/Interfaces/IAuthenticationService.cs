using IncidentIQ.Domain.Entities;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> RegisterUserAsync(UserRegistrationModel model);
    Task<AuthenticationResult> CreateUserProfileAsync(object applicationUser, UserRegistrationModel model);
}

public class UserRegistrationModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public AccessLevel AccessLevel { get; set; }
    public SecurityLevel SecurityLevel { get; set; }
}

public class AuthenticationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public object? ApplicationUser { get; set; }
    public User? User { get; set; }
    
    public static AuthenticationResult SuccessResult(object applicationUser, User user)
    {
        return new AuthenticationResult
        {
            Success = true,
            ApplicationUser = applicationUser,
            User = user
        };
    }
    
    public static AuthenticationResult FailureResult(string errorMessage)
    {
        return new AuthenticationResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}