using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.Application.Migrations
{
    /// <inheritdoc />
    public partial class TenancyIdSlugIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_Posts_Slug",
            //    table: "Posts");

            // ❌ Remova essa parte abaixo:
            // migrationBuilder.DropIndex(
            //     name: "IX_Posts_TenancyId",
            //     table: "Posts");

            // ✅ Adicione índice único composto:
            migrationBuilder.CreateIndex(
                name: "IX_Posts_TenancyId_Slug",
                table: "Posts",
                columns: new[] { "TenancyId", "Slug" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_TenancyId_Slug",
                table: "Posts");

            // ✅ Restaure o índice de Slug
            migrationBuilder.CreateIndex(
                name: "IX_Posts_Slug",
                table: "Posts",
                column: "Slug",
                unique: true);

            // ❌ Não adicione o índice de TenancyId novamente, pois ele já existe por ser chave estrangeira
            // migrationBuilder.CreateIndex(
            //     name: "IX_Posts_TenancyId",
            //     table: "Posts",
            //     column: "TenancyId");
        }
    }
}
