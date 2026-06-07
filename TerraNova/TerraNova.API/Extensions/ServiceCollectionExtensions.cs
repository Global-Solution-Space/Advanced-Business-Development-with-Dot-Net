using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
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
            options.UseOracle(connectionString, b => b.UseNetTopologySuite()));
 
        return services;
    }
 
    /// <summary>Registra todas as implementações de repositório como <c>Scoped</c>.</summary>
    public static IServiceCollection AddTerraNovaRepositories(this IServiceCollection services)
    {
        // Repositórios especializados
        services.AddScoped<IProdutorRepository,        ProdutorRepository>();
        services.AddScoped<ITalhaoRepository,          TalhaoRepository>();
        services.AddScoped<IAlertaAgricolaRepository,  AlertaAgricolaRepository>();
        services.AddScoped<IReqApiRepository,          ReqApiRepository>();
        services.AddScoped<IDadoTemporalRepository,    DadoTemporalRepository>();
        
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
        services.AddScoped<ITipoApiService,        TipoApiService>(); 
        
        // Core
        services.AddScoped<IProdutorService,       ProdutorService>();
        services.AddScoped<IPropriedadeService,    PropriedadeService>();
        services.AddScoped<ITalhaoService,         TalhaoService>();
        
        // APIs externas + dados temporais
        services.AddScoped<IReqApiService,       ReqApiService>();
        services.AddScoped<IDadoTemporalService, DadoTemporalService>();
        
        // Alertas
        services.AddScoped<IAlertaAgricolaService, AlertaAgricolaService>();
 
        // Clientes HTTP
        services.AddHttpClient<NasaPowerClient>(client =>
            client.BaseAddress = new Uri("https://power.larc.nasa.gov/api/"));
 
        services.AddHttpClient<SatVegClient>(client =>
            client.BaseAddress = new Uri("https://api.cnptia.embrapa.br/satveg/v2"));
        
        return services;
    }
}