using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.Services.Implementations;

public sealed class TelefoneService(
    ITelefoneRepository telefoneRepository,
    IProdutorRepository produtorRepository) : ITelefoneService
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
        var t = telefoneRepository.GetByProdutorId(produtorId);
        return t is null ? null : TelefoneResponse.FromDomain(t);
    }

    public TelefoneResponse Create(TelefoneRequest request)
    {
        if (!produtorRepository.ExistsById(request.ProdutorId))
            throw new InvalidOperationException("Produtor não encontrado.");

        if (telefoneRepository.ExistsByProdutorId(request.ProdutorId))
            throw new InvalidOperationException("Este produtor já possui um telefone detalhado cadastrado.");

        if (telefoneRepository.ExistsByDddNumero(request.Ddd, request.Numero))
            throw new InvalidOperationException("Este DDD e número já estão cadastrados.");

        var telefone = request.ToDomain();
        telefoneRepository.Add(telefone);
        return TelefoneResponse.FromDomain(telefone);
    }

    public TelefoneResponse Update(Guid id, TelefoneRequest request)
    {
        var existing = telefoneRepository.GetById(id)
                       ?? throw new KeyNotFoundException("Telefone não encontrado.");

        if (telefoneRepository.ExistsByDddNumeroExceptId(request.Ddd, request.Numero, id))
            throw new InvalidOperationException("Este DDD e número já estão cadastrados para outro telefone.");

        existing.Atualizar(request.Ddd, request.Numero);
        telefoneRepository.Update(id, existing);
        return TelefoneResponse.FromDomain(existing);
    }

    public bool Delete(Guid id) => telefoneRepository.Delete(id);
}
