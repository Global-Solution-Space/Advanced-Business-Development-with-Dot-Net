using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Repositories;

public sealed class ProdutorRepository(TerraNovaContext context)
    : Repository<Produtor>(context), IProdutorRepository
{
    public Produtor? GetByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        var normalizado = email.Trim().ToLowerInvariant();
        return Context.Produtores.AsNoTracking()
            .FirstOrDefault(p => p.Email.ToLower() == normalizado);
    }
 
    public bool ExistsByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        var normalizado = email.Trim().ToLowerInvariant();
        return Context.Produtores.Any(p => p.Email.ToLower() == normalizado);
    }
}