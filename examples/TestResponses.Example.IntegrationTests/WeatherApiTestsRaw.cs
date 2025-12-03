namespace TestResponses.Example.IntegrationTests;

[Collection(nameof(AppContextCollection))]
public class WeatherApiTestsRaw(TestFixture fixture)
{
    private HttpClient _httpClient = fixture.HttpClient;

    [Fact] // It's not that much code actually
    public async Task GetTodayWeather_CityExists_ShouldReturnForecast()
    {
        var httpResponse = await _httpClient.GetAsync($"forecasts?city=Saratov&date=today");
        var dto = await httpResponse.Content.ReadFromJsonAsync<WeatherForecast>();
        
        dto!.City.Should().Be("Saratov");
    }
    
    [Fact] // But when the app breaks, test does not tell you what's wrong
    public async Task GetTodayWeather_CityExists_ShouldReturnForecast_ButActuallyDoesNot()
    {
        var httpResponse = await _httpClient.GetAsync($"forecasts?city=Samara&date=today");
        var dto = await httpResponse.Content.ReadFromJsonAsync<WeatherForecast>();
        
        dto!.City.Should().Be("Samara");
        // "Expected dto.City to be "Samara", but found <null>", but why
    }
    
    [Fact] // We can check the status code, and see we got 404, but is it because city is missing or forecast on the date?
    public async Task GetTodayWeather_CityExists_ShouldReturnForecast_ButActuallyDoesNot2()
    {
        var httpResponse = await _httpClient.GetAsync($"forecasts?city=Balakovo&date=today");
        httpResponse.EnsureSuccessStatusCode();
        var dto = await httpResponse.Content.ReadFromJsonAsync<WeatherForecast>();
        
        dto!.City.Should().Be("Balakovo");
    }
}