namespace TerraNova.Domain.Enums;

/// <summary>
/// Nível de severidade de um alerta agrícola.
/// Persiste como string no banco (HasConversion) para legibilidade das queries Oracle.
/// </summary>
public enum NivelAlerta
{
    Baixo   = 0,
    Medio   = 1,
    Alto    = 2,
    Critico = 3
}
