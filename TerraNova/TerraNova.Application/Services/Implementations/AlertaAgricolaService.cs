using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.Services.Implementations;

public sealed class AlertaAgricolaService(
    IAlertaAgricolaRepository alertaRepository,
    ITalhaoRepository         talhaoRepository) : IAlertaAgricolaService
{
    public IReadOnlyList<AlertaAgricolaResponse> GetAll() =>
        alertaRepository.GetAll().Select(AlertaAgricolaResponse.FromDomain).ToList();

    public AlertaAgricolaResponse? GetById(Guid id)
    {
        var a = alertaRepository.GetById(id);
        return a is null ? null : AlertaAgricolaResponse.FromDomain(a);
    }

    public IReadOnlyList<AlertaAgricolaResponse> GetByTalhaoId(Guid talhaoId) =>
        alertaRepository.GetByTalhaoId(talhaoId).Select(AlertaAgricolaResponse.FromDomain).ToList();

    public IReadOnlyList<AlertaAgricolaResponse> GetByNivelAlerta(NivelAlerta nivel) =>
        alertaRepository.GetByNivelAlerta(nivel).Select(AlertaAgricolaResponse.FromDomain).ToList();

    public IReadOnlyList<AlertaAgricolaResponse> GetNaoResolvidos() =>
        alertaRepository.GetNaoResolvidos().Select(AlertaAgricolaResponse.FromDomain).ToList();

    public AlertaAgricolaResponse Create(AlertaAgricolaRequest request)
    {
        if (!talhaoRepository.ExistsById(request.TalhaoId))
            throw new InvalidOperationException("Talhão não encontrado.");

        var alerta = request.ToDomain();
        alertaRepository.Add(alerta);
        return AlertaAgricolaResponse.FromDomain(alerta);
    }

    public AlertaAgricolaResponse Update(Guid id, AlertaAgricolaRequest request)
    {
        var alerta = alertaRepository.GetById(id)
            ?? throw new InvalidOperationException("Alerta agrícola não encontrado.");

        if (!talhaoRepository.ExistsById(request.TalhaoId))
            throw new InvalidOperationException("Talhão não encontrado.");

        alerta.Atualizar(request.Titulo, request.Descricao, request.NivelAlerta, request.TalhaoId);
        alertaRepository.Update(id, alerta);
        return AlertaAgricolaResponse.FromDomain(alerta);
    }

    public AlertaAgricolaResponse Resolver(Guid id)
    {
        var alerta = alertaRepository.GetById(id)
                     ?? throw new InvalidOperationException("Alerta agrícola não encontrado.");

        alerta.Resolver();
        alertaRepository.Update(alerta.Id, alerta);
        return AlertaAgricolaResponse.FromDomain(alerta);
    }

    public AlertaAgricolaResponse Reabrir(Guid id)
    {
        var alerta = alertaRepository.GetById(id)
                     ?? throw new InvalidOperationException("Alerta agrícola não encontrado.");

        alerta.Reabrir();
        alertaRepository.Update(alerta.Id, alerta);
        return AlertaAgricolaResponse.FromDomain(alerta);
    }

    public bool Delete(Guid id) => alertaRepository.Delete(id);
}