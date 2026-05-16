TestResponses
=============

Readable, contextual API test responses for .NET



## Overview

TestResponses is a set of `HttpResponseMessage` decorators that make API integration tests more readable and more informative, while keeping everything you need easy to access.

Here’s a plain `HttpResponseMessage` example:

```csharp
var httpResponse = await _weatherApi.GetTodayWeather("Saratov");
httpResponse.EnsureSuccessStatusCode();
var dto = await httpResponse.Content.ReadFromJsonAsync<WeatherForecast>();

dto!.City.Should().Be("Saratov");
```

With TestResponses, test code becomes much simpler:

```csharp
var response = await _weatherApi.GetTodayWeather("Saratov");

response.AsDto!.City.Should().Be("Saratov");
```

- Less boilerplate
- DTO model for happy paths
- Raw HttpResponseMessage property for corner cases
- Does not fail the test right away so you can test 4xx responses with the same object, without try-catch statement
- But main value is in failure messages: Instead of a generic `HttpRequestException`, TestResponses shows response context, which makes it easier to understand the failure without debugging:

```
TestResponses.TestResponseAssertionException
Response status code is not 200
Status code: 400 (Bad Request)
Response:
{
  "type": "Validation",
  "title": "Request validation failed",
  "status": 400,
  "errors": {
    "date": ["'00000000-0000-0000-0000-000000000000' is not a valid date"]
  }
}
```

And it's not restricted to JSON responses, because TestResponses is a family of response types - from empty to JSON and files. They are also designed to be extensible and configurable, so you can create a type that fits your scenario, or modify behavior of existing one.



## Get started

### 1. Install

```sh
dotnet add package TestResponses
```

### 2. Instantiate

Wrap the request task in a suitable TestResponse type with `ReadAs`:

```csharp
public Task<TestJsonResponse<WeatherForecast>> GetTodayWeather(string city)
{
    return httpClient
        .GetAsync($"forecasts?city={city}&date=today")
        .ReadAs<TestJsonResponse<WeatherForecast>>(expectedStatusCode: 200);
}
```

### 3. Await and use

```csharp
var response = await _weatherApi.GetTodayWeather("Saratov");

var statusCode = response.StatusCode;
var rawResponse = response.HttpResponse;
var dto = response.AsDto;
```



## Documentation

For more information you can read the docs. \
[Docs →](https://github.com/STeeL835/TestResponses/wiki)

Project also has usage examples with failing tests that you can run and play around with \
[Examples →](https://github.com/STeeL835/TestResponses/tree/main/examples/TestResponses.Example.IntegrationTests/)
