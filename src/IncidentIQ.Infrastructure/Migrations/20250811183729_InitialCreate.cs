using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentIQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SecurityLevel = table.Column<int>(type: "int", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
                    SecurityProfile_VulnerabilityPatterns = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityProfile_LearningPreferences = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityProfile_CompanyContext = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityProfile_ToolsAndSystems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityProfile_ColleagueNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityProfile_RecentProjects = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    TargetSecurityLevel = table.Column<int>(type: "int", nullable: false),
                    TargetRole = table.Column<int>(type: "int", nullable: false),
                    Content_PrimaryContent = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Content_InteractiveElements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content_DecisionPoints = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content_Consequences = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content_LearningObjectives = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonalizationContext_CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PersonalizationContext_ColleagueNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonalizationContext_RelevantSystems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonalizationContext_RoleSpecificContext = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonalizationContext_CompanySpecificDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Configuration_EstimatedDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Configuration_AllowsMultipleAttempts = table.Column<bool>(type: "bit", nullable: false),
                    Configuration_MaxAttempts = table.Column<int>(type: "int", nullable: false),
                    Configuration_ScoringCriteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Configuration_RequiresRealTimeCoaching = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingScenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BehavioralAnalytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetencyProfile_PhishingRecognition = table.Column<double>(type: "float", nullable: false),
                    CompetencyProfile_SocialEngineeringAwareness = table.Column<double>(type: "float", nullable: false),
                    CompetencyProfile_PasswordSecurity = table.Column<double>(type: "float", nullable: false),
                    CompetencyProfile_DataProtection = table.Column<double>(type: "float", nullable: false),
                    CompetencyProfile_IncidentResponse = table.Column<double>(type: "float", nullable: false),
                    CompetencyProfile_ComplianceAwareness = table.Column<double>(type: "float", nullable: false),
                    CompetencyProfile_OverallSecurityAwareness = table.Column<double>(type: "float", nullable: false),
                    CompetencyProfile_LastAssessed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformanceHistory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentRiskProfile_CurrentRiskLevel = table.Column<int>(type: "int", nullable: false),
                    CurrentRiskProfile_IdentifiedVulnerabilities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentRiskProfile_MitigationStrategies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentRiskProfile_LastAssessed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentRiskProfile_ConfidenceScore = table.Column<double>(type: "float", nullable: false),
                    LearningBehavior_PreferredLearningStyle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LearningBehavior_EngagementLevel = table.Column<double>(type: "float", nullable: false),
                    LearningBehavior_AverageSessionDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    LearningBehavior_StrongAreas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LearningBehavior_ChallengeAreas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LearningBehavior_RespondsWellToCoaching = table.Column<bool>(type: "bit", nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehavioralAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BehavioralAnalytics_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingScenarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false),
                    Responses = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scoring_OverallScore = table.Column<double>(type: "float", nullable: false),
                    Scoring_CategoryScores = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scoring_CorrectResponses = table.Column<int>(type: "int", nullable: false),
                    Scoring_TotalResponses = table.Column<int>(type: "int", nullable: false),
                    Scoring_StrengthAreas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scoring_ImprovementAreas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scoring_FeedbackSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CoachingInteractions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_TrainingScenarios_TrainingScenarioId",
                        column: x => x.TrainingScenarioId,
                        principalTable: "TrainingScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgentInteractions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrainingSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrainingScenarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AgentType = table.Column<int>(type: "int", nullable: false),
                    AgentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InteractionType = table.Column<int>(type: "int", nullable: false),
                    Input = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Output = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessingTimeMs = table.Column<int>(type: "int", nullable: false),
                    ConfidenceScore = table.Column<double>(type: "float", nullable: false),
                    WasSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InputTokens = table.Column<int>(type: "int", nullable: false),
                    OutputTokens = table.Column<int>(type: "int", nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentInteractions_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgentInteractions_TrainingScenarios_TrainingScenarioId",
                        column: x => x.TrainingScenarioId,
                        principalTable: "TrainingScenarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgentInteractions_TrainingSessions_TrainingSessionId",
                        column: x => x.TrainingSessionId,
                        principalTable: "TrainingSessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentInteractions_TrainingScenarioId",
                table: "AgentInteractions",
                column: "TrainingScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentInteractions_TrainingSessionId",
                table: "AgentInteractions",
                column: "TrainingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentInteractions_UserId",
                table: "AgentInteractions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BehavioralAnalytics_UserId",
                table: "BehavioralAnalytics",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_TrainingScenarioId",
                table: "TrainingSessions",
                column: "TrainingScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_UserId",
                table: "TrainingSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentInteractions");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BehavioralAnalytics");

            migrationBuilder.DropTable(
                name: "TrainingSessions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "TrainingScenarios");
        }
    }
}
