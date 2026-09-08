using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class RestringeExclusaoDeProdutoComLotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lotes_Produtos_ProdutoId",
                table: "Lotes");

            migrationBuilder.AddForeignKey(
                name: "FK_Lotes_Produtos_ProdutoId",
                table: "Lotes",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lotes_Produtos_ProdutoId",
                table: "Lotes");

            migrationBuilder.AddForeignKey(
                name: "FK_Lotes_Produtos_ProdutoId",
                table: "Lotes",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
