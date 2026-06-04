using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.Services.Implementations;

public sealed class TipoApiService(IRepository<TipoApi> tipoApiRepository) : ITipoApiService
{
    public IReadOnlyList<TipoApiResponse> GetAll() =>
        tipoApiRepository.GetAll().Select(TipoApiResponse.FromDomain).ToList();
 
    public TipoApiResponse? GetById(Guid id)
    {
        var t = tipoApiRepository.GetById(id);
        return t is null ? null : TipoApiResponse.FromDomain(t);
    }
 
    public TipoApiResponse Create(TipoApiRequest request)
    {
        var tipo = request.ToDomain();
        tipoApiRepository.Add(tipo);
        return TipoApiResponse.FromDomain(tipo);
    }
 
    public bool Delete(Guid id) => tipoApiRepository.Delete(id);
}