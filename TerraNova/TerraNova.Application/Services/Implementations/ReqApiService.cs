using System.Globalization;
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
    IReqApiRepository           reqApiRepository,
    IDadoTemporalRepository      dadoTemporalRepository,
    ITalhaoRepository            talhaoRepository,
    IRepository<TipoApi>         tipoApiRepository,
    IAlertaAgricolaRepository    alertaRepository,
    SatVegClient                 satVegClient,
    NasaPowerClient              nasaPowerClient,
    IConfiguration               configuration,
    ILogger<ReqApiService>       logger) : IReqApiService
{
    private string SatVegToken =>
        configuration["SatVegApiToken"] ?? "Bearer e97dab05-eedc-39b9-a3fd-fa83cb5fef5e";
 
    public IReadOnlyList<ReqApiResponse> GetAll()
    {
        var reqApis = reqApiRepository.GetAll();
        var counts = reqApiRepository.CountDadosByReqApiIds(reqApis.Select(r => r.Id));
        return reqApis
            .Select(r => ReqApiResponse.FromDomain(r, counts.GetValueOrDefault(r.Id, 0)))
            .ToList();
    }
 
    public ReqApiResponse? GetById(Guid id)
    {
        var r = reqApiRepository.GetById(id);
        return r is null ? null : ReqApiResponse.FromDomain(r, reqApiRepository.CountDadosByReqApiId(r.Id));
    }
 
    public IReadOnlyList<ReqApiResponse> GetByTipoParam(TipoParamReqApi tipoParam)
    {
        var reqApis = reqApiRepository.GetByTipoParam(tipoParam);
        var counts = reqApiRepository.CountDadosByReqApiIds(reqApis.Select(r => r.Id));
        return reqApis
            .Select(r => ReqApiResponse.FromDomain(r, counts.GetValueOrDefault(r.Id, 0)))
            .ToList();
    }
 
    public IReadOnlyList<ReqApiResponse> GetByTipoApiId(Guid tipoApiId)
    {
        var reqApis = reqApiRepository.GetByTipoApiId(tipoApiId);
        var counts = reqApiRepository.CountDadosByReqApiIds(reqApis.Select(r => r.Id));
        return reqApis
            .Select(r => ReqApiResponse.FromDomain(r, counts.GetValueOrDefault(r.Id, 0)))
            .ToList();
    }
 
    public IReadOnlyList<ReqApiResponse> GetByTalhaoId(Guid talhaoId)
    {
        var reqApis = reqApiRepository.GetByTalhaoId(talhaoId);
        var counts = reqApiRepository.CountDadosByReqApiIds(reqApis.Select(r => r.Id));
        return reqApis
            .Select(r => ReqApiResponse.FromDomain(r, counts.GetValueOrDefault(r.Id, 0)))
            .ToList();
    }
 
    public async Task<ReqApiResponse> CreateAsync(ReqApiRequest request)
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
            TipoParamReqApi.Ndvi        => await BuscarDadosSatVeg(reqApi.Id, talhao),
            TipoParamReqApi.Prectotcorr => await BuscarDadosNasaPower(reqApi.Id, talhao),
            _                           => []
        };
 
        if (dados.Count > 0)
        {
            dadoTemporalRepository.AddRange(dados);
            var tipoApiNome = request.TipoParam == TipoParamReqApi.Ndvi ? "SATVEG" : "NASA POWER";
            AnalisarEGerarAlertas(talhao.Id, dados, tipoApiNome);
        }
 
        return ReqApiResponse.FromDomain(reqApi, dados.Count);
    }
 
    public bool Delete(Guid id) => reqApiRepository.Delete(id);
    
    // Integrações externas
 
    private void AnalisarEGerarAlertas(Guid talhaoId, List<DadoTemporal> dadosRecentes, string tipoApiNome)
    {
        if (tipoApiNome == "NASA POWER")
        {
            var dataLimite = DateTime.UtcNow.AddDays(-15);
            var dados = dadosRecentes
                .Where(d => d.DataLeitura >= dataLimite)
                .OrderByDescending(d => d.DataLeitura)
                .ToList();

            if (dados.Count == 0) return;

            var chuvaAcumulada15dias = dados.Sum(d => d.Valor);
            var chuvaAcumulada3dias = dados.Take(3).Sum(d => d.Valor);

            if (chuvaAcumulada3dias > 80.0m)
                CriarAlertaSeNovo(talhaoId, NivelAlerta.Alto, "Risco de Alagamento (NASA)",
                    "Chuva extrema detectada nos últimos 3 dias. Risco de erosão e asfixia radicular.");
            else if (chuvaAcumulada15dias < 10.0m)
                CriarAlertaSeNovo(talhaoId, NivelAlerta.Critico, "Seca Severa (NASA)",
                    "Pouquíssima chuva acumulada nos últimos 15 dias.");
            else if (chuvaAcumulada15dias < 25.0m)
                CriarAlertaSeNovo(talhaoId, NivelAlerta.Medio, "Estresse Hídrico (NASA)",
                    "Baixa precipitação acumulada nos últimos 15 dias.");
        }
        else if (tipoApiNome == "SATVEG")
        {
            var dataLimite = DateTime.UtcNow.AddDays(-365);
            var dados = dadosRecentes
                .Where(d => d.DataLeitura >= dataLimite)
                .OrderByDescending(d => d.DataLeitura)
                .ToList();

            if (dados.Count == 0) return;

            var ultimoNdvi = dados.First().Valor;

            if (ultimoNdvi < 0.2m)
                CriarAlertaSeNovo(talhaoId, NivelAlerta.Critico, "Anomalia Vegetativa Severa (SATVEG)",
                    "O NDVI atual caiu drasticamente. Possível falha na cultura ou solo exposto.");
            else if (ultimoNdvi < 0.4m)
                CriarAlertaSeNovo(talhaoId, NivelAlerta.Medio, "Baixo Vigor Vegetativo (SATVEG)",
                    "O NDVI atual está baixo. Monitore para pragas, doenças ou estresse nutricional.");
        }
    }

    private void CriarAlertaSeNovo(Guid talhaoId, NivelAlerta nivel, string titulo, string descricao)
    {
        if (alertaRepository.ExisteAlertaAtivo(talhaoId, titulo)) return;

        var alerta = new AlertaAgricola(titulo, descricao, nivel, talhaoId);
        alertaRepository.Add(alerta);
    }

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
            var lat = talhao.Localizacao!.Coordenadas.Y.ToString("0.0000", CultureInfo.InvariantCulture);
            var lon = talhao.Localizacao.Coordenadas.X.ToString("0.0000", CultureInfo.InvariantCulture);

            var resposta = await nasaPowerClient.GetDailyDataAsync(
                dataInicio, dataFim, lat, lon);
 
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