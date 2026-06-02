using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Coordenadas geográficas. Compartilhada por Propriedade e Talhao (1:1 cada).
/// </summary>
public sealed class Localizacao : BaseEntity
{
    public decimal Latitude  { get; private set; }
    public decimal Longitude { get; private set; }
 
    // Navegações inversas — apenas uma das duas estará populada
    public Propriedade? Propriedade { get; private set; }
    public Talhao?      Talhao      { get; private set; }
 
    private Localizacao() { }
 
    public Localizacao(decimal latitude, decimal longitude)
    {
        Validar(latitude, longitude);
        Latitude  = latitude;
        Longitude = longitude;
    }
 
    public void Atualizar(decimal latitude, decimal longitude)
    {
        Validar(latitude, longitude);
        Latitude  = latitude;
        Longitude = longitude;
    }
 
    private static void Validar(decimal lat, decimal lon)
    {
        if (lat < -90 || lat > 90)
            throw new DomainException("Latitude deve estar entre -90 e 90.");
        if (lon < -180 || lon > 180)
            throw new DomainException("Longitude deve estar entre -180 e 180.");
    }
}