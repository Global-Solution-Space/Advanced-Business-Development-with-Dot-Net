using System.Text.Json;
using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;
using TerraNova.Integration.Nasa;

namespace TerraNova.Application.Services.Implementations;

public sealed class NasaPowerService(
    INasaPowerRepository nasaPowerRepository,
    ITalhaoRepository talhaoRepository,
    NasaPowerClient nasaPowerClient) : INasaPowerService
{
    public IReadOnlyList<NasaPowerResponse> GetAll()
    {
        var entities = nasaPowerRepository.GetAll();
        return entities.Select(NasaPowerResponse.FromDomain).ToList();
    }

    public NasaPowerResponse? GetById(Guid id)
    {
        var entity = nasaPowerRepository.GetById(id);
        return entity != null ? NasaPowerResponse.FromDomain(entity) : null;
    }

    public IReadOnlyList<NasaPowerResponse> GetByTalhaoId(Guid talhaoId)
    {
        var entities = nasaPowerRepository.GetByTalhaoId(talhaoId);
        return entities.Select(NasaPowerResponse.FromDomain).ToList();
    }

    // Assinatura idêntica à interface INasaPowerService (Retorno síncrono)
    public NasaPowerResponse Create(NasaPowerRequest request)
    {
        var talhao = talhaoRepository.GetById(request.TalhaoId) 
            ?? throw new InvalidOperationException("Talhão não encontrado.");

        // Coordenadas obtidas através da Localização associada ao Talhão
        if (talhao.Localizacao == null)
            throw new InvalidOperationException("Localização do talhão não foi carregada ou não existe.");

        var nasaPower = request.ToDomain();
        nasaPower.SetDataAnalise();

        // Executa a chamada HTTP assíncrona de forma síncrona para obedecer à interface
        FetchAndSetNasaPowerData(nasaPower, (decimal)talhao.Localizacao.Coordenadas.Y, (decimal)talhao.Localizacao.Coordenadas.X, "{}")
            .GetAwaiter()
            .GetResult();

        nasaPowerRepository.Add(nasaPower);
        return NasaPowerResponse.FromDomain(nasaPower);
    }

    // Retorna bool e encaminha o Guid diretamente ao repositório
    public bool Delete(Guid id)
    {
        return nasaPowerRepository.Delete(id);
    }

    private async Task FetchAndSetNasaPowerData(NasaPower entity, decimal latitude, decimal longitude, string fallbackJson)
    {
        try
        {
            var apiResponse = await nasaPowerClient.GetDailyDataAsync(entity.DataInicio, entity.DataFim, latitude, longitude);

            if (apiResponse?.Properties?.Parameter != null && apiResponse.Properties.Parameter.TryGetValue("PRECTOTCORR", out var dadosBrutos))
            {
                var datasNormalizadas = new Dictionary<string, double>();

                foreach (var (dataAntiga, valor) in dadosBrutos)
                {
                    if (valor.HasValue && valor.Value > -900.0)
                    {
                        string dataFormatada = (dataAntiga != null && dataAntiga.Length == 8)
                            ? $"{dataAntiga[..4]}-{dataAntiga.Substring(4, 2)}-{dataAntiga.Substring(6, 2)}"
                            : dataAntiga ?? string.Empty;

                        datasNormalizadas[dataFormatada] = valor.Value;
                    }
                }

                var jsonFinal = new Dictionary<string, object> { { "PRECTOTCORR", datasNormalizadas } };
                entity.SetDadosJson(JsonSerializer.Serialize(jsonFinal));
            }
        }
        catch (Exception)
        {
            // Em caso de falha, resguarda a aplicação gravando o fallback estrutural no banco
            entity.SetDadosJson(fallbackJson);
        }
    }
}