using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddReclamacoesClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Produtos",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Numero",
                table: "Lotes",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Clientes",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Contato",
                table: "Clientes",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "ReclamacoesClientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    SequenciaAnual = table.Column<int>(type: "integer", nullable: false),
                    DataRecebimento = table.Column<DateOnly>(type: "date", nullable: false),
                    CanalRecebimento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    ContatoCliente = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    DataFabricacao = table.Column<DateOnly>(type: "date", nullable: true),
                    DataValidade = table.Column<DateOnly>(type: "date", nullable: true),
                    QuantidadeEnvolvida = table.Column<decimal>(type: "numeric", nullable: true),
                    LocalAquisicao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProdutoDisponivel = table.Column<bool>(type: "boolean", nullable: false),
                    QuantidadeDisponivel = table.Column<decimal>(type: "numeric", nullable: true),
                    VolumeDisponivel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Classificacao = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    UsuarioAbertura = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CriadaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReclamacoesClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReclamacoesClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReclamacoesClientes_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReclamacaoClienteLote",
                columns: table => new
                {
                    ReclamacaoClienteId = table.Column<int>(type: "integer", nullable: false),
                    LoteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReclamacaoClienteLote", x => new { x.ReclamacaoClienteId, x.LoteId });
                    table.ForeignKey(
                        name: "FK_ReclamacaoClienteLote_Lotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "Lotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReclamacaoClienteLote_ReclamacoesClientes_ReclamacaoCliente~",
                        column: x => x.ReclamacaoClienteId,
                        principalTable: "ReclamacoesClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReclamacaoClienteLote_LoteId",
                table: "ReclamacaoClienteLote",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_ReclamacoesClientes_Ano_SequenciaAnual",
                table: "ReclamacoesClientes",
                columns: new[] { "Ano", "SequenciaAnual" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReclamacoesClientes_ClienteId",
                table: "ReclamacoesClientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ReclamacoesClientes_Codigo",
                table: "ReclamacoesClientes",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReclamacoesClientes_ProdutoId",
                table: "ReclamacoesClientes",
                column: "ProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReclamacaoClienteLote");

            migrationBuilder.DropTable(
                name: "ReclamacoesClientes");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Produtos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Numero",
                table: "Lotes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Clientes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Contato",
                table: "Clientes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);
        }
    }
}
