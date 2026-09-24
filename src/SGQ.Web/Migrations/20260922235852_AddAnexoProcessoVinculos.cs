using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAnexoProcessoVinculos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnexosProcessosVinculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnexoId = table.Column<int>(type: "integer", nullable: false),
                    ReclamacaoClienteId = table.Column<int>(type: "integer", nullable: true),
                    NaoConformidadeId = table.Column<int>(type: "integer", nullable: true),
                    RecallId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnexosProcessosVinculos", x => x.Id);
                    table.CheckConstraint("CK_AnexosProcessosVinculos_UmProcesso", "(\"ReclamacaoClienteId\" IS NOT NULL)::integer + (\"NaoConformidadeId\" IS NOT NULL)::integer + (\"RecallId\" IS NOT NULL)::integer = 1");
                    table.ForeignKey(
                        name: "FK_AnexosProcessosVinculos_Anexos_AnexoId",
                        column: x => x.AnexoId,
                        principalTable: "Anexos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnexosProcessosVinculos_NaoConformidades_NaoConformidadeId",
                        column: x => x.NaoConformidadeId,
                        principalTable: "NaoConformidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnexosProcessosVinculos_Recalls_RecallId",
                        column: x => x.RecallId,
                        principalTable: "Recalls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnexosProcessosVinculos_ReclamacoesClientes_ReclamacaoClien~",
                        column: x => x.ReclamacaoClienteId,
                        principalTable: "ReclamacoesClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnexosProcessosVinculos_AnexoId_NaoConformidadeId",
                table: "AnexosProcessosVinculos",
                columns: new[] { "AnexoId", "NaoConformidadeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnexosProcessosVinculos_AnexoId_RecallId",
                table: "AnexosProcessosVinculos",
                columns: new[] { "AnexoId", "RecallId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnexosProcessosVinculos_AnexoId_ReclamacaoClienteId",
                table: "AnexosProcessosVinculos",
                columns: new[] { "AnexoId", "ReclamacaoClienteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnexosProcessosVinculos_NaoConformidadeId",
                table: "AnexosProcessosVinculos",
                column: "NaoConformidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_AnexosProcessosVinculos_RecallId",
                table: "AnexosProcessosVinculos",
                column: "RecallId");

            migrationBuilder.CreateIndex(
                name: "IX_AnexosProcessosVinculos_ReclamacaoClienteId",
                table: "AnexosProcessosVinculos",
                column: "ReclamacaoClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnexosProcessosVinculos");
        }
    }
}
