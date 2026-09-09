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

    public DbSet<ReclamacaoCliente> ReclamacoesClientes => Set<ReclamacaoCliente>();

    public DbSet<NaoConformidade> NaoConformidades => Set<NaoConformidade>();

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

        builder.Entity<ReclamacaoCliente>(entity =>
        {
            entity.HasIndex(reclamacao => reclamacao.Codigo).IsUnique();
            entity.HasIndex(reclamacao => new { reclamacao.Ano, reclamacao.SequenciaAnual }).IsUnique();
            entity.Property(reclamacao => reclamacao.Status).HasConversion<string>();
            entity.Property(reclamacao => reclamacao.Classificacao).HasConversion<string>();
            entity.HasOne(reclamacao => reclamacao.Cliente).WithMany().HasForeignKey(reclamacao => reclamacao.ClienteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(reclamacao => reclamacao.Produto).WithMany().HasForeignKey(reclamacao => reclamacao.ProdutoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ReclamacaoClienteLote>(entity =>
        {
            entity.HasKey(item => new { item.ReclamacaoClienteId, item.LoteId });
            entity.HasOne(item => item.ReclamacaoCliente).WithMany(reclamacao => reclamacao.Lotes).HasForeignKey(item => item.ReclamacaoClienteId);
            entity.HasOne(item => item.Lote).WithMany().HasForeignKey(item => item.LoteId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<NaoConformidade>(entity =>
        {
            entity.HasIndex(nc => nc.Codigo).IsUnique();
            entity.HasIndex(nc => new { nc.Ano, nc.SequenciaAnual }).IsUnique();
            entity.HasIndex(nc => nc.ReclamacaoClienteId).IsUnique();
            entity.Property(nc => nc.Origem).HasConversion<string>();
            entity.Property(nc => nc.Status).HasConversion<string>();
            entity.Property(nc => nc.Classificacao).HasConversion<string>();
            entity.HasOne(nc => nc.ReclamacaoCliente).WithOne(rc => rc.NaoConformidade).HasForeignKey<NaoConformidade>(nc => nc.ReclamacaoClienteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(nc => nc.Produto).WithMany().HasForeignKey(nc => nc.ProdutoId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
