using TerraNova.Application.DTOs;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.Services.Interfaces;

public interface IAlertaAgricolaService
{
    IReadOnlyList<AlertaAgricolaResponse> GetAll();
    AlertaAgricolaResponse? GetById(Guid id);
    IReadOnlyList<AlertaAgricolaResponse> GetBySatvegId(Guid satvegId);
    IReadOnlyList<AlertaAgricolaResponse> GetByNasaPowerId(Guid nasaPowerId);
    IReadOnlyList<AlertaAgricolaResponse> GetByNivelAlerta(NivelAlerta nivel);
    IReadOnlyList<AlertaAgricolaResponse> GetNaoResolvidos();
    AlertaAgricolaResponse Create(AlertaAgricolaRequest request);
 
    /// <summary>Marca o alerta como resolvido.</summary>
    AlertaAgricolaResponse Resolver(Guid id);
 
    bool Delete(Guid id);
}