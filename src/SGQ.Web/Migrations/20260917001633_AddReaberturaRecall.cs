using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddReaberturaRecall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JustificativaReabertura",
                table: "Recalls",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoReabertura",
                table: "Recalls",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReabertaEm",
                table: "Recalls",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusAnteriorReabertura",
                table: "Recalls",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioReabertura",
                table: "Recalls",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JustificativaReabertura",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "MotivoReabertura",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "ReabertaEm",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "StatusAnteriorReabertura",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "UsuarioReabertura",
                table: "Recalls");
        }
    }
}
