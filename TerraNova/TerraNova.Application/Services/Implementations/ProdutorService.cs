using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.Services.Implementations;

public sealed class ProdutorService(
    IProdutorRepository produtorRepository,
    IRepository<Telefone> telefoneRepository) : IProdutorService
{
    public IReadOnlyList<ProdutorResponse> GetAll() =>
        produtorRepository.GetAll().Select(ProdutorResponse.FromDomain).ToList();

    public ProdutorResponse? GetById(Guid id)
    {
        var p = produtorRepository.GetById(id);
        return p is null ? null : ProdutorResponse.FromDomain(p);
    }

    public ProdutorResponse? GetByEmail(string email)
    {
        var p = produtorRepository.GetByEmail(email);
        return p is null ? null : ProdutorResponse.FromDomain(p);
    }

    public ProdutorResponse Create(ProdutorRequest request)
    {
        if (produtorRepository.ExistsByEmail(request.Email))
            throw new InvalidOperationException("Já existe um produtor cadastrado com este e-mail.");

        var produtor = request.ToDomain();

        // Extrai DDD e Número da string limpa enviada no request
        var (ddd, numero) = ExtrairTelefone(request.TelefoneContato);

        // Cria a entidade Telefone e associa ao Produtor
        var telefoneDetalhado = new Telefone(ddd, numero, produtor.Id);
        produtor.AtribuirTelefone(telefoneDetalhado);

        produtorRepository.Add(produtor);
        return ProdutorResponse.FromDomain(produtor);
    }

    public ProdutorResponse Update(Guid id, ProdutorRequest request)
    {
        var existing = produtorRepository.GetById(id)
                       ?? throw new InvalidOperationException("Produtor não encontrado.");

        var emailExistente = produtorRepository.GetByEmail(request.Email);
        if (emailExistente is not null && emailExistente.Id != id)
            throw new InvalidOperationException("Já existe um produtor cadastrado com este e-mail.");

        var entity = new Produtor(request.Nome, request.Email, request.Senha);
        produtorRepository.Update(id, entity);

        var telefone = AtualizarTelefoneContato(id, request.TelefoneContato, existing.TelefoneDetalhado?.Id);
        entity.AtribuirTelefone(telefone);

        return ProdutorResponse.FromDomain(entity);
    }

    public bool Delete(Guid id) => produtorRepository.Delete(id);

    private Telefone AtualizarTelefoneContato(Guid produtorId, string telefoneContato, Guid? telefoneId)
    {
        var (ddd, numero) = ExtrairTelefone(telefoneContato);
        var telefone = new Telefone(ddd, numero, produtorId);

        if (telefoneId.HasValue)
        {
            telefoneRepository.Update(telefoneId.Value, telefone);
        }
        else
        {
            telefoneRepository.Add(telefone);
        }

        return telefone;
    }

    private static (string Ddd, string Numero) ExtrairTelefone(string telefoneContato)
    {
        var telefoneLimpo = new string(telefoneContato.Where(char.IsDigit).ToArray());
        var ddd = telefoneLimpo.Substring(0, 2);
        var numero = telefoneLimpo.Substring(2);

        return (ddd, numero);
    }
}
