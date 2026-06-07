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

    public TipoApiResponse Update(Guid id, TipoApiRequest request)
    {
        _ = tipoApiRepository.GetById(id)
            ?? throw new InvalidOperationException("Tipo de API não encontrado.");

        var entity = request.ToDomain();
        tipoApiRepository.Update(id, entity);
        return TipoApiResponse.FromDomain(entity);
    }

    public bool Delete(Guid id) => tipoApiRepository.Delete(id);
}