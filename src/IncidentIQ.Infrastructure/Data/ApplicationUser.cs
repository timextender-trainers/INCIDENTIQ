using Microsoft.AspNetCore.Identity;

namespace IncidentIQ.Infrastructure.Data;

public class ApplicationUser : IdentityUser
{
    public Guid? AppUserId { get; set; } // Links to our custom User entity
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool CompletedOnboarding { get; set; } = false;
}

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}