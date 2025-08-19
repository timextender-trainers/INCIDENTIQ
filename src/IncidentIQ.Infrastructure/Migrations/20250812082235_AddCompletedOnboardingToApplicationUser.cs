using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentIQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletedOnboardingToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CompletedOnboarding",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedOnboarding",
                table: "AspNetUsers");
        }
    }
}
