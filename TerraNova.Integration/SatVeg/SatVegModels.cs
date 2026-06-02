using System.Text.Json.Serialization;

namespace TerraNova.Integration.SatVeg;

public record SatVegDataRequest(
    [property: JsonPropertyName("tipoPerfil")] string TipoPerfil,
    [property: JsonPropertyName("satelite")] string Satelite,
    [property: JsonPropertyName("preFiltro")] int PreFiltro,
    [property: JsonPropertyName("filtro")] string Filtro,
    [property: JsonPropertyName("parametroFiltro")] int ParametroFiltro,
    [property: JsonPropertyName("longitude")] decimal Longitude,
    [property: JsonPropertyName("latitude")] decimal Latitude
);

public record SatVegDataResponse(
    [property: JsonPropertyName("listaSerie")] List<double> ListaSerie,
    [property: JsonPropertyName("listaDatas")] List<string> ListaDatas
);