using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.Repositories;

public interface IAlertaAgricolaRepository : IRepository<AlertaAgricola>
{
    IReadOnlyList<AlertaAgricola> GetByTalhaoId(Guid talhaoId);
    IReadOnlyList<AlertaAgricola> GetByNivelAlerta(NivelAlerta nivel);
    IReadOnlyList<AlertaAgricola> GetNaoResolvidos();
    bool ExisteAlertaAtivo(Guid talhaoId, string titulo);
}