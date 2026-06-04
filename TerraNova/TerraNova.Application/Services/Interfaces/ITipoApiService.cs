using TerraNova.Application.DTOs;

namespace TerraNova.Application.Services.Interfaces;

public interface ITipoApiService
{
    IReadOnlyList<TipoApiResponse> GetAll();
    TipoApiResponse? GetById(Guid id);
    TipoApiResponse Create(TipoApiRequest request);
    bool Delete(Guid id);
}
