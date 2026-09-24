using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class SyncCurrentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DecididaPeloCqEm",
                table: "Recalls",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DecisaoCq",
                table: "Recalls",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JustificativaDecisaoCq",
                table: "Recalls",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReprovadaGq",
                table: "Recalls",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReprovadaRt",
                table: "Recalls",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioDecisaoCq",
                table: "Recalls",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DecididaPeloCqEm",
                table: "NaoConformidades",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DecisaoCq",
                table: "NaoConformidades",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JustificativaDecisaoCq",
                table: "NaoConformidades",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReprovadaGq",
                table: "NaoConformidades",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReprovadaRt",
                table: "NaoConformidades",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioDecisaoCq",
                table: "NaoConformidades",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecididaPeloCqEm",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "DecisaoCq",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "JustificativaDecisaoCq",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "ReprovadaGq",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "ReprovadaRt",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "UsuarioDecisaoCq",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "DecididaPeloCqEm",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "DecisaoCq",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "JustificativaDecisaoCq",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "ReprovadaGq",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "ReprovadaRt",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "UsuarioDecisaoCq",
                table: "NaoConformidades");
        }
    }
}
