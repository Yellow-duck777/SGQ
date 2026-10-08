using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddParecerUsuariosRtGq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioParecerGq",
                table: "Recalls",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioParecerRt",
                table: "Recalls",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioParecerGq",
                table: "NaoConformidades",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioParecerRt",
                table: "NaoConformidades",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioParecerGq",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "UsuarioParecerRt",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "UsuarioParecerGq",
                table: "NaoConformidades");

            migrationBuilder.DropColumn(
                name: "UsuarioParecerRt",
                table: "NaoConformidades");
        }
    }
}
