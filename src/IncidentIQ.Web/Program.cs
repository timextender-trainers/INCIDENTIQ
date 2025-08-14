using IncidentIQ.Application.Interfaces;
using IncidentIQ.Application.Interfaces.AI;
using IncidentIQ.Application.Interfaces.AI.Agents;
using IncidentIQ.Infrastructure.AI.Agents;
using IncidentIQ.Infrastructure.Data;
using IncidentIQ.Infrastructure.Services;
using IncidentIQ.Web.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Temporarily disabled due to Razor generator conflicts
// builder.Services.AddRazorComponents()
//     .AddInteractiveServerComponents();

// Add Entity Framework with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Use In-Memory database for demo purposes
    options.UseInMemoryDatabase("IncidentIQDemo");
    // Use SQL Server for production: options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Development-only: Relaxed password requirements for easy testing
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add Redis caching for session management and AI memory
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Add memory cache as fallback
builder.Services.AddMemoryCache();

// Add SignalR
builder.Services.AddSignalR();

// Add Semantic Kernel and AI Agents
builder.Services.AddScoped<ISemanticKernelService, SemanticKernelService>();
builder.Services.AddScoped<IScenarioGeneratorAgent, ScenarioGeneratorAgent>();
builder.Services.AddScoped<ICoachingAgent, CoachingAgent>();

// Add User Profile Service
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

// Add Personalized Scenario Service
builder.Services.AddScoped<IPersonalizedScenarioService, PersonalizedScenarioService>();

// Add Authentication Service
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Add Phone Scenario Services
builder.Services.AddScoped<IPhoneScenarioService, PhoneScenarioService>();
builder.Services.AddScoped<IConversationFlowService, ConversationFlowService>();
builder.Services.AddScoped<ISocialEngineeringAnalyzer, SocialEngineeringAnalyzer>();
builder.Services.AddScoped<IConversationCacheService, ConversationCacheService>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Add authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Add health check endpoint
app.MapHealthChecks("/health");

// Add SignalR hub
app.MapHub<TrainingHub>("/traininghub");

// Add MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Comment out Razor components temporarily due to generator issues
// app.MapRazorComponents<App>()
//     .AddInteractiveServerRenderMode();

// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Apply any pending migrations
        context.Database.Migrate();
        
        // Seed initial data if needed
        await SeedDatabase(context);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        
        // In development, create the database if migration fails
        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation("Attempting to create database...");
            context.Database.EnsureCreated();
        }
    }
}

async Task SeedDatabase(ApplicationDbContext context)
{
    // Seed initial data if the database is empty
    if (!context.Users.Any())
    {
        // Add any initial data seeding logic here
        await context.SaveChangesAsync();
    }
}

app.Run();
