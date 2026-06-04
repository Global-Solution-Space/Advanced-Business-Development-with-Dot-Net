using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.DTOs;

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
