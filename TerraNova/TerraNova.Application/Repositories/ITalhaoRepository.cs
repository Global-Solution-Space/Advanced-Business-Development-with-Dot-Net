using TerraNova.Domain.Entities;

namespace TerraNova.Application.Repositories;

public interface ITalhaoRepository : IRepository<Talhao>
{
    IReadOnlyList<Talhao> GetByPropriedadeId(Guid propriedadeId);
    IReadOnlyList<Talhao> GetByTipoPlantacaoId(Guid tipoPlantacaoId);
    Talhao? GetByIdWithLocalizacao(Guid id); 
}