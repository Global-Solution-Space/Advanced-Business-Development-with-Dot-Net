namespace TerraNova.Domain.Enums;

/// <summary>
/// Parâmetro consultado em uma requisição externa.
/// Persiste como string (HasConversion) para respeitar o CHECK do Oracle.
/// </summary>
public enum TipoParamReqApi
{
    /// <summary>Índice de Vegetação — consultado via SatVeg/Embrapa.</summary>
    Ndvi        = 0,
 
    /// <summary>Precipitação Corrigida Total — consultada via NASA POWER.</summary>
    Prectotcorr = 1
}
