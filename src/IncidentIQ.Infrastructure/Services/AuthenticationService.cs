using IncidentIQ.Application.Interfaces;
using IncidentIQ.Domain.Entities;
using IncidentIQ.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IncidentIQ.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<AuthenticationService> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<AuthenticationResult> RegisterUserAsync(UserRegistrationModel model)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Create ApplicationUser (Identity)
            var applicationUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CompletedOnboarding = false
            };

            // Create the domain User entity
            var user = new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Company = model.Company,
                Department = model.Department,
                Role = model.Role,
                AccessLevel = model.AccessLevel,
                SecurityLevel = model.SecurityLevel,
                SecurityProfile = new SecurityProfile()
            };

            // Add User to context
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            // Link ApplicationUser to User
            applicationUser.AppUserId = user.Id;

            await transaction.CommitAsync();

            _logger.LogInformation("Successfully created user profile for {Email} with role {Role}", 
                model.Email, model.Role);

            return AuthenticationResult.SuccessResult((object)applicationUser, user);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating user profile for {Email}", model.Email);
            return AuthenticationResult.FailureResult("Failed to create user profile");
        }
    }

    public async Task<AuthenticationResult> CreateUserProfileAsync(object applicationUserObj, UserRegistrationModel model)
    {
        if (applicationUserObj is not ApplicationUser applicationUser)
        {
            return AuthenticationResult.FailureResult("Invalid application user type");
        }
        
        try
        {
            // Create the domain User entity
            var user = new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Company = model.Company,
                Department = model.Department,
                Role = model.Role,
                AccessLevel = model.AccessLevel,
                SecurityLevel = model.SecurityLevel,
                SecurityProfile = new SecurityProfile
                {
                    CompanyContext = new Dictionary<string, object>
                    {
                        { "CompanySize", DetermineCompanySize(model.Company) },
                        { "Industry", DetermineIndustry(model.Company) },
                        { "Department", model.Department }
                    },
                    VulnerabilityPatterns = InitializeVulnerabilityPatterns(model.Role),
                    LearningPreferences = new Dictionary<string, object>
                    {
                        { "SecurityLevel", model.SecurityLevel.ToString() },
                        { "PreferredTrainingDuration", GetPreferredDurationByRole(model.Role) },
                        { "RoleSpecificFocus", GetRoleSpecificFocus(model.Role) }
                    }
                }
            };

            // Add User to context
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            // Update ApplicationUser to link to User
            applicationUser.AppUserId = user.Id;
            var updateResult = await _userManager.UpdateAsync(applicationUser);
            
            if (!updateResult.Succeeded)
            {
                _logger.LogWarning("Failed to link ApplicationUser to User for {Email}: {Errors}", 
                    model.Email, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }

            _logger.LogInformation("Successfully created user profile for {Email} with role {Role}", 
                model.Email, model.Role);

            return AuthenticationResult.SuccessResult((object)applicationUser, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user profile for {Email}", model.Email);
            return AuthenticationResult.FailureResult("Failed to create user profile");
        }
    }

    private static Dictionary<string, object> InitializeVulnerabilityPatterns(Domain.Enums.UserRole role)
    {
        return role switch
        {
            Domain.Enums.UserRole.Developer => new Dictionary<string, object>
            {
                { "CodeReviewVulnerabilities", new[] { "SQL Injection", "XSS", "Insecure Dependencies" } },
                { "GitHubSecurity", new[] { "Exposed API Keys", "Public Repository Risks" } },
                { "DevelopmentTools", new[] { "IDE Security", "Package Manager Security" } }
            },
            Domain.Enums.UserRole.HR => new Dictionary<string, object>
            {
                { "PersonalDataHandling", new[] { "PII Protection", "Employee Data Security" } },
                { "SocialEngineering", new[] { "Employee Impersonation", "Fake Job Applications" } },
                { "ComplianceRisks", new[] { "GDPR Violations", "Background Check Fraud" } }
            },
            Domain.Enums.UserRole.Executive => new Dictionary<string, object>
            {
                { "SpearPhishing", new[] { "CEO Fraud", "Business Email Compromise" } },
                { "StrategicRisks", new[] { "Insider Threats", "Corporate Espionage" } },
                { "ComplianceOversight", new[] { "Regulatory Violations", "Data Breach Liability" } }
            },
            Domain.Enums.UserRole.Finance => new Dictionary<string, object>
            {
                { "FinancialFraud", new[] { "Invoice Fraud", "Payment Redirection", "Wire Transfer Fraud" } },
                { "DataSecurity", new[] { "Financial Records Protection", "Payment Card Security" } },
                { "ComplianceRisks", new[] { "SOX Violations", "Financial Data Breaches" } }
            },
            _ => new Dictionary<string, object>
            {
                { "GeneralSecurity", new[] { "Phishing", "Social Engineering", "Password Security" } },
                { "DataProtection", new[] { "Sensitive Information Handling", "Access Control" } }
            }
        };
    }

    private static string GetRoleSpecificFocus(Domain.Enums.UserRole role)
    {
        return role switch
        {
            Domain.Enums.UserRole.Developer => "Code Security & DevSecOps",
            Domain.Enums.UserRole.HR => "Personal Data Protection & Social Engineering",
            Domain.Enums.UserRole.Executive => "Strategic Security & Business Continuity",
            Domain.Enums.UserRole.Finance => "Financial Fraud Prevention & Compliance",
            Domain.Enums.UserRole.Marketing => "Brand Protection & Customer Data Security",
            Domain.Enums.UserRole.Sales => "Customer Data Protection & CRM Security",
            Domain.Enums.UserRole.IT => "Infrastructure Security & System Administration",
            _ => "General Security Awareness"
        };
    }

    private static int GetPreferredDurationByRole(Domain.Enums.UserRole role)
    {
        return role switch
        {
            Domain.Enums.UserRole.Executive => 10, // Shorter sessions for executives
            Domain.Enums.UserRole.Developer => 20, // Longer technical sessions
            Domain.Enums.UserRole.IT => 20,
            _ => 15 // Standard duration
        };
    }

    private static string DetermineCompanySize(string companyName)
    {
        // In a real application, you might have a service that determines company size
        // For now, we'll return a placeholder
        return "Unknown";
    }

    private static string DetermineIndustry(string companyName)
    {
        // In a real application, you might have a service that determines industry
        // For now, we'll return a placeholder
        return "Unknown";
    }
}