using System.Text.Json.Serialization;

namespace TerraNova.Integration.Nasa;

public record NasaPowerDataResponse(
    [property: JsonPropertyName("geometry")] NasaPowerGeometry Geometry,
    [property: JsonPropertyName("properties")] NasaPowerProperties Properties
);

public record NasaPowerGeometry(List<double> coordinates);

public record NasaPowerProperties(
    [property: JsonPropertyName("parameter")] Dictionary<string, Dictionary<string, double?>> Parameter
);