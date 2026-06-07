using TerraNova.Application.DTOs;

namespace TerraNova.Application.Services.Interfaces;

public interface ITelefoneService
{
    IReadOnlyList<TelefoneResponse> GetAll();
    TelefoneResponse? GetById(Guid id);
    TelefoneResponse? GetByProdutorId(Guid produtorId);
    TelefoneResponse Create(TelefoneRequest request);
    TelefoneResponse Update(Guid id, TelefoneUpdateRequest request);
    bool Delete(Guid id);
}