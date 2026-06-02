using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Implementations;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Infrastructure.Persistence;
using TerraNova.Infrastructure.Persistence.Repositories;
using TerraNova.Integration.Nasa;
using TerraNova.Integration.SatVeg;

namespace TerraNova.API.Extensions;

/// <summary>
/// Extensões para registrar persistência, repositórios e serviços do TerraNova na injeção de dependências.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registra o <see cref="TerraNovaContext"/> com Oracle.</summary>
    /// <exception cref="InvalidOperationException">Quando a connection string não for encontrada.</exception>
    public static IServiceCollection AddTerraNovaDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "TerraNovaOracle")
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' não encontrada.");
 
        services.AddDbContext<TerraNovaContext>(options =>
            options.UseOracle(connectionString, b =>
                b.UseOracleSQLCompatibility(
                    Microsoft.EntityFrameworkCore.OracleSQLCompatibility.DatabaseVersion19)));
 
        return services;
    }
 
    /// <summary>Registra todas as implementações de repositório como <c>Scoped</c>.</summary>
    public static IServiceCollection AddTerraNovaRepositories(this IServiceCollection services)
    {
        // Repositórios especializados
        services.AddScoped<IProdutorRepository,        ProdutorRepository>();
        services.AddScoped<ITalhaoRepository,          TalhaoRepository>();
        services.AddScoped<ISatvegRepository,          SatvegRepository>();
        services.AddScoped<INasaPowerRepository,       NasaPowerRepository>();
        services.AddScoped<IAlertaAgricolaRepository,  AlertaAgricolaRepository>();
 
        // Repositório genérico para entidades sem queries especiais
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
 
        return services;
    }
 
    /// <summary>Registra os serviços de aplicação como <c>Scoped</c>.</summary>
    public static IServiceCollection AddTerraNovaApplicationServices(this IServiceCollection services)
    {
        // Lookup / infraestrutura
        services.AddScoped<ILocalizacaoService,    LocalizacaoService>();
        services.AddScoped<ITipoPlantacaoService,  TipoPlantacaoService>();
        services.AddScoped<ITelefoneService,       TelefoneService>();
 
        // Core
        services.AddScoped<IProdutorService,       ProdutorService>();
        services.AddScoped<IPropriedadeService,    PropriedadeService>();
        services.AddScoped<ITalhaoService,         TalhaoService>();
 
        // Dados externos
        services.AddScoped<ISatvegService,         SatvegService>();
        services.AddScoped<INasaPowerService,      NasaPowerService>();
 
        // Alertas
        services.AddScoped<IAlertaAgricolaService, AlertaAgricolaService>();
 
        services.AddHttpClient<NasaPowerClient>(client => 
        {
            client.BaseAddress = new Uri("https://power.larc.nasa.gov");
        });

        services.AddHttpClient<SatVegClient>(client => 
        {
            client.BaseAddress = new Uri("https://api.cnptia.embrapa.br");
        });
        
        return services;
    }
}