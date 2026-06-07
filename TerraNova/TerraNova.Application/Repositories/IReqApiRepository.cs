using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.Repositories;

public interface IReqApiRepository : IRepository<ReqApi>
{
    IReadOnlyList<ReqApi> GetByTipoParam(TipoParamReqApi tipoParam);
    IReadOnlyList<ReqApi> GetByTipoApiId(Guid tipoApiId);
    IReadOnlyList<ReqApi> GetByTalhaoId(Guid talhaoId);
}