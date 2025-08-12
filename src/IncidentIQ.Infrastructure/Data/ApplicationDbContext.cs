using IncidentIQ.Domain.Common;
using IncidentIQ.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace IncidentIQ.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> AppUsers { get; set; } = null!;
    public DbSet<TrainingScenario> TrainingScenarios { get; set; } = null!;
    public DbSet<TrainingSession> TrainingSessions { get; set; } = null!;
    public DbSet<BehavioralAnalytics> BehavioralAnalytics { get; set; } = null!;
    public DbSet<AgentInteraction> AgentInteractions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureTrainingScenario(modelBuilder);
        ConfigureTrainingSession(modelBuilder);
        ConfigureBehavioralAnalytics(modelBuilder);
        ConfigureAgentInteraction(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<User>();
        
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Email).HasMaxLength(320).IsRequired();
        entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        entity.Property(u => u.Company).HasMaxLength(200);
        entity.Property(u => u.Department).HasMaxLength(100);

        // Configure SecurityProfile as JSON
        entity.OwnsOne(u => u.SecurityProfile, sp =>
        {
            sp.Property(p => p.VulnerabilityPatterns)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
            
            sp.Property(p => p.LearningPreferences)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
            
            sp.Property(p => p.CompanyContext)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
            
            sp.Property(p => p.ToolsAndSystems)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
            
            sp.Property(p => p.ColleagueNames)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());
            
            sp.Property(p => p.RecentProjects)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());
        });

        entity.HasMany(u => u.TrainingSessions)
            .WithOne(ts => ts.User)
            .HasForeignKey(ts => ts.UserId);

        entity.HasMany(u => u.BehavioralAnalytics)
            .WithOne(ba => ba.User)
            .HasForeignKey(ba => ba.UserId);
    }

    private static void ConfigureTrainingScenario(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<TrainingScenario>();
        
        entity.HasKey(ts => ts.Id);
        entity.Property(ts => ts.Title).HasMaxLength(200).IsRequired();
        entity.Property(ts => ts.Description).HasMaxLength(1000);

        // Configure ScenarioContent as JSON
        entity.OwnsOne(ts => ts.Content, sc =>
        {
            sc.Property(c => c.PrimaryContent).HasMaxLength(5000);
            sc.Property(c => c.InteractiveElements)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
            
            sc.Property(c => c.DecisionPoints)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<DecisionPoint>>(v, (JsonSerializerOptions?)null) ?? new());
            
            sc.Property(c => c.Consequences)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new());
            
            sc.Property(c => c.LearningObjectives)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());
        });

        // Configure PersonalizationData as JSON
        entity.OwnsOne(ts => ts.PersonalizationContext, pd =>
        {
            pd.Property(p => p.CompanyName).HasMaxLength(200);
            pd.Property(p => p.ColleagueNames)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());
            
            pd.Property(p => p.RelevantSystems)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());
            
            pd.Property(p => p.RoleSpecificContext)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
            
            pd.Property(p => p.CompanySpecificDetails)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
        });

        // Configure ScenarioConfiguration as JSON
        entity.OwnsOne(ts => ts.Configuration, sc =>
        {
            sc.Property(c => c.ScoringCriteria)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
        });
    }

    private static void ConfigureTrainingSession(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<TrainingSession>();
        
        entity.HasKey(ts => ts.Id);
        
        entity.Property(ts => ts.Responses)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<UserResponse>>(v, (JsonSerializerOptions?)null) ?? new());

        entity.OwnsOne(ts => ts.Scoring, s =>
        {
            s.Property(sc => sc.CategoryScores)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, double>>(v, (JsonSerializerOptions?)null) ?? new());
            
            s.Property(sc => sc.StrengthAreas)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());
            
            s.Property(sc => sc.ImprovementAreas)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());

            s.Property(sc => sc.FeedbackSummary).HasMaxLength(2000);
        });

        entity.Property(ts => ts.CoachingInteractions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<CoachingInteraction>>(v, (JsonSerializerOptions?)null) ?? new());

        entity.HasOne(ts => ts.TrainingScenario)
            .WithMany(sc => sc.TrainingSessions)
            .HasForeignKey(ts => ts.TrainingScenarioId);
    }

    private static void ConfigureBehavioralAnalytics(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<BehavioralAnalytics>();
        
        entity.HasKey(ba => ba.Id);

        entity.OwnsOne(ba => ba.CompetencyProfile);
        
        entity.Property(ba => ba.PerformanceHistory)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<PerformanceMetric>>(v, (JsonSerializerOptions?)null) ?? new());

        entity.OwnsOne(ba => ba.CurrentRiskProfile, rp =>
        {
            rp.Property(r => r.IdentifiedVulnerabilities)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());
            
            rp.Property(r => r.MitigationStrategies)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());
        });

        entity.OwnsOne(ba => ba.LearningBehavior, lb =>
        {
            lb.Property(l => l.PreferredLearningStyle).HasMaxLength(100);
            lb.Property(l => l.StrongAreas)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());
            
            lb.Property(l => l.ChallengeAreas)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());
        });

        entity.Property(ba => ba.Recommendations)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<ImprovementRecommendation>>(v, (JsonSerializerOptions?)null) ?? new());
    }

    private static void ConfigureAgentInteraction(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<AgentInteraction>();
        
        entity.HasKey(ai => ai.Id);
        entity.Property(ai => ai.AgentName).HasMaxLength(100).IsRequired();
        entity.Property(ai => ai.Input).HasMaxLength(5000);
        entity.Property(ai => ai.Output).HasMaxLength(5000);

        entity.Property(ai => ai.Context)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
        
        entity.HasOne(ai => ai.User)
            .WithMany()
            .HasForeignKey(ai => ai.UserId)
            .IsRequired(false);

        entity.HasOne(ai => ai.TrainingSession)
            .WithMany(ts => ts.AgentInteractions)
            .HasForeignKey(ai => ai.TrainingSessionId)
            .IsRequired(false);

        entity.HasOne(ai => ai.TrainingScenario)
            .WithMany(sc => sc.AgentInteractions)
            .HasForeignKey(ai => ai.TrainingScenarioId)
            .IsRequired(false);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var baseEntity = (BaseEntity)entity.Entity;
            
            if (entity.State == EntityState.Added)
            {
                baseEntity.CreatedAt = DateTime.UtcNow;
            }
            
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }
    }
}