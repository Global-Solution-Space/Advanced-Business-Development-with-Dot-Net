using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.DTOs;

public record AlertaAgricolaResponse(
    Guid        Id,
    string      Titulo,
    string      Descricao,
    NivelAlerta NivelAlerta,
    bool        Resolvido,
    DateTime    DataAlerta,
    Guid        SatvegId,
    Guid        NasaPowerId)
{
    public static AlertaAgricolaResponse FromDomain(AlertaAgricola a) =>
        new(a.Id, a.Titulo, a.Descricao, a.NivelAlerta,
            a.Resolvido, a.DataAlerta, a.SatvegId, a.NasaPowerId);
}
