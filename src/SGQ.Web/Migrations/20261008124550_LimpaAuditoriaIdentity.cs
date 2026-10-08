using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class LimpaAuditoriaIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // A auditoria passou a ignorar tokens e logins do Identity e a mascarar credenciais.
            // Remove o que já havia sido gravado antes da correção (hash de senha, carimbos de segurança, tokens).
            migrationBuilder.Sql("""
                DELETE FROM "HistoricosAuditoria" WHERE "Entidade" IN ('IdentityUserToken`1', 'IdentityUserLogin`1');
                """);
            migrationBuilder.Sql("""
                UPDATE "HistoricosAuditoria"
                SET "Alteracoes" = regexp_replace("Alteracoes", '(PasswordHash|SecurityStamp|ConcurrencyStamp): [^;]*', '\1: [removido]', 'g')
                WHERE "Alteracoes" ~ '(PasswordHash|SecurityStamp|ConcurrencyStamp): ';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Irreversível por desenho: os dados sensíveis removidos não devem ser restaurados.
        }
    }
}
