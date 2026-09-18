using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddLaboratorioExterno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DataEnvioAmostraLaboratorio",
                table: "ReclamacoesClientes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataRecebimentoResultadoLaboratorio",
                table: "ReclamacoesClientes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificacaoLaudoLaboratorio",
                table: "ReclamacoesClientes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LaboratorioExterno",
                table: "ReclamacoesClientes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LaudoLaboratorioAnexoId",
                table: "ReclamacoesClientes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultadoLaboratorio",
                table: "ReclamacoesClientes",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataEnvioAmostraLaboratorio",
                table: "NaoConformidades",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataRecebimentoResultadoLaboratorio",
                table: "NaoConformidades",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificacaoLaudoLaboratorio",
                table: "NaoConformidades",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LaboratorioExterno",
                table: "NaoConformidades",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LaudoLaboratorioAnexoId",
                table: "NaoConformidades",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultadoLaboratorio",
                table: "NaoConformidades",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataEnvioAmostraLaboratorio",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "DataRecebimentoResultadoLaboratorio",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "IdentificacaoLaudoLaboratorio",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "LaboratorioExterno",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "LaudoLaboratorioAnexoId",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "ResultadoLaboratorio",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "DataEnvioAmostraLaboratorio",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "DataRecebimentoResultadoLaboratorio",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "IdentificacaoLaudoLaboratorio",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "LaboratorioExterno",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "LaudoLaboratorioAnexoId",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "ResultadoLaboratorio",
                table: "NaoConformidades");
        }
    }
}
