using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using TerraNova.Application.DTOs;

namespace TerraNova.Application.DTOs.Validators;

[AttributeUsage(AttributeTargets.Class)]
public sealed class BrasilCoordenadasAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not LocalizacaoRequest request) return ValidationResult.Success;

        var httpClientFactory = (IHttpClientFactory?)validationContext.GetService(typeof(IHttpClientFactory));
        if (httpClientFactory == null) return ValidationResult.Success;

        var client = httpClientFactory.CreateClient();
        try
        {
            var url = $"https://api.bigdatacloud.net/data/reverse-geocode-client?latitude={request.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={request.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&localityLanguage=en";
            var response = client.GetAsync(url).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                using var json = JsonDocument.Parse(content);
                if (json.RootElement.TryGetProperty("countryCode", out var countryCode))
                {
                    var code = countryCode.GetString();
                    if (!string.IsNullOrEmpty(code) && code != "BR")
                    {
                        return new ValidationResult("As coordenadas caem fora do território brasileiro (countryCode != BR).");
                    }
                }
            }
        }
        catch
        {
            // Fail-safe (aberta em caso de indisponibilidade da API externa)
        }

        return ValidationResult.Success;
    }
}
