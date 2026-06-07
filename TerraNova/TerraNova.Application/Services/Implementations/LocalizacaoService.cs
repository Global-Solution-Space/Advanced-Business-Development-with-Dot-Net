using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.Services.Implementations;

public sealed class LocalizacaoService(IRepository<Localizacao> localizacaoRepository) : ILocalizacaoService
{
    public IReadOnlyList<LocalizacaoResponse> GetAll() =>
        localizacaoRepository.GetAll().Select(LocalizacaoResponse.FromDomain).ToList();

    public LocalizacaoResponse? GetById(Guid id)
    {
        var loc = localizacaoRepository.GetById(id);
        return loc is null ? null : LocalizacaoResponse.FromDomain(loc);
    }

    public LocalizacaoResponse Create(LocalizacaoRequest request)
    {
        var loc = request.ToDomain();
        localizacaoRepository.Add(loc);
        return LocalizacaoResponse.FromDomain(loc);
    }

    public LocalizacaoResponse Update(Guid id, LocalizacaoRequest request)
    {
        _ = localizacaoRepository.GetById(id)
            ?? throw new InvalidOperationException("Localização não encontrada.");

        var entity = request.ToDomain();
        localizacaoRepository.Update(id, entity);
        return LocalizacaoResponse.FromDomain(entity);
    }

    public bool Delete(Guid id) => localizacaoRepository.Delete(id);
}