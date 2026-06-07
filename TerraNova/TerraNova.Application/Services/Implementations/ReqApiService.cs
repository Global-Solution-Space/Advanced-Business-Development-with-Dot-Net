using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;
using TerraNova.Integration.Nasa;
using TerraNova.Integration.SatVeg;

namespace TerraNova.Application.Services.Implementations;

public sealed class ReqApiService(
    IReqApiRepository      reqApiRepository,
    IDadoTemporalRepository dadoTemporalRepository,
    ITalhaoRepository       talhaoRepository,
    IRepository<TipoApi>    tipoApiRepository,
    SatVegClient            satVegClient,
    NasaPowerClient         nasaPowerClient,
    IConfiguration          configuration,
    ILogger<ReqApiService>  logger) : IReqApiService
{
    private string SatVegToken =>
        configuration["SatVegApiToken"] ?? "Bearer e97dab05-eedc-39b9-a3fd-fa83cb5fef5e";
 
    public IReadOnlyList<ReqApiResponse> GetAll() =>
        reqApiRepository.GetAll()
            .Select(r => ReqApiResponse.FromDomain(r, r.DadosTemporais.Count))
            .ToList();
 
    public ReqApiResponse? GetById(Guid id)
    {
        var r = reqApiRepository.GetById(id);
        return r is null ? null : ReqApiResponse.FromDomain(r, r.DadosTemporais.Count);
    }
 
    public IReadOnlyList<ReqApiResponse> GetByTipoParam(TipoParamReqApi tipoParam) =>
        reqApiRepository.GetByTipoParam(tipoParam)
            .Select(r => ReqApiResponse.FromDomain(r, r.DadosTemporais.Count))
            .ToList();
 
    public IReadOnlyList<ReqApiResponse> GetByTipoApiId(Guid tipoApiId) =>
        reqApiRepository.GetByTipoApiId(tipoApiId)
            .Select(r => ReqApiResponse.FromDomain(r, r.DadosTemporais.Count))
            .ToList();
 
    public ReqApiResponse Create(ReqApiRequest request)
    {
        if (!tipoApiRepository.ExistsById(request.TipoApiId))
            throw new InvalidOperationException("Tipo de API não encontrado.");
 
        var talhao = talhaoRepository.GetByIdWithLocalizacao(request.TalhaoId)
            ?? throw new InvalidOperationException("Talhão não encontrado.");
 
        if (talhao.Localizacao is null)
            throw new InvalidOperationException("Localização do talhão não foi encontrada.");
 
        var reqApi = new ReqApi(request.TipoParam, request.TipoApiId);
        reqApiRepository.Add(reqApi);
 
        var dados = request.TipoParam switch
        {
            TipoParamReqApi.Nvdi        => BuscarDadosSatVeg(reqApi.Id, talhao).GetAwaiter().GetResult(),
            TipoParamReqApi.Prectotcorr => BuscarDadosNasaPower(reqApi.Id, talhao).GetAwaiter().GetResult(),
            _                           => []
        };
 
        if (dados.Count > 0)
            dadoTemporalRepository.AddRange(dados);
 
        return ReqApiResponse.FromDomain(reqApi, dados.Count);
    }
 
    public bool Delete(Guid id) => reqApiRepository.Delete(id);
    
    // Integrações externas
 
    private async Task<List<DadoTemporal>> BuscarDadosSatVeg(Guid reqApiId, Talhao talhao)
    {
        var resultado = new List<DadoTemporal>();
 
        try
        {
            var apiRequest = new SatVegDataRequest(
                TipoPerfil:      "ndvi",
                Satelite:        "comb",
                PreFiltro:       3,
                Filtro:          "sav",
                ParametroFiltro: 4,
                Longitude:       (decimal)talhao.Localizacao!.Coordenadas.X,
                Latitude:        (decimal)talhao.Localizacao.Coordenadas.Y);
 
            var resposta = await satVegClient.GetSeriesAsync(SatVegToken, apiRequest);
 
            if (resposta is null) return resultado;
 
            for (int i = 0; i < resposta.ListaSerie.Count; i++)
            {
                var dataStr = resposta.ListaDatas[i];
 
                if (string.IsNullOrEmpty(dataStr) ||
                    string.Compare(dataStr, "2020-01-01", StringComparison.Ordinal) < 0)
                    continue;
 
                if (!DateTime.TryParse(dataStr, out var dataLeitura)) continue;
 
                resultado.Add(new DadoTemporal(
                    dataLeitura,
                    (decimal)resposta.ListaSerie[i],
                    talhao.Id,
                    reqApiId));
            }
        }
        catch (InvalidOperationException)
        {
            throw; // Permite que a validação de regra de negócio/limites do oceano suba para o Controller
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao integrar com a API SatVeg/Embrapa para o talhão {TalhaoId}", talhao.Id);
        }
 
        return resultado;
    }
 
    private async Task<List<DadoTemporal>> BuscarDadosNasaPower(
        Guid reqApiId, Talhao talhao)
    {
        var resultado = new List<DadoTemporal>();
 
        try
        {
            string dataInicio = "20200101";
            string dataFim = DateTime.UtcNow.ToString("yyyyMMdd");

            var resposta = await nasaPowerClient.GetDailyDataAsync(
                dataInicio, dataFim,
                (decimal)talhao.Localizacao!.Coordenadas.Y,
                (decimal)talhao.Localizacao.Coordenadas.X);
 
            if (resposta?.Properties?.Parameter is null) return resultado;
 
            if (!resposta.Properties.Parameter.TryGetValue("PRECTOTCORR", out var serie))
                return resultado;
 
            foreach (var (dataStr, valor) in serie)
            {
                if (!valor.HasValue || valor.Value <= -900.0) continue;
 
                // Normaliza YYYYMMDD → YYYY-MM-DD
                var dataFormatada = dataStr?.Length == 8
                    ? $"{dataStr[..4]}-{dataStr.Substring(4, 2)}-{dataStr.Substring(6, 2)}"
                    : dataStr ?? string.Empty;
 
                if (!DateTime.TryParse(dataFormatada, out var dataLeitura)) continue;
 
                resultado.Add(new DadoTemporal(
                    dataLeitura,
                    (decimal)valor.Value,
                    talhao.Id,
                    reqApiId));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao integrar com a NASA POWER API para o talhão {TalhaoId}", talhao.Id);
        }
 
        return resultado;
    }
}
 