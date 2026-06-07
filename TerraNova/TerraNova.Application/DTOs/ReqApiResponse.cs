using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.DTOs;

/// <summary>Resultado de uma requisição feita a uma API externa.</summary>
/// <param name="Id">Identificador da requisição.</param>
/// <param name="TipoParam">Parâmetro consultado.</param>
/// <param name="DataAnalise">Data e hora da análise.</param>
/// <param name="TipoApiId">Identificador do tipo de API utilizado.</param>
/// <param name="TotalDadosSalvos">Quantidade de dados temporais persistidos pela requisição.</param>
public record ReqApiResponse(
    Guid           Id,
    TipoParamReqApi TipoParam,
    DateTime       DataAnalise,
    Guid           TipoApiId,
    int            TotalDadosSalvos)
{
    public static ReqApiResponse FromDomain(ReqApi r, int totalDados) =>
        new(r.Id, r.TipoParam, r.DataAnalise, r.TipoApiId, totalDados);
}
