using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.MinimalAPI.Migrations
{
    /// <inheritdoc />
    public partial class FKRelationsPt2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_UserDTO_UserId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDTO",
                table: "UserDTO");

            migrationBuilder.RenameTable(
                name: "UserDTO",
                newName: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Transactions",
                newName: "UserDTOId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                newName: "IX_Transactions_UserDTOId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UserDTOId",
                table: "Transactions",
                column: "UserDTOId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UserDTOId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "UserDTO");

            migrationBuilder.RenameColumn(
                name: "UserDTOId",
                table: "Transactions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_UserDTOId",
                table: "Transactions",
                newName: "IX_Transactions_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDTO",
                table: "UserDTO",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_UserDTO_UserId",
                table: "Transactions",
                column: "UserId",
                principalTable: "UserDTO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
