using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Models;
using Microsoft.AspNetCore.Http;

namespace SGQ.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();

    public DbSet<Produto> Produtos => Set<Produto>();

    public DbSet<Lote> Lotes => Set<Lote>();

    public DbSet<ReclamacaoCliente> ReclamacoesClientes => Set<ReclamacaoCliente>();

    public DbSet<NaoConformidade> NaoConformidades => Set<NaoConformidade>();

    public DbSet<Recall> Recalls => Set<Recall>();
    public DbSet<AcaoNaoConformidade> AcoesNaoConformidade => Set<AcaoNaoConformidade>();
    public DbSet<RetornoRecall> RetornosRecall => Set<RetornoRecall>();
    public DbSet<HistoricoAuditoria> HistoricosAuditoria => Set<HistoricoAuditoria>();
    public DbSet<Anexo> Anexos => Set<Anexo>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        var pendentes = ChangeTracker.Entries().Where(item => item.Entity is not HistoricoAuditoria && item.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(item => new { Entry = item, Estado = item.State, Alteracoes = string.Join("; ", item.Properties.Where(p => item.State == EntityState.Added || item.State == EntityState.Deleted || p.IsModified).Select(p => $"{p.Metadata.Name}: {p.OriginalValue} → {p.CurrentValue}")) }).ToList();
        var result = await base.SaveChangesAsync(cancellationToken);
        if (pendentes.Count == 0) return result;
        var usuario = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Sistema";
        foreach (var item in pendentes)
        {
            var chave = string.Join(",", item.Entry.Properties.Where(p => p.Metadata.IsPrimaryKey()).Select(p => p.CurrentValue?.ToString()));
            HistoricosAuditoria.Add(new HistoricoAuditoria { Entidade = item.Entry.Metadata.ClrType.Name, ChaveRegistro = chave ?? string.Empty, Acao = item.Estado.ToString(), Usuario = usuario, OcorridaEm = DateTimeOffset.UtcNow, Alteracoes = item.Alteracoes });
        }
        await base.SaveChangesAsync(cancellationToken);
        return result;
    }

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
            entity.Property(reclamacao => reclamacao.Resultado).HasConversion<string>();
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

        builder.Entity<Recall>(entity =>
        {
            entity.HasIndex(recall => recall.Codigo).IsUnique();
            entity.HasIndex(recall => new { recall.Ano, recall.SequenciaAnual }).IsUnique();
            entity.Property(recall => recall.Origem).HasConversion<string>();
            entity.Property(recall => recall.Decisao).HasConversion<string>();
            entity.Property(recall => recall.Status).HasConversion<string>();
            entity.HasOne(recall => recall.NaoConformidade).WithMany().HasForeignKey(recall => recall.NaoConformidadeId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(recall => recall.ReclamacaoCliente).WithMany().HasForeignKey(recall => recall.ReclamacaoClienteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(recall => recall.Produto).WithMany().HasForeignKey(recall => recall.ProdutoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(recall => recall.Lote).WithMany().HasForeignKey(recall => recall.LoteId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AcaoNaoConformidade>(entity =>
        {
            entity.HasOne(item => item.NaoConformidade).WithMany(nc => nc.Acoes).HasForeignKey(item => item.NaoConformidadeId).OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<RetornoRecall>(entity =>
        {
            entity.HasOne(item => item.Recall).WithMany(recall => recall.Retornos).HasForeignKey(item => item.RecallId).OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<Anexo>(entity =>
        {
            entity.HasOne<ReclamacaoCliente>().WithMany().HasForeignKey(item => item.ReclamacaoClienteId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<NaoConformidade>().WithMany().HasForeignKey(item => item.NaoConformidadeId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Recall>().WithMany().HasForeignKey(item => item.RecallId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
