using TerraNova.API.Exceptions;
using TerraNova.API.Extensions;
using TerraNova.API.Swagger;
using Microsoft.OpenApi;

namespace TerraNova.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddTerraNovaDbContext(builder.Configuration);
        builder.Services.AddTerraNovaRepositories();
        builder.Services.AddTerraNovaApplicationServices();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title       = "TerraNova API",
                Version     = "v1",
                Description = "API REST para monitoramento agrícola com dados SatVeg e NASA POWER.",
                Contact     = new OpenApiContact
                {
                    Name  = "Equipe TerraNova",
                    Email = "contato@terranova.com"
                }
            });

            var apiXml     = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXml);
            if (File.Exists(apiXmlPath))
                options.IncludeXmlComments(apiXmlPath);

            var appXml     = "TerraNova.Application.xml";
            var appXmlPath = Path.Combine(AppContext.BaseDirectory, appXml);
            if (File.Exists(appXmlPath))
                options.IncludeXmlComments(appXmlPath);

            options.DocumentFilter<SwaggerTagDescriptionsDocumentFilter>();
        });

        var app = builder.Build();

        app.UseExceptionHandler();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "TerraNova API v1");
            options.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}