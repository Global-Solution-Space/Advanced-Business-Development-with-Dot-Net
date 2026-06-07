using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;
using NetTopologySuite.Geometries;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Coordenadas geográficas. Compartilhada por Propriedade e Talhao (1:1 cada).
/// </summary>
public sealed class Localizacao : BaseEntity
{
    public Point Coordenadas { get; private set; } = default!;
 
    // Navegações inversas — apenas uma das duas estará populada
    public Propriedade? Propriedade { get; private set; }
    public Talhao?      Talhao      { get; private set; }
 
    private Localizacao() { }
 
    public Localizacao(Point coordenadas)
    {
        Validar(coordenadas);
        Coordenadas = coordenadas;
    }
 
    private static void Validar(Point coord)
    {
        if (coord == null)
            throw new DomainException("Coordenadas não podem ser nulas.");
        if (coord.Y < -90 || coord.Y > 90)
            throw new DomainException("Latitude deve estar entre -90 e 90.");
        if (coord.X < -180 || coord.X > 180)
            throw new DomainException("Longitude deve estar entre -180 e 180.");
    }
}
