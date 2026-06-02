using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.Services.Implementations;

public sealed class AlertaAgricolaService(
    IAlertaAgricolaRepository alertaRepository,
    ISatvegRepository         satvegRepository,
    INasaPowerRepository      nasaPowerRepository) : IAlertaAgricolaService
{
    public IReadOnlyList<AlertaAgricolaResponse> GetAll() =>
        alertaRepository.GetAll().Select(AlertaAgricolaResponse.FromDomain).ToList();
 
    public AlertaAgricolaResponse? GetById(Guid id)
    {
        var a = alertaRepository.GetById(id);
        return a is null ? null : AlertaAgricolaResponse.FromDomain(a);
    }
 
    public IReadOnlyList<AlertaAgricolaResponse> GetBySatvegId(Guid satvegId) =>
        alertaRepository.GetBySatvegId(satvegId).Select(AlertaAgricolaResponse.FromDomain).ToList();
 
    public IReadOnlyList<AlertaAgricolaResponse> GetByNasaPowerId(Guid nasaPowerId) =>
        alertaRepository.GetByNasaPowerId(nasaPowerId).Select(AlertaAgricolaResponse.FromDomain).ToList();
 
    public IReadOnlyList<AlertaAgricolaResponse> GetByNivelAlerta(NivelAlerta nivel) =>
        alertaRepository.GetByNivelAlerta(nivel).Select(AlertaAgricolaResponse.FromDomain).ToList();
 
    public IReadOnlyList<AlertaAgricolaResponse> GetNaoResolvidos() =>
        alertaRepository.GetNaoResolvidos().Select(AlertaAgricolaResponse.FromDomain).ToList();
 
    public AlertaAgricolaResponse Create(AlertaAgricolaRequest request)
    {
        var satveg = satvegRepository.GetById(request.SatvegId)
            ?? throw new InvalidOperationException("Análise SatVeg não encontrada.");
 
        var nasaPower = nasaPowerRepository.GetById(request.NasaPowerId)
            ?? throw new InvalidOperationException("Registro NASA POWER não encontrado.");
 
        // Regra de negócio: SatVeg e NasaPower devem pertencer ao mesmo talhão
        if (satveg.TalhaoId != nasaPower.TalhaoId)
            throw new InvalidOperationException(
                "A análise SatVeg e o registro NASA POWER devem pertencer ao mesmo talhão.");
 
        var alerta = request.ToDomain();
        alertaRepository.Add(alerta);
        return AlertaAgricolaResponse.FromDomain(alerta);
    }
 
    public AlertaAgricolaResponse Resolver(Guid id)
    {
        var alerta = alertaRepository.GetById(id)
            ?? throw new InvalidOperationException("Alerta agrícola não encontrado.");
 
        alerta.Resolver();
        alertaRepository.Update(alerta);
        return AlertaAgricolaResponse.FromDomain(alerta);
    }
 
    public bool Delete(Guid id) => alertaRepository.Delete(id);
}