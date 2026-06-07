using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.DTOs;

/// <summary>Dados para criar um alerta agrícola manual associado a um talhão.</summary>
/// <param name="Titulo">Título curto do alerta.</param>
/// <param name="Descricao">Descrição do risco ou evento observado.</param>
/// <param name="NivelAlerta">Nível de severidade do alerta.</param>
/// <param name="TalhaoId">Identificador do talhão afetado.</param>
public record AlertaAgricolaRequest(
    [Required][StringLength(100, MinimumLength = 2)] string      Titulo,
    [Required][StringLength(300, MinimumLength = 2)] string      Descricao,
    [Required]                                       NivelAlerta NivelAlerta,
    [Required]                                       Guid        TalhaoId) 
{
    public AlertaAgricola ToDomain() => new(Titulo, Descricao, NivelAlerta, TalhaoId);
}