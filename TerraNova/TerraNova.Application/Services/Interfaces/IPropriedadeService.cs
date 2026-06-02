using TerraNova.Application.DTOs;

namespace TerraNova.Application.Services.Interfaces;

public interface IPropriedadeService
{
    IReadOnlyList<PropriedadeResponse> GetAll();
    PropriedadeResponse? GetById(Guid id);
    IReadOnlyList<PropriedadeResponse> GetByProdutorId(Guid produtorId);
    PropriedadeResponse Create(PropriedadeRequest request);
    bool Delete(Guid id);
}