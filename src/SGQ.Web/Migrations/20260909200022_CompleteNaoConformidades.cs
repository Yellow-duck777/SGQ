using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class CompleteNaoConformidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AprovadaGq",
                table: "NaoConformidades",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AprovadaRt",
                table: "NaoConformidades",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CausaProvavel",
                table: "NaoConformidades",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CausaRaiz",
                table: "NaoConformidades",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contencao",
                table: "NaoConformidades",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eficaz",
                table: "NaoConformidades",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EncerradaEm",
                table: "NaoConformidades",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Investigacao",
                table: "NaoConformidades",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetodoAnalise",
                table: "NaoConformidades",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEncerramento",
                table: "NaoConformidades",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AcoesNaoConformidade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NaoConformidadeId = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Responsavel = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Prazo = table.Column<DateOnly>(type: "date", nullable: false),
                    Obrigatoria = table.Column<bool>(type: "boolean", nullable: false),
                    DataConclusao = table.Column<DateOnly>(type: "date", nullable: true),
                    Evidencia = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcoesNaoConformidade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcoesNaoConformidade_NaoConformidades_NaoConformidadeId",
                        column: x => x.NaoConformidadeId,
                        principalTable: "NaoConformidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcoesNaoConformidade_NaoConformidadeId",
                table: "AcoesNaoConformidade",
                column: "NaoConformidadeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcoesNaoConformidade");

            migrationBuilder.DropColumn(
                name: "AprovadaGq",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "AprovadaRt",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "CausaProvavel",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "CausaRaiz",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "Contencao",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "Eficaz",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "EncerradaEm",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "Investigacao",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "MetodoAnalise",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "UsuarioEncerramento",
                table: "NaoConformidades");
        }
    }
}
