using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.DTOs;

/// <summary>Alerta agrícola retornado pela API.</summary>
/// <param name="Id">Identificador do alerta.</param>
/// <param name="Titulo">Título curto do alerta.</param>
/// <param name="Descricao">Descrição do risco ou evento observado.</param>
/// <param name="NivelAlerta">Nível de severidade do alerta.</param>
/// <param name="Resolvido">Indica se o alerta já foi resolvido.</param>
/// <param name="DataAlerta">Data e hora em que o alerta foi registrado.</param>
/// <param name="TalhaoId">Identificador do talhão afetado.</param>
public record AlertaAgricolaResponse(
    Guid        Id,
    string      Titulo,
    string      Descricao,
    NivelAlerta NivelAlerta,
    bool        Resolvido,
    DateTime    DataAlerta,
    Guid        TalhaoId)
{
    public static AlertaAgricolaResponse FromDomain(AlertaAgricola a) =>
        new(a.Id, a.Titulo, a.Descricao, a.NivelAlerta, a.Resolvido, a.DataAlerta, a.TalhaoId);
}