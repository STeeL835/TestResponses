using TestResponses.Empty;
using TestResponses.Features;
using TestResponses.Json;

namespace TestResponses.Example.IntegrationTests;

public class WeatherApi(HttpClient httpClient)
{
    public Task<TestJsonResponse<WeatherForecast>> GetTodayWeather(string city)
    {
        return httpClient
            .GetAsync($"forecasts?city={city}&date=today")
            .ReadAs<TestJsonResponse<WeatherForecast>>(200);
    }
    
    public Task<TestJsonResponse<WeatherForecast>> GetWeather(string city, DateOnly date)
    {
        return httpClient
            .GetAsync($"forecasts?city={city}&date={date}")
            .ReadAs<TestJsonResponse<WeatherForecast>>(200);
    }
    
    public Task<TestEmptyResponse> PutForecast(WeatherForecast forecast)
    {
        return httpClient
            .PutAsync($"/forecasts", JsonContent.Create(forecast))
            .ReadAs<TestEmptyResponse>(204);
    }
    
    public Task<TestEmptyResponse> AddCity(string city)
    {
        return httpClient
            .PostAsync($"/cities?city={city}", new StringContent(String.Empty))
            .ReadAs<TestEmptyResponse>(201);
    }
    
    public Task<TestJsonResponse<List<string>>> GetCities()
    {
        return httpClient
            .GetAsync($"/cities")
            .ReadAs<TestJsonResponse<List<string>>>(200);
    }

}