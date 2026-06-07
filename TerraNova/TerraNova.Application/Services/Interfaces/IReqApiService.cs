using TerraNova.Application.DTOs;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.Services.Interfaces;

public interface IReqApiService
{
    IReadOnlyList<ReqApiResponse> GetAll();
    ReqApiResponse? GetById(Guid id);
    IReadOnlyList<ReqApiResponse> GetByTipoParam(TipoParamReqApi tipoParam);
    IReadOnlyList<ReqApiResponse> GetByTipoApiId(Guid tipoApiId);
    IReadOnlyList<ReqApiResponse> GetByTalhaoId(Guid talhaoId);
 
    /// <summary>
    /// Cria a requisição, chama a API externa correspondente ao TipoParam
    /// e persiste os dados retornados como DadoTemporal.
    /// </summary>
    Task<ReqApiResponse> CreateAsync(ReqApiRequest request);
 
    bool Delete(Guid id);
}