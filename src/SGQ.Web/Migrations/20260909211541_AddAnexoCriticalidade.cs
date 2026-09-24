using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAnexoCriticalidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AnuladoEm",
                table: "Anexos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Anexos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Critico",
                table: "Anexos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JustificativaAnulacao",
                table: "Anexos",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioAnulacao",
                table: "Anexos",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnuladoEm",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "Critico",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "JustificativaAnulacao",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "UsuarioAnulacao",
                table: "Anexos");
        }
    }
}
