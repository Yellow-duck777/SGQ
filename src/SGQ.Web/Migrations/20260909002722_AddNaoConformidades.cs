using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddNaoConformidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NaoConformidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    SequenciaAnual = table.Column<int>(type: "integer", nullable: false),
                    Origem = table.Column<string>(type: "text", nullable: false),
                    ReclamacaoClienteId = table.Column<int>(type: "integer", nullable: true),
                    DataAbertura = table.Column<DateOnly>(type: "date", nullable: false),
                    Area = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: true),
                    Descricao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Classificacao = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    UsuarioAbertura = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CriadaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NaoConformidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NaoConformidades_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NaoConformidades_ReclamacoesClientes_ReclamacaoClienteId",
                        column: x => x.ReclamacaoClienteId,
                        principalTable: "ReclamacoesClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NaoConformidades_Ano_SequenciaAnual",
                table: "NaoConformidades",
                columns: new[] { "Ano", "SequenciaAnual" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NaoConformidades_Codigo",
                table: "NaoConformidades",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NaoConformidades_ProdutoId",
                table: "NaoConformidades",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_NaoConformidades_ReclamacaoClienteId",
                table: "NaoConformidades",
                column: "ReclamacaoClienteId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NaoConformidades");
        }
    }
}
