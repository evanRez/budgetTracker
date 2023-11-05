using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.MinimalAPI.Migrations
{
    /// <inheritdoc />
    public partial class removeExtraUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Auth0UserId",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Auth0UserId",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
