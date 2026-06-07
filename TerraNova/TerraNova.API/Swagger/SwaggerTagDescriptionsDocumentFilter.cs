using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TerraNova.API.Swagger;

/// <summary>Organiza as tags exibidas na UI do Swagger.</summary>
public sealed class SwaggerTagDescriptionsDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags = new HashSet<OpenApiTag>
        {
            new OpenApiTag { Name = "Produtor", Description = "Gerenciamento de produtores rurais" },
            new OpenApiTag { Name = "Telefone", Description = "Gerenciamento de contatos telefônicos" },
            new OpenApiTag { Name = "Localização", Description = "Gerenciamento de geolocalizações" },
            new OpenApiTag { Name = "Propriedade", Description = "Gerenciamento de propriedades rurais" },
            new OpenApiTag { Name = "Talhão", Description = "Gerenciamento de talhões e suas plantações" },
            new OpenApiTag { Name = "Tipo de Plantação", Description = "Gerenciamento dos tipos de culturas" },
            new OpenApiTag { Name = "Tipo de API", Description = "Gerenciamento dos tipos de APIs externas" },
            new OpenApiTag { Name = "Integração (Req API)", Description = "Orquestrador de dados de APIs externas (NASA POWER / SatVeg)" },
            new OpenApiTag { Name = "Dado Temporal", Description = "Séries temporais unificadas de dados climáticos e vegetativos" },
            new OpenApiTag { Name = "Alerta Agrícola", Description = "Alertas baseados na análise cruzada dos dados" }
        };
    }
}
