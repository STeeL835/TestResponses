using TestResponses.Example.App;
using TestResponses.Features;
using TestResponses.Json;

namespace TestResponses.Example.IntegrationTests;

public class WeatherApi(HttpClient httpClient)
{
    public Task<TestJsonResponse<WeatherForecast>> GetTodayWeather(string city)
    {
        return httpClient
            .GetAsync($"/weather/{city}/today")
            .ReadAs<TestJsonResponse<WeatherForecast>>();
    }
}