using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class CompleteRecallWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AprovadaGq",
                table: "Recalls",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AprovadaRt",
                table: "Recalls",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AutoridadeSanitaria",
                table: "Recalls",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BloqueioRegistrado",
                table: "Recalls",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ComunicacaoClientes",
                table: "Recalls",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ComunicadaAutoridadeEm",
                table: "Recalls",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Destinacao",
                table: "Recalls",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EncerradaEm",
                table: "Recalls",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenciaDestinacao",
                table: "Recalls",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProtocoloAutoridade",
                table: "Recalls",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEncerramento",
                table: "Recalls",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RetornosRecall",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecallId = table.Column<int>(type: "integer", nullable: false),
                    Cliente = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric", nullable: false),
                    DataRetorno = table.Column<DateOnly>(type: "date", nullable: false),
                    CondicaoEmbalagem = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CondicaoLacre = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Avarias = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DocumentoTransporte = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AvaliacaoGq = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetornosRecall", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetornosRecall_Recalls_RecallId",
                        column: x => x.RecallId,
                        principalTable: "Recalls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RetornosRecall_RecallId",
                table: "RetornosRecall",
                column: "RecallId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetornosRecall");

            migrationBuilder.DropColumn(
                name: "AprovadaGq",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "AprovadaRt",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "AutoridadeSanitaria",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "BloqueioRegistrado",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "ComunicacaoClientes",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "ComunicadaAutoridadeEm",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "Destinacao",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "EncerradaEm",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "EvidenciaDestinacao",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "ProtocoloAutoridade",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "UsuarioEncerramento",
                table: "Recalls");
        }
    }
}
