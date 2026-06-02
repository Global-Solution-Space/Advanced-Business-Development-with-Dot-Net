using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.Application.Services.Implementations;

public sealed class ProdutorService(IProdutorRepository produtorRepository) : IProdutorService
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
        produtorRepository.Add(produtor);
        return ProdutorResponse.FromDomain(produtor);
    }
 
    public bool Delete(Guid id) => produtorRepository.Delete(id);
}