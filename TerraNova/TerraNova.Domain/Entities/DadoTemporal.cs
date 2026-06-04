using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Ponto de série temporal retornado por uma API externa.
/// Liga <see cref="ReqApi"/> (qual consulta gerou o dado) ao <see cref="Talhao"/>
/// (a qual talhão o valor se refere).
/// </summary>
public sealed class DadoTemporal : BaseEntity
{
    public DateTime DataLeitura { get; private set; }
    public decimal  Valor       { get; private set; }
 
    public Guid    TalhaoId { get; private set; }
    public Talhao? Talhao   { get; private set; }
 
    public Guid    ReqApiId { get; private set; }
    public ReqApi? ReqApi   { get; private set; }
 
    private DadoTemporal() { }
 
    public DadoTemporal(DateTime dataLeitura, decimal valor, Guid talhaoId, Guid reqApiId)
    {
        if (talhaoId == Guid.Empty)
            throw new DomainException("O dado temporal deve estar associado a um talhão válido.");
 
        if (reqApiId == Guid.Empty)
            throw new DomainException("O dado temporal deve estar associado a uma requisição de API válida.");
 
        DataLeitura = dataLeitura;
        Valor       = valor;
        TalhaoId    = talhaoId;
        ReqApiId    = reqApiId;
    }
}