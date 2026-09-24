using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoricoAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricosAuditoria",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Entidade = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ChaveRegistro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Acao = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Usuario = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OcorridaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Alteracoes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricosAuditoria", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricosAuditoria");
        }
    }
}
