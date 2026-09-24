using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddReaberturaReclamacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JustificativaReabertura",
                table: "ReclamacoesClientes",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReabertaEm",
                table: "ReclamacoesClientes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusAnteriorReabertura",
                table: "ReclamacoesClientes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioReabertura",
                table: "ReclamacoesClientes",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JustificativaReabertura",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "ReabertaEm",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "StatusAnteriorReabertura",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "UsuarioReabertura",
                table: "ReclamacoesClientes");
        }
    }
}
