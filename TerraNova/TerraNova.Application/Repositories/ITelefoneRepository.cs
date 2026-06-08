using TerraNova.Domain.Entities;

namespace TerraNova.Application.Repositories;

public interface ITelefoneRepository : IRepository<Telefone>
{
    Telefone? GetByProdutorId(Guid produtorId);
    bool ExistsByProdutorId(Guid produtorId);
    bool ExistsByDddNumero(string ddd, string numero);
    bool ExistsByDddNumeroExceptId(string ddd, string numero, Guid telefoneId);
    bool ExistsByDddNumeroExceptProdutorId(string ddd, string numero, Guid produtorId);
}
