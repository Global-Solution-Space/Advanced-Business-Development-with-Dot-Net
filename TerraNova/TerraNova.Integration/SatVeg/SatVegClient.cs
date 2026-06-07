using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TerraNova.Integration.SatVeg;

public class SatVegClient(HttpClient httpClient)
{
    public async Task<SatVegDataResponse?> GetSeriesAsync(string token, SatVegDataRequest request)
    {
        var jsonRequest = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
        
        var response = await httpClient.PostAsync("/satveg/v2/series", content);
        
        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            throw new InvalidOperationException("As coordenadas caem no oceano ou fora dos limites cobertos pelo SATVeg.");
        }
        
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SatVegDataResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}