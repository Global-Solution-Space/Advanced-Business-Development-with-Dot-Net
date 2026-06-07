using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Infrastructure.Persistence.Repositories;

public sealed class AlertaAgricolaRepository(TerraNovaContext context)
    : Repository<AlertaAgricola>(context), IAlertaAgricolaRepository
{

    public IReadOnlyList<AlertaAgricola> GetByTalhaoId(Guid talhaoId) =>
        Context.Alertas.AsNoTracking()
            .Where(a => a.TalhaoId == talhaoId)
            .OrderByDescending(a => a.DataAlerta)
            .ToList();

    public IReadOnlyList<AlertaAgricola> GetByNivelAlerta(NivelAlerta nivel) =>
        Context.Alertas.AsNoTracking()
            .Where(a => a.NivelAlerta == nivel)
            .OrderByDescending(a => a.DataAlerta)
            .ToList();

    public IReadOnlyList<AlertaAgricola> GetNaoResolvidos() =>
        Context.Alertas.AsNoTracking()
            .Where(a => !a.Resolvido)
            .OrderByDescending(a => a.NivelAlerta)
            .ThenByDescending(a => a.DataAlerta)
            .ToList();

    public bool ExisteAlertaAtivo(Guid talhaoId, string titulo) =>
        Context.Alertas.Count(a => a.TalhaoId == talhaoId && a.Titulo == titulo && !a.Resolvido) > 0;
}