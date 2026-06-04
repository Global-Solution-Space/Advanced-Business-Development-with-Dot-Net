using TerraNova.Domain.Common;
using TerraNova.Domain.Enums;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Registro de uma requisição a uma API externa (SatVeg ou NASA POWER).
/// Os dados retornados ficam normalizados em <see cref="DadoTemporal"/>.
/// </summary>
public sealed class ReqApi : BaseEntity
{
    /// <summary>Parâmetro consultado: NVDI (SatVeg) ou PRECTOTCORR (NASA POWER).</summary>
    public TipoParamReqApi TipoParam   { get; private set; }
 
    public DateTime        DataAnalise { get; private set; }
 
    public Guid    TipoApiId { get; private set; }
    public TipoApi? TipoApi  { get; private set; }
 
    public List<DadoTemporal> DadosTemporais { get; private set; } = [];
 
    private ReqApi() { }
 
    public ReqApi(TipoParamReqApi tipoParam, Guid tipoApiId)
    {
        if (tipoApiId == Guid.Empty)
            throw new DomainException("A requisição deve estar associada a um tipo de API válido.");
 
        TipoParam   = tipoParam;
        DataAnalise = DateTime.UtcNow;
        TipoApiId   = tipoApiId;
    }
}