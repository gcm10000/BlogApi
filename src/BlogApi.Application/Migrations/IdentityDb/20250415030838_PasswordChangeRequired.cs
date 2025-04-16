using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.Application.Migrations.IdentityDb
{
    /// <inheritdoc />
    public partial class PasswordChangeRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PasswordChangeRequired",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordChangeRequired",
                table: "AspNetUsers");
        }
    }
}
