var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var weatherData = new Dictionary<string, Dictionary<DateOnly, int>>()
{
    ["Saratov"] = new()
    {
        [DateOnly.FromDateTime(DateTime.Today)] = 15
    }
};

app.MapGet("/weather/{city}/today", (string city) =>
{
    var date = DateOnly.FromDateTime(DateTime.Today);

    if (!weatherData.TryGetValue(city, out var cityData))
        return Results.Problem(title: "Entity not found", statusCode: 404, detail: $"City '{city}' was not found");

    if (!cityData.TryGetValue(date, out var temp))
        return Results.Problem(title: "Entity not found", statusCode: 404, detail: $"Date '{date}' was not found");
    
    return Results.Ok(new WeatherForecast(date, temp, city));
});

app.Run();


public record WeatherForecast(DateOnly Date, int TemperatureC, string? City)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
