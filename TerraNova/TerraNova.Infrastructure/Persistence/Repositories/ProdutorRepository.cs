using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Repositories;

public sealed class ProdutorRepository(TerraNovaContext context)
    : Repository<Produtor>(context), IProdutorRepository
{
    public override IReadOnlyList<Produtor> GetAll() =>
        Context.Produtores.AsNoTracking()
            .Include(p => p.TelefoneDetalhado)
            .OrderBy(p => p.Id)
            .ToList();

    public override Produtor? GetById(Guid id) =>
        Context.Produtores.AsNoTracking()
            .Include(p => p.TelefoneDetalhado)
            .FirstOrDefault(p => p.Id == id);

    public Produtor? GetByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        var normalizado = email.Trim().ToLowerInvariant();
        return Context.Produtores.AsNoTracking()
            .Include(p => p.TelefoneDetalhado)
            .FirstOrDefault(p => p.Email.ToLower() == normalizado);
    }
 
    public bool ExistsByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        var normalizado = email.Trim().ToLowerInvariant();
        return Context.Produtores.Count(p => p.Email.ToLower() == normalizado) > 0;
    }
}