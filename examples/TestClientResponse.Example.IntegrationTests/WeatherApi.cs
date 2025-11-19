using TestClientResponse.Json;

namespace TestClientResponse.Example.IntegrationTests;

public class WeatherApi(HttpClient httpClient)
{
    public Task<TestJsonResponse<WeatherForecast>> GetTodayWeather(string city)
    {
        return httpClient
            .GetAsync($"/weather/{city}/today")
            .ReadAs<TestJsonResponse<WeatherForecast>>();
    }
}