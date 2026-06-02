using TerraNova.Domain.Entities;

namespace TerraNova.Application.Repositories;

public interface IProdutorRepository : IRepository<Produtor>
{
    Produtor? GetByEmail(string email);
    bool ExistsByEmail(string email);
}
