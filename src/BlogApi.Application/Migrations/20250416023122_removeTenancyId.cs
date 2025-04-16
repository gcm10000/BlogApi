using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.Application.Migrations
{
    /// <inheritdoc />
    public partial class removeTenancyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostViews_Tenancies_TenancyId",
                table: "PostViews");

            migrationBuilder.DropIndex(
                name: "IX_PostViews_TenancyId",
                table: "PostViews");

            migrationBuilder.DropColumn(
                name: "TenancyId",
                table: "PostViews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenancyId",
                table: "PostViews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PostViews_TenancyId",
                table: "PostViews",
                column: "TenancyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostViews_Tenancies_TenancyId",
                table: "PostViews",
                column: "TenancyId",
                principalTable: "Tenancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
