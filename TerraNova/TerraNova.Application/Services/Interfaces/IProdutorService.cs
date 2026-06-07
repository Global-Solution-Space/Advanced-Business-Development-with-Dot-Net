using TerraNova.Application.DTOs;

namespace TerraNova.Application.Services.Interfaces;

public interface IProdutorService
{
    IReadOnlyList<ProdutorResponse> GetAll();
    ProdutorResponse? GetById(Guid id);
    ProdutorResponse? GetByEmail(string email);
    ProdutorResponse Create(ProdutorRequest request);
    ProdutorResponse Update(Guid id, ProdutorRequest request);
    bool Delete(Guid id);
}