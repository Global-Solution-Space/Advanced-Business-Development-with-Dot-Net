using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.Services.Implementations;

public sealed class TipoPlantacaoService(IRepository<TipoPlantacao> tipoPlantacaoRepository) : ITipoPlantacaoService
{
    public IReadOnlyList<TipoPlantacaoResponse> GetAll() =>
        tipoPlantacaoRepository.GetAll().Select(TipoPlantacaoResponse.FromDomain).ToList();
 
    public TipoPlantacaoResponse? GetById(Guid id)
    {
        var t = tipoPlantacaoRepository.GetById(id);
        return t is null ? null : TipoPlantacaoResponse.FromDomain(t);
    }
 
    public TipoPlantacaoResponse Create(TipoPlantacaoRequest request)
    {
        var tipo = request.ToDomain();
        tipoPlantacaoRepository.Add(tipo);
        return TipoPlantacaoResponse.FromDomain(tipo);
    }
 
    public bool Delete(Guid id) => tipoPlantacaoRepository.Delete(id);
}