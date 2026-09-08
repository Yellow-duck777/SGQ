using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Models;

namespace SGQ.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();

    public DbSet<Produto> Produtos => Set<Produto>();

    public DbSet<Lote> Lotes => Set<Lote>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Cliente>(entity =>
        {
            entity.Property(cliente => cliente.Nome).IsRequired();
            entity.Property(cliente => cliente.Contato).IsRequired();
        });

        builder.Entity<Produto>(entity =>
        {
            entity.Property(produto => produto.Nome).IsRequired();
        });

        builder.Entity<Lote>(entity =>
        {
            entity.Property(lote => lote.Numero).IsRequired();
            entity.HasOne(lote => lote.Produto)
                .WithMany(produto => produto.Lotes)
                .HasForeignKey(lote => lote.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });
    }
}
