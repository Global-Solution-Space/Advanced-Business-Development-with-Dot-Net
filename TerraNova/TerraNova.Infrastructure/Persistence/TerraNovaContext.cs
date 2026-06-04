using Microsoft.EntityFrameworkCore;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence;

public class TerraNovaContext(DbContextOptions<TerraNovaContext> options) : DbContext(options)
{
    // Lookup
    public DbSet<Localizacao>   Localizacoes   { get; set; }
    public DbSet<TipoPlantacao> TiposPlantacao { get; set; }
    public DbSet<Telefone>      Telefones      { get; set; }
    public DbSet<TipoApi>       TiposApi       { get; set; }

    // Core
    public DbSet<Produtor>    Produtores   { get; set; }
    public DbSet<Propriedade> Propriedades { get; set; }
    public DbSet<Talhao>      Talhoes      { get; set; }

    // Dados externos (normalizado)
    public DbSet<ReqApi>       ReqApis        { get; set; }
    public DbSet<DadoTemporal> DadosTemporais { get; set; }

    // Alertas
    public DbSet<AlertaAgricola> Alertas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TerraNovaContext).Assembly);

        modelBuilder.Entity<Propriedade>(entity =>
        {
            entity.Property(e => e.TamanhoTotal)
                .HasPrecision(18, 4);
        });

        modelBuilder.Entity<Talhao>(entity =>
        {
            entity.Property(e => e.VolumArea)
                .HasPrecision(18, 4);
        });
    }
}