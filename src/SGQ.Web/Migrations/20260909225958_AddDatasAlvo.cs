using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddDatasAlvo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DataAlvo",
                table: "ReclamacoesClientes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataAlvo",
                table: "Recalls",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataAlvo",
                table: "NaoConformidades",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAlvo",
                table: "ReclamacoesClientes");

            migrationBuilder.DropColumn(
                name: "DataAlvo",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "DataAlvo",
                table: "NaoConformidades");
        }
    }
}
