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

    public TipoPlantacaoResponse Update(Guid id, TipoPlantacaoRequest request)
    {
        var existing = tipoPlantacaoRepository.GetById(id)
            ?? throw new InvalidOperationException("Tipo de plantação não encontrado.");

        existing.Atualizar(request.TipoPlant);
        tipoPlantacaoRepository.Update(id, existing);
        return TipoPlantacaoResponse.FromDomain(existing);
    }

    public bool Delete(Guid id) => tipoPlantacaoRepository.Delete(id);
}