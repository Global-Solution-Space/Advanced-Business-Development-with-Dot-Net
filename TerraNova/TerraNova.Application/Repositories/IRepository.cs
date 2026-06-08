using TerraNova.Domain.Common;
using TerraNova.Domain.Entities;
namespace TerraNova.Application.Repositories;

/// <summary>Contrato genérico de persistência para entidades que derivam de <see cref="BaseEntity"/>.</summary>
public interface IRepository<T> where T : BaseEntity
{
    IReadOnlyList<T> GetAll();
    T? GetById(Guid id);
    T  Add(T entity);
    T  Update(Guid id, T entity);
    bool Delete(Guid id);
    bool ExistsById(Guid id);
    bool ExistsByNome(string valor);
}
