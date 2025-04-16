using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddTenancyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenancyId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenancyId",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenancyId",
                table: "Authors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tenancies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenancies", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_TenancyId",
                table: "Posts",
                column: "TenancyId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenancyId",
                table: "Categories",
                column: "TenancyId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_TenancyId",
                table: "Authors",
                column: "TenancyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Tenancies_TenancyId",
                table: "Authors",
                column: "TenancyId",
                principalTable: "Tenancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Tenancies_TenancyId",
                table: "Categories",
                column: "TenancyId",
                principalTable: "Tenancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Tenancies_TenancyId",
                table: "Posts",
                column: "TenancyId",
                principalTable: "Tenancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Tenancies_TenancyId",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Tenancies_TenancyId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Tenancies_TenancyId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Tenancies");

            migrationBuilder.DropIndex(
                name: "IX_Posts_TenancyId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Categories_TenancyId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Authors_TenancyId",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "TenancyId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "TenancyId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "TenancyId",
                table: "Authors");
        }
    }
}
