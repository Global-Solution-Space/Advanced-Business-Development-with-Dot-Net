using System.Text.Json;

namespace TerraNova.Integration.Nasa;

public class NasaPowerClient(HttpClient httpClient)
{
    public async Task<NasaPowerDataResponse?> GetDailyDataAsync(string start, string end, decimal latitude, decimal longitude)
    {
        var url = $"/api/temporal/daily/point?start={start}&end={end}&latitude={latitude}&longitude={longitude}&community=ag&parameters=PRECTOTCORR&format=JSON&units=metric&user=terranova&header=true&time-standard=UTC";
        
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<NasaPowerDataResponse>(content);
    }
}