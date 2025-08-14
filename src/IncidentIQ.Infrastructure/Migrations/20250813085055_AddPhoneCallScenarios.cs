using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentIQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneCallScenarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhoneCallScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    CallerProfile_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CallerProfile_Company = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CallerProfile_PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CallerProfile_Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CallerProfile_Persona = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CallerProfile_Background = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationFlow_InitialNode_Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConversationFlow_InitialNode_HackerMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ConversationFlow_InitialNode_UserOptions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationFlow_InitialNode_PrimaryTactic = table.Column<int>(type: "int", nullable: false),
                    ConversationFlow_InitialNode_RiskLevel = table.Column<int>(type: "int", nullable: false),
                    ConversationFlow_InitialNode_NextNodeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConversationFlow_InitialNode_Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationFlow_Nodes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationFlow_SuccessConditions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationFlow_FailureConsequences = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlannedTactics = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LearningObjectives = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetCompany = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TargetRole = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanyContext = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneCallScenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhoneCallSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CallState = table.Column<int>(type: "int", nullable: false),
                    CallStartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CallEndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CallDurationSeconds = table.Column<int>(type: "int", nullable: false),
                    Exchanges = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentNodeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TacticsUsed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlertsTriggered = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scoring_OverallScore = table.Column<double>(type: "float", nullable: false),
                    Scoring_CorrectResponses = table.Column<int>(type: "int", nullable: false),
                    Scoring_TotalResponses = table.Column<int>(type: "int", nullable: false),
                    Scoring_TacticResistance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scoring_StrengthAreas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scoring_VulnerabilityAreas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scoring_AverageResponseTime = table.Column<double>(type: "float", nullable: false),
                    Scoring_FeedbackSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneCallSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhoneCallSessions_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhoneCallSessions_PhoneCallScenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "PhoneCallScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhoneCallSessions_ScenarioId",
                table: "PhoneCallSessions",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneCallSessions_UserId",
                table: "PhoneCallSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhoneCallSessions");

            migrationBuilder.DropTable(
                name: "PhoneCallScenarios");
        }
    }
}
