using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.Services.Implementations;

public sealed class PropriedadeService(
    IPropriedadeRepository   propriedadeRepository,
    IProdutorRepository      produtorRepository,
    IRepository<Localizacao> localizacaoRepository) : IPropriedadeService
{
    public IReadOnlyList<PropriedadeResponse> GetAll() =>
        propriedadeRepository.GetAll().Select(PropriedadeResponse.FromDomain).ToList();

    public PropriedadeResponse? GetById(Guid id)
    {
        var p = propriedadeRepository.GetById(id);
        return p is null ? null : PropriedadeResponse.FromDomain(p);
    }
 
    // Filtro feito pelo Oracle via WHERE — não carrega tudo em memória
    public IReadOnlyList<PropriedadeResponse> GetByProdutorId(Guid produtorId) =>
        propriedadeRepository.GetByProdutorId(produtorId)
            .Select(PropriedadeResponse.FromDomain)
            .ToList();

    public PropriedadeResponse Create(PropriedadeRequest request)
    {
        if (!produtorRepository.ExistsById(request.ProdutorId))
            throw new InvalidOperationException("Produtor não encontrado.");

        if (!localizacaoRepository.ExistsById(request.LocalizacaoId))
            throw new InvalidOperationException("Localização não encontrada.");
 
        // COUNT() direto no banco — evita GetAll em memória
        if (propriedadeRepository.ExistsByLocalizacaoId(request.LocalizacaoId))
            throw new InvalidOperationException("Esta localização já está associada a outra propriedade.");
 
        var propriedade = request.ToDomain();
        propriedadeRepository.Add(propriedade);
        return PropriedadeResponse.FromDomain(propriedade);
    }
 
    public bool Delete(Guid id) => propriedadeRepository.Delete(id);
}