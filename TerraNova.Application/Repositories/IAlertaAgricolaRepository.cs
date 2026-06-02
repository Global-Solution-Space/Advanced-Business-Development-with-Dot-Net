using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.Repositories;

public interface IAlertaAgricolaRepository : IRepository<AlertaAgricola>
{
    IReadOnlyList<AlertaAgricola> GetBySatvegId(Guid satvegId);
    IReadOnlyList<AlertaAgricola> GetByNasaPowerId(Guid nasaPowerId);
    IReadOnlyList<AlertaAgricola> GetByNivelAlerta(NivelAlerta nivel);
    IReadOnlyList<AlertaAgricola> GetNaoResolvidos();
}
