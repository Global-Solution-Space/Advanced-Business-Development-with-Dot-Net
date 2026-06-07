using TerraNova.Application.DTOs;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.Services.Interfaces;

public interface IAlertaAgricolaService
{
    IReadOnlyList<AlertaAgricolaResponse> GetAll();
    AlertaAgricolaResponse? GetById(Guid id);
    IReadOnlyList<AlertaAgricolaResponse> GetByTalhaoId(Guid talhaoId);
    IReadOnlyList<AlertaAgricolaResponse> GetByNivelAlerta(NivelAlerta nivel);
    IReadOnlyList<AlertaAgricolaResponse> GetNaoResolvidos();
    AlertaAgricolaResponse Create(AlertaAgricolaRequest request);
    AlertaAgricolaResponse Resolver(Guid id);
    AlertaAgricolaResponse Reabrir(Guid id);
    bool Delete(Guid id);
}