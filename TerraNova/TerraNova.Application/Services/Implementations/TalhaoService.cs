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

        var propriedade = propriedadeRepository.GetById(request.PropriedadeId)
            ?? throw new InvalidOperationException("Propriedade não encontrada.");

        // Soma das áreas direto no banco (evita carregar todos os talhões na memória)
        var areaExistente = talhaoRepository.SomarAreaPorPropriedade(request.PropriedadeId);
        if (areaExistente + request.VolumArea > propriedade.TamanhoTotal)
            throw new InvalidOperationException("A soma das áreas dos talhões não pode exceder o tamanho total da propriedade.");

        if (!localizacaoRepository.ExistsById(request.LocalizacaoId))
            throw new InvalidOperationException("Localização não encontrada.");

        // Localização exclusiva: verifica com COUNT direto no banco (evita GetAll completo)
        if (talhaoRepository.ExistsByLocalizacaoId(request.LocalizacaoId))
            throw new InvalidOperationException("Esta localização já está associada a outro talhão.");

        var talhao = request.ToDomain();
        talhaoRepository.Add(talhao);
        return TalhaoResponse.FromDomain(talhao);
    }

    public TalhaoResponse Update(Guid id, TalhaoRequest request)
    {
        var existing = talhaoRepository.GetById(id)
                       ?? throw new InvalidOperationException("Talhão não encontrado.");

        if (!tipoPlantacaoRepository.ExistsById(request.TipoPlantacaoId))
            throw new InvalidOperationException("Tipo de plantação não encontrado.");

        var propriedade = propriedadeRepository.GetById(request.PropriedadeId)
            ?? throw new InvalidOperationException("Propriedade não encontrada.");

        var areaExistente = talhaoRepository.SomarAreaPorPropriedade(request.PropriedadeId);
        var areaAtual = existing.PropriedadeId == request.PropriedadeId ? existing.VolumArea : 0m;
        if (areaExistente - areaAtual + request.VolumArea > propriedade.TamanhoTotal)
            throw new InvalidOperationException("A soma das áreas dos talhões não pode exceder o tamanho total da propriedade.");

        if (!localizacaoRepository.ExistsById(request.LocalizacaoId))
            throw new InvalidOperationException("Localização não encontrada.");

        if (talhaoRepository.ExistsByLocalizacaoId(request.LocalizacaoId)
            && existing.LocalizacaoId != request.LocalizacaoId)
            throw new InvalidOperationException("Esta localização já está associada a outro talhão.");

        var entity = request.ToDomain();
        talhaoRepository.Update(id, entity);
        return TalhaoResponse.FromDomain(entity);
    }

    public bool Delete(Guid id) => talhaoRepository.Delete(id);
}