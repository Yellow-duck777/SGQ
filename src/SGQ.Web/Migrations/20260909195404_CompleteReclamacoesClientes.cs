using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class CompleteReclamacoesClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DataRespostaCliente",
                table: "ReclamacoesClientes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EncerradaEm",
                table: "ReclamacoesClientes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Investigacao",
                table: "ReclamacoesClientes",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RespostaCliente",
                table: "ReclamacoesClientes",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resultado",
                table: "ReclamacoesClientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TratamentoAplicado",
                table: "ReclamacoesClientes",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEncerramento",
                table: "ReclamacoesClientes",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioValidacao",
                table: "ReclamacoesClientes",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidadaEm",
                table: "ReclamacoesClientes",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataRespostaCliente",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "EncerradaEm",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "Investigacao",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "RespostaCliente",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "Resultado",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "TratamentoAplicado",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "UsuarioEncerramento",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "UsuarioValidacao",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "ValidadaEm",
                table: "ReclamacoesClientes");
        }
    }
}
