using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddProrrogacoesPrazo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProrrogacoesPrazo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReclamacaoClienteId = table.Column<int>(type: "integer", nullable: true),
                    NaoConformidadeId = table.Column<int>(type: "integer", nullable: true),
                    RecallId = table.Column<int>(type: "integer", nullable: true),
                    DataAnterior = table.Column<DateOnly>(type: "date", nullable: false),
                    NovaData = table.Column<DateOnly>(type: "date", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ClienteComunicado = table.Column<bool>(type: "boolean", nullable: false),
                    RegistroComunicacaoCliente = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Usuario = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RegistradaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProrrogacoesPrazo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProrrogacoesPrazo_NaoConformidades_NaoConformidadeId",
                        column: x => x.NaoConformidadeId,
                        principalTable: "NaoConformidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProrrogacoesPrazo_Recalls_RecallId",
                        column: x => x.RecallId,
                        principalTable: "Recalls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProrrogacoesPrazo_ReclamacoesClientes_ReclamacaoClienteId",
                        column: x => x.ReclamacaoClienteId,
                        principalTable: "ReclamacoesClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProrrogacoesPrazo_NaoConformidadeId",
                table: "ProrrogacoesPrazo",
                column: "NaoConformidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProrrogacoesPrazo_RecallId",
                table: "ProrrogacoesPrazo",
                column: "RecallId");

            migrationBuilder.CreateIndex(
                name: "IX_ProrrogacoesPrazo_ReclamacaoClienteId",
                table: "ProrrogacoesPrazo",
                column: "ReclamacaoClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProrrogacoesPrazo");
        }
    }
}
