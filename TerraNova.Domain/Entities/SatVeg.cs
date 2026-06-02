using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Análise de vegetação via plataforma SatVeg para um <see cref="Talhao"/>.
/// Armazena os parâmetros da consulta e o polígono de área.
/// </summary>
public sealed class Satveg : BaseEntity
{
    /// <summary>Tipo de perfil de índice de vegetação (ex.: NDVI, EVI).</summary>
    public int  TipoPerfil       { get; private set; }
 
    /// <summary>Código do satélite consultado.</summary>
    public int  Satelite         { get; private set; }
 
    /// <summary>Pré-filtro aplicado (nullable).</summary>
    public bool? PreFiltro       { get; private set; }
 
    /// <summary>Sigla do filtro temporal (ex.: SG, WS) — máx. 3 caracteres (nullable).</summary>
    public string? Filtro        { get; private set; }
 
    /// <summary>Parâmetro do filtro (nullable).</summary>
    public int? ParametroFiltro  { get; private set; }
 
    /// <summary>Polígono WKT da área de análise (CLOB).</summary>
    public string Poligono       { get; private set; } = string.Empty;
 
    /// <summary>Indica se todas as estatísticas foram solicitadas.</summary>
    public bool TodasEstatisticas { get; private set; }
 
    public DateTime DataAnalise { get; private set; }
 
    public Guid    TalhaoId { get; private set; }
    public Talhao? Talhao   { get; private set; }
 
    public List<AlertaAgricola> Alertas { get; private set; } = [];
 
    private Satveg() { }
 
    public Satveg(
        int      tipoPerfil,
        int      satelite,
        bool?    preFiltro,
        string?  filtro,
        int?     parametroFiltro,
        string   poligono,
        bool     todasEstatisticas,
        DateTime dataAnalise,
        Guid     talhaoId)
    {
        if (tipoPerfil < 0)
            throw new DomainException("O tipo de perfil deve ser um valor não negativo.");
 
        if (satelite < 0)
            throw new DomainException("O código do satélite deve ser um valor não negativo.");
 
        if (string.IsNullOrWhiteSpace(poligono))
            throw new DomainException("O polígono não pode ser vazio.");
 
        if (filtro != null && filtro.Trim().Length > 3)
            throw new DomainException("O filtro deve ter no máximo 3 caracteres.");
 
        if (parametroFiltro.HasValue && (parametroFiltro < 0 || parametroFiltro > 99))
            throw new DomainException("O parâmetro de filtro deve estar entre 0 e 99.");
 
        if (talhaoId == Guid.Empty)
            throw new DomainException("A análise SatVeg deve estar associada a um talhão válido.");
 
        TipoPerfil        = tipoPerfil;
        Satelite          = satelite;
        PreFiltro         = preFiltro;
        Filtro            = filtro?.Trim();
        ParametroFiltro   = parametroFiltro;
        Poligono          = poligono.Trim();
        TodasEstatisticas = todasEstatisticas;
        DataAnalise       = dataAnalise;
        TalhaoId          = talhaoId;
    }
}