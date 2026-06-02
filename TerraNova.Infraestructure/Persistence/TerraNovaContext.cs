using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Common;
using TerraNova.Domain.Entities;

namespace TerraNova.Infraestructure.Persistence;

public class TerraNovaContext(DbContextOptions<TerraNovaContext> options) : DbContext(options)
{
    // Lookup
    public DbSet<Localizacao>    Localizacoes    { get; set; }
    public DbSet<TipoPlantacao>  TiposPlantacao  { get; set; }
    public DbSet<Telefone>       Telefones       { get; set; }
 
    // Core
    public DbSet<Produtor>       Produtores      { get; set; }
    public DbSet<Propriedade>    Propriedades    { get; set; }
    public DbSet<Talhao>         Talhoes         { get; set; }
 
    // Dados externos
    public DbSet<Satveg>         Satvegs         { get; set; }
    public DbSet<NasaPower>      NasPowers       { get; set; }
 
    // Alertas
    public DbSet<AlertaAgricola> Alertas         { get; set; }
 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TerraNovaContext).Assembly);
    }
}