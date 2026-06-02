using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Dados climáticos obtidos via NASA POWER para um <see cref="Talhao"/>.
/// Datas no formato YYYYMMDD (armazenadas como VARCHAR2(8) no Oracle).
/// </summary>
public sealed class NasaPower : BaseEntity
{
    /// <summary>Data de início do período (YYYYMMDD).</summary>
    public string  DataInicio { get; private set; } = string.Empty;
 
    /// <summary>Data de fim do período (YYYYMMDD).</summary>
    public string  DataFim    { get; private set; } = string.Empty;
 
    public decimal Latitude   { get; private set; }
    public decimal Longitude  { get; private set; }
    public decimal Elevacao   { get; private set; }
 
    public Guid    TalhaoId { get; private set; }
    public Talhao? Talhao   { get; private set; }
 
    public List<AlertaAgricola> Alertas { get; private set; } = [];
 
    private NasaPower() { }
 
    public NasaPower(
        string  dataInicio,
        string  dataFim,
        decimal latitude,
        decimal longitude,
        decimal elevacao,
        Guid    talhaoId)
    {
        if (string.IsNullOrWhiteSpace(dataInicio) || dataInicio.Trim().Length != 8)
            throw new DomainException("A data de início deve estar no formato YYYYMMDD (8 dígitos).");
 
        if (string.IsNullOrWhiteSpace(dataFim) || dataFim.Trim().Length != 8)
            throw new DomainException("A data de fim deve estar no formato YYYYMMDD (8 dígitos).");
 
        if (string.Compare(dataInicio.Trim(), dataFim.Trim(), StringComparison.Ordinal) > 0)
            throw new DomainException("A data de início não pode ser posterior à data de fim.");
 
        if (latitude < -90 || latitude > 90)
            throw new DomainException("Latitude deve estar entre -90 e 90.");
 
        if (longitude < -180 || longitude > 180)
            throw new DomainException("Longitude deve estar entre -180 e 180.");
 
        if (talhaoId == Guid.Empty)
            throw new DomainException("O registro NASA POWER deve estar associado a um talhão válido.");
 
        DataInicio = dataInicio.Trim();
        DataFim    = dataFim.Trim();
        Latitude   = latitude;
        Longitude  = longitude;
        Elevacao   = elevacao;
        TalhaoId   = talhaoId;
    }
}