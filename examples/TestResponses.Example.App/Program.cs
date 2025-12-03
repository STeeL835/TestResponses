var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var weatherData = new Dictionary<string, Dictionary<DateOnly, int>>()
{
    ["Saratov"] = new() { [DateOnly.FromDateTime(DateTime.Today)] = 15 },
    // ["Engels"] = new() { [DateOnly.FromDateTime(DateTime.Today)] = 15 }, // previously Pokrovsk but renamed, should exist, but was lost in refactoring
    ["Balakovo"] = new() { [DateOnly.FromDateTime(DateTime.Today.AddDays(1))] = 16 },
    ["Kuybyshev"] = new() { [DateOnly.FromDateTime(DateTime.Today)] = 13 }, // now it's Samara, but was not renamed in the project, test should catch that
};

app.MapGet("forecasts", (string city, string date) =>
{
    if (!DateOnly.TryParse(date, out var parsedDate))
    {
        if (date == "today") parsedDate = DateOnly.FromDateTime(DateTime.Today);
        else return Results.Problem(title: "Validation error has occured", statusCode: 400, detail: $"Date '{date}' is invalid, expected a date or 'today'");
    }

    if (!weatherData.TryGetValue(city, out var cityData))
        return Results.Problem(title: "Entity not found", statusCode: 404, detail: $"City '{city}' was not found");

    if (!cityData.TryGetValue(parsedDate, out var temp))
        return Results.Problem(title: "Entity not found", statusCode: 404, detail: $"Date '{date}' was not found");
    
    return Results.Ok(new WeatherForecast(city, parsedDate, temp));
});

app.MapPut("forecasts", (WeatherForecast forecast) =>
{
    if (!weatherData.TryGetValue(forecast.City, out var cityData))
        return Results.Problem(title: "Entity not found", statusCode: 404, detail: $"City '{forecast.City}' was not found");

    cityData[forecast.Date] = forecast.TemperatureC;
    
    return Results.NoContent();
});

app.MapGet("/cities", () => weatherData.Keys.ToList());
app.MapPost("/cities", (string city) =>
{
    if (weatherData.ContainsKey(city)) return Results.Ok();
    weatherData.Add(city, new Dictionary<DateOnly, int>());
    return Results.Created();
});

app.Run();


public record WeatherForecast(string City, DateOnly Date, int TemperatureC)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
