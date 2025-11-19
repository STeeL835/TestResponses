namespace TestClientResponse.Example.IntegrationTests;

public class WeatherApiTests : IClassFixture<TestFixture>
{
    private WeatherApi _weatherApi;

    public WeatherApiTests(TestFixture fixture)
    {
        _weatherApi = new WeatherApi(fixture.Client);
    }
    
    [Fact]
    public async Task GetTodayWeather_CityExists_ShouldReturnForecast()
    {
        var response = await _weatherApi.GetTodayWeather("Saratov");
        
        response.AsDto!.City.Should().Be("Saratov");
    }
    
    [Fact]
    public async Task GetTodayWeather_CityDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _weatherApi.GetTodayWeather("Samara");
        
        response.AsDto!.City.Should().Be("Samara");
    }
}