using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.Services.Implementations;
 
public sealed class TalhaoService(
    ITalhaoRepository        talhaoRepository,
    IRepository<Propriedade> propriedadeRepository,
    IRepository<TipoPlantacao> tipoPlantacaoRepository,
    IRepository<Localizacao> localizacaoRepository) : ITalhaoService
{
    public IReadOnlyList<TalhaoResponse> GetAll() =>
        talhaoRepository.GetAll().Select(TalhaoResponse.FromDomain).ToList();
 
    public TalhaoResponse? GetById(Guid id)
    {
        var t = talhaoRepository.GetById(id);
        return t is null ? null : TalhaoResponse.FromDomain(t);
    }
 
    public IReadOnlyList<TalhaoResponse> GetByPropriedadeId(Guid propriedadeId) =>
        talhaoRepository.GetByPropriedadeId(propriedadeId)
            .Select(TalhaoResponse.FromDomain).ToList();
 
    public IReadOnlyList<TalhaoResponse> GetByTipoPlantacaoId(Guid tipoPlantacaoId) =>
        talhaoRepository.GetByTipoPlantacaoId(tipoPlantacaoId)
            .Select(TalhaoResponse.FromDomain).ToList();
 
    public TalhaoResponse Create(TalhaoRequest request)
    {
        if (!tipoPlantacaoRepository.ExistsById(request.TipoPlantacaoId))
            throw new InvalidOperationException("Tipo de plantação não encontrado.");
 
        if (!propriedadeRepository.ExistsById(request.PropriedadeId))
            throw new InvalidOperationException("Propriedade não encontrada.");
 
        if (!localizacaoRepository.ExistsById(request.LocalizacaoId))
            throw new InvalidOperationException("Localização não encontrada.");
 
        // Localização exclusiva: não pode estar associada a outra propriedade ou talhão
        var localizacaoEmUso = talhaoRepository.GetAll()
            .Any(t => t.LocalizacaoId == request.LocalizacaoId);
 
        if (localizacaoEmUso)
            throw new InvalidOperationException("Esta localização já está associada a outro talhão.");
 
        var talhao = request.ToDomain();
        talhaoRepository.Add(talhao);
        return TalhaoResponse.FromDomain(talhao);
    }
 
    public bool Delete(Guid id) => talhaoRepository.Delete(id);
}