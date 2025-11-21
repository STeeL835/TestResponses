using TestClientResponse.Json;

namespace TestClientResponse.Example.IntegrationTests;

[Collection(nameof(AppContextCollection))]
public class WeatherApiTestsTestResponse(TestFixture fixture)
{
    private WeatherApi _weatherApi = new(fixture.HttpClient);

    
    [Fact] // Basic scenario, pretty simple
    public async Task GetTodayWeather_CityExists_ShouldReturnForecast()
    {
        var response = await _weatherApi.GetTodayWeather("Saratov");
        
        response.AsDto!.City.Should().Be("Saratov");
    }
    
    [Fact] // Or straight to dto
    public async Task GetTodayWeather_CityExists_ShouldReturnForecast2()
    {
        var weather = await _weatherApi.GetTodayWeather("Saratov").ShouldSucceed();
        
        weather!.City.Should().Be("Saratov");
    }
    
    [Fact] // If code changes break the app, TestResponse will show response details in error message
    public async Task GetTodayWeather_CityExists_ShouldReturnForecast_ButActuallyDoesNot()
    {
        var response = await _weatherApi.GetTodayWeather("Engels");
        
        response.AsDto!.City.Should().Be("Engels");
    }
    
    [Fact] // We can see that the problem is not the city but the date
    public async Task GetTodayWeather_CityExists_ShouldReturnForecast_ButActuallyDoesNot2()
    {
        var response = await _weatherApi.GetTodayWeather("Balakovo");
        
        response.AsDto!.City.Should().Be("Balakovo");
    }
    
    [Fact] // If a non-happy path needs to be tested, TestResponse won't stand in the way
    public async Task GetTodayWeather_CityDoesNotExist_ShouldReturn404()
    {
        var response = await _weatherApi.GetTodayWeather("Pokrovsk");
        
        response.AssertStatusCode(404);
    }
    
    [Fact] // But actually will help to catch inconsistencies here too, with details in error message
    public async Task GetTodayWeather_CityDoesNotExist_ShouldReturn404_ButActuallyDoesNot()
    {
        var response = await _weatherApi.GetTodayWeather("Kuybyshev");
        
        response.AssertStatusCode(404);
    }
}