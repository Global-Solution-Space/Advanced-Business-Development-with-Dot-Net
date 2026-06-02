using System.Text;
using System.Text.Json;

namespace TerraNova.Integration.SatVeg;

public class SatVegClient(HttpClient httpClient)
{
    public async Task<SatVegDataResponse?> GetSeriesAsync(string token, SatVegDataRequest request)
    {
        var jsonRequest = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
        
        var response = await httpClient.PostAsync("/satveg/v2/series", content);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SatVegDataResponse>(jsonResponse);
    }
}