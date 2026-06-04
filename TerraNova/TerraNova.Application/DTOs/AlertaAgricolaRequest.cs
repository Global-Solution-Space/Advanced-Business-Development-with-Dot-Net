using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.DTOs;

public record AlertaAgricolaRequest(
    [Required][StringLength(100, MinimumLength = 2)] string      Titulo,
    [Required][StringLength(300, MinimumLength = 2)] string      Descricao,
    [Required]                                       NivelAlerta NivelAlerta,
    [Required]                                       Guid        TalhaoId) 
{
    public AlertaAgricola ToDomain() => new(Titulo, Descricao, NivelAlerta, TalhaoId);
}