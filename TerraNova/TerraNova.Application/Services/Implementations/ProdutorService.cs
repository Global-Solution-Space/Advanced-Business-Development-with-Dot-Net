using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.Services.Implementations;

public sealed class ProdutorService(
    IProdutorRepository produtorRepository,
    ITelefoneRepository telefoneRepository) : IProdutorService
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
        if (telefoneRepository.ExistsByDddNumero(ddd, numero))
            throw new InvalidOperationException("Este DDD e número já estão cadastrados.");

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

        var produtor = new Produtor(request.Nome, request.Email, request.Senha);

        var (ddd, numero) = ExtrairTelefone(request.TelefoneContato);
        if (telefoneRepository.ExistsByDddNumeroExceptProdutorId(ddd, numero, id))
            throw new InvalidOperationException("Este DDD e número já estão cadastrados para outro telefone.");

        var telefone = new Telefone(ddd, numero, id);

        produtorRepository.Update(id, produtor);

        if (existing.TelefoneDetalhado?.Id is Guid telefoneId)
        {
            telefoneRepository.Update(telefoneId, telefone);
        }
        else
        {
            telefoneRepository.Add(telefone);
        }

        produtor.AtribuirTelefone(telefone);
        return ProdutorResponse.FromDomain(produtor);
    }

    public bool Delete(Guid id) => produtorRepository.Delete(id);

    private static (string Ddd, string Numero) ExtrairTelefone(string telefoneContato)
    {
        var telefoneLimpo = new string(telefoneContato.Where(char.IsDigit).ToArray());

        if (telefoneLimpo.Length is not (10 or 11))
            throw new InvalidOperationException("O telefone deve conter DDD e numero com 10 ou 11 digitos.");

        var ddd = telefoneLimpo[..2];
        var numero = telefoneLimpo[2..];

        return (ddd, numero);
    }
}
