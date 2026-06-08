using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Repositories;

public sealed class TelefoneRepository(TerraNovaContext context)
    : Repository<Telefone>(context), ITelefoneRepository
{
    public Telefone? GetByProdutorId(Guid produtorId) =>
        Context.Telefones
            .AsNoTracking()
            .FirstOrDefault(t => t.ProdutorId == produtorId);

    public bool ExistsByProdutorId(Guid produtorId) =>
        Context.Telefones
            .AsNoTracking()
            .Count(t => t.ProdutorId == produtorId) > 0;

    public bool ExistsByDddNumero(string ddd, string numero) =>
        Context.Telefones
            .AsNoTracking()
            .Count(t => t.Ddd == ddd.Trim() && t.Numero == numero.Trim()) > 0;

    public bool ExistsByDddNumeroExceptId(string ddd, string numero, Guid telefoneId) =>
        Context.Telefones
            .AsNoTracking()
            .Count(t =>
                t.Ddd == ddd.Trim()
                && t.Numero == numero.Trim()
                && t.Id != telefoneId) > 0;

    public bool ExistsByDddNumeroExceptProdutorId(string ddd, string numero, Guid produtorId) =>
        Context.Telefones
            .AsNoTracking()
            .Count(t =>
                t.Ddd == ddd.Trim()
                && t.Numero == numero.Trim()
                && t.ProdutorId != produtorId) > 0;
}
