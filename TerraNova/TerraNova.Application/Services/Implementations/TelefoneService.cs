using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.Services.Implementations;

public sealed class TelefoneService(
    IRepository<Telefone> telefoneRepository,
    IProdutorRepository   produtorRepository) : ITelefoneService
{
    public IReadOnlyList<TelefoneResponse> GetAll() =>
        telefoneRepository.GetAll().Select(TelefoneResponse.FromDomain).ToList();

    public TelefoneResponse? GetById(Guid id)
    {
        var t = telefoneRepository.GetById(id);
        return t is null ? null : TelefoneResponse.FromDomain(t);
    }

    public TelefoneResponse? GetByProdutorId(Guid produtorId)
    {
        var t = telefoneRepository.GetAll().FirstOrDefault(t => t.ProdutorId == produtorId);
        return t is null ? null : TelefoneResponse.FromDomain(t);
    }

    public TelefoneResponse Create(TelefoneRequest request)
    {
        if (!produtorRepository.ExistsById(request.ProdutorId))
            throw new InvalidOperationException("Produtor não encontrado.");

        var jaExiste = telefoneRepository.GetAll().Any(t => t.ProdutorId == request.ProdutorId);
        if (jaExiste)
            throw new InvalidOperationException("Este produtor já possui um telefone detalhado cadastrado.");

        var telefone = request.ToDomain();
        telefoneRepository.Add(telefone);
        return TelefoneResponse.FromDomain(telefone);
    }

    public TelefoneResponse Update(Guid id, TelefoneRequest request)
    {
        var existing = telefoneRepository.GetById(id)
                       ?? throw new InvalidOperationException("Telefone não encontrado.");

        var entity = new Telefone(request.Ddd, request.Numero, existing.ProdutorId);
        telefoneRepository.Update(id, entity);
        return TelefoneResponse.FromDomain(entity);
    }

    public bool Delete(Guid id) => telefoneRepository.Delete(id);
}