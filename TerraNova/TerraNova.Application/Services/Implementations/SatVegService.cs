using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;
using TerraNova.Integration.SatVeg;

namespace TerraNova.Application.Services.Implementations;

public sealed class SatvegService(
    ISatvegRepository satvegRepository,
    ITalhaoRepository talhaoRepository,
    SatVegClient satVegClient,
    IConfiguration configuration,
    ILogger<SatvegService> logger) : ISatvegService
{
    private string SatVegToken => configuration["SatVegApiToken"] ?? "Bearer e97dab05-eedc-39b9-a3fd-fa83cb5fef5e";

    public IReadOnlyList<SatvegResponse> GetAll()
    {
        var entities = satvegRepository.GetAll();
        return entities.Select(SatvegResponse.FromDomain).ToList();
    }

    public SatvegResponse? GetById(Guid id)
    {
        var entity = satvegRepository.GetById(id);
        return entity != null ? SatvegResponse.FromDomain(entity) : null;
    }

    public IReadOnlyList<SatvegResponse> GetByTalhaoId(Guid talhaoId)
    {
        var entities = satvegRepository.GetByTalhaoId(talhaoId);
        return entities.Select(SatvegResponse.FromDomain).ToList();
    }

    // Assinatura agora é idêntica à da interface ISatvegService
    public SatvegResponse Create(SatvegRequest request)
    {
        var talhao = talhaoRepository.GetById(request.TalhaoId) 
            ?? throw new InvalidOperationException("Talhão não encontrado.");

        // As coordenadas vêm da entidade Localizacao associada ao Talhão
        // Nota: Certifique-se de que o seu repositório está fazendo o .Include(t => t.Localizacao) no GetById
        if (talhao.Localizacao == null)
            throw new InvalidOperationException("Localização do talhão não foi carregada ou não existe.");

        var satveg = request.ToDomain();

        // Executa a chamada HTTP assíncrona de forma síncrona para respeitar o retorno da interface
        // Nota: Se a sua classe Localizacao usar nomes diferentes, ajuste "Latitude" e "Longitude" abaixo.
        FetchAndSetSatVegData(satveg, talhao.Localizacao.Latitude, talhao.Localizacao.Longitude, "{}")
            .GetAwaiter()
            .GetResult();

        satvegRepository.Add(satveg);
        return SatvegResponse.FromDomain(satveg);
    }

    // Retorna bool e passa o 'id' (Guid) para o repositório em vez da entidade inteira
    public bool Delete(Guid id)
    {
        return satvegRepository.Delete(id);
    }

    private async Task FetchAndSetSatVegData(Satveg entity, decimal latitude, decimal longitude, string fallbackJson)
    {
        try
        {
            var apiRequest = new SatVegDataRequest(
                TipoPerfil: "ndvi",
                Satelite: "comb",
                PreFiltro: 3,
                Filtro: "sav",
                ParametroFiltro: 4,
                Longitude: longitude,
                Latitude: latitude
            );

            var apiResponse = await satVegClient.GetSeriesAsync(SatVegToken, apiRequest);

            if (apiResponse != null)
            {
                var serieValores = new Dictionary<string, double>();

                for (int i = 0; i < apiResponse.ListaSerie.Count; i++)
                {
                    string dataString = apiResponse.ListaDatas[i];
                    
                    if (!string.IsNullOrEmpty(dataString) && string.Compare(dataString, "2020-01-01", StringComparison.Ordinal) >= 0)
                    {
                        serieValores[dataString] = apiResponse.ListaSerie[i];
                    }
                }

                var finalJson = new Dictionary<string, object> { { "NDVI", serieValores } };
                entity.SetDadosJson(JsonSerializer.Serialize(finalJson));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao integrar com a API da Embrapa SATveg");
            entity.SetDadosJson(fallbackJson);
        }
    }
}