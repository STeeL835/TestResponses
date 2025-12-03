using TestResponses.Empty;
using TestResponses.Json;

namespace TestResponses.Example.IntegrationTests;

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
        var forecast = await _weatherApi.GetTodayWeather("Balakovo").ShouldSucceed();
        
        forecast!.City.Should().Be("Balakovo");
    }
    
    [Fact] // If a non-happy path needs to be tested, TestResponse won't throw because of a 404 or different response
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
    
           
    [Fact] // TestResponse can make subsequent API calls easy and asserted
    public async Task PutForecast_CityDoesNotExist_ShouldReturn404_ButActuallyDoesNot()
    {
        var forecast = new WeatherForecast("Engels", DateOnly.Parse("2025-11-22"), 5);
        
        await _weatherApi.PutForecast(forecast).ShouldSucceed(200..299);
        
        var receivedForecast = await _weatherApi.GetWeather("Balakovo", forecast.Date);
        
        receivedForecast.Should().BeEquivalentTo(forecast);
    }
    
    [Fact] // or, directly
    public async Task PutForecast_CityDoesNotExist_ShouldReturn404_ButActuallyDoesNot2()
    {
        var forecast = new WeatherForecast("Engels", DateOnly.Parse("2025-11-22"), 5);

        var response = await _weatherApi.PutForecast(forecast);
        response.AssertValid(200..299);
        
        var receivedForecast = await _weatherApi.GetWeather("Balakovo", forecast.Date);
        
        receivedForecast.Should().BeEquivalentTo(forecast);
    }
    
    [Fact] // If API can return different 2xx statuses, either trust the client set expected statuscode
    public async Task AddCity_CityDoesNotExist_ShouldAddACityWith201()
    {
        await _weatherApi.AddCity("Balashov").ShouldSucceed();
        
        var cities = await _weatherApi.GetCities().ShouldSucceed();
        cities.Should().Contain("Balashov");
    }
    
    [Fact] // No, but really, set in client once, reused by default everywhere else
    public async Task AddCity_CityDoesNotExist_ShouldAddACityWith201_ButActuallyDoesNot()
    {
        await _weatherApi.AddCity("Kuybyshev").ShouldSucceed();
        
        var cities = await _weatherApi.GetCities().ShouldSucceed();
        cities.Should().Contain("Kuybyshev");
    }
    
    [Fact] // But it can be overriden with needed
    public async Task AddCity_CityDoesExist_ShouldIdempotentlySucceedWith200()
    {
        await _weatherApi.AddCity("Saratov").ShouldSucceed(200);
        
        var cities = await _weatherApi.GetCities().ShouldSucceed();
        cities.Should().Contain("Saratov");
    }
}