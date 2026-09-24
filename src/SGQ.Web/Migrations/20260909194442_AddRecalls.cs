using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddRecalls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    SequenciaAnual = table.Column<int>(type: "integer", nullable: false),
                    Origem = table.Column<string>(type: "text", nullable: false),
                    NaoConformidadeId = table.Column<int>(type: "integer", nullable: true),
                    ReclamacaoClienteId = table.Column<int>(type: "integer", nullable: true),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    LoteId = table.Column<int>(type: "integer", nullable: false),
                    DataAbertura = table.Column<DateOnly>(type: "date", nullable: false),
                    DataFabricacao = table.Column<DateOnly>(type: "date", nullable: true),
                    DataValidade = table.Column<DateOnly>(type: "date", nullable: true),
                    QuantidadeProduzida = table.Column<decimal>(type: "numeric", nullable: false),
                    QuantidadeEstoque = table.Column<decimal>(type: "numeric", nullable: false),
                    QuantidadeDistribuida = table.Column<decimal>(type: "numeric", nullable: false),
                    ClientesEnvolvidos = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    NaturezaOcorrencia = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    RiscoPotencial = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Decisao = table.Column<string>(type: "text", nullable: false),
                    JustificativaDecisao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    UsuarioAbertura = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CriadaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recalls_Lotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "Lotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recalls_NaoConformidades_NaoConformidadeId",
                        column: x => x.NaoConformidadeId,
                        principalTable: "NaoConformidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recalls_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recalls_ReclamacoesClientes_ReclamacaoClienteId",
                        column: x => x.ReclamacaoClienteId,
                        principalTable: "ReclamacoesClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_Ano_SequenciaAnual",
                table: "Recalls",
                columns: new[] { "Ano", "SequenciaAnual" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_Codigo",
                table: "Recalls",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_LoteId",
                table: "Recalls",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_NaoConformidadeId",
                table: "Recalls",
                column: "NaoConformidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_ProdutoId",
                table: "Recalls",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_ReclamacaoClienteId",
                table: "Recalls",
                column: "ReclamacaoClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recalls");
        }
    }
}
