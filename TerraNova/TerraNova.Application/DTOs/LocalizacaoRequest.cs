using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

using TerraNova.Application.DTOs.Validators;

namespace TerraNova.Application.DTOs;
 
/// <summary>Coordenadas geográficas usadas para cadastrar uma localização no Brasil.</summary>
/// <param name="Latitude">Latitude em graus decimais.</param>
/// <param name="Longitude">Longitude em graus decimais.</param>
[BrasilCoordenadas]
public record LocalizacaoRequest(
    [Range(-90.0, 90.0,   ErrorMessage = "Latitude deve estar entre -90 e 90.")]   
    decimal Latitude,
    [Range(-180.0, 180.0, ErrorMessage = "Longitude deve estar entre -180 e 180.")] 
    decimal Longitude
) 
{
    public Localizacao ToDomain() 
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var point = geometryFactory.CreatePoint(new Coordinate((double)Longitude, (double)Latitude));
        return new Localizacao(point);
    }
}
