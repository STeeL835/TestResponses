TestResponse
============

A family of HttpResponseMessage decorators that make testing easier and more informative

## Overview
It turns this test fail message
```
System.Net.Http.HttpRequestException
Response status code does not indicate success: 400 (Bad Request).
```
into this
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
while allowing to write this
```csharp
var response = await _weatherApi.GetTodayWeather("Saratov");
        
response.AsDto!.City.Should().Be("Saratov");
```
instead of this
```csharp
var httpResponse = await _weatherApi.GetTodayWeather("Saratov");
httpResponse.EnsureSuccessStatusCode();
var dto = await httpResponse.Content.ReadFromJsonAsync<WeatherForecast>();
        
dto!.City.Should().Be("Saratov");
```

#### Supports several response types out-of-box:
- Empty
- Stream
  - Text
    - Json
  - File
  - _...or write your own_

#### Can also help with
- showing formatted info if response type is different\
  expected empty 204, but got 400 with error model

- asserting fail status code\
  test response doesn't fail until you assert it, or try to read value that can't be read.
  `AssertStatusCode()` will assert status code and will fail if status code is not 400 for example
    - using different notations\
      use `HttpStatusCode.BadRequest` enum, `StatusCodes.Status400BadRequest` constant, number `400` or even a range `400..499`

- setting expected status code(s) for specific api\
  (you can set expected status code)

- reading the same json response
  - as expected type (`AsDto`)
  - as unexpected type (`As<T>()`)
  - as text (`AsText`)

- making sure test fails fast if it uses several api calls\
  `AssertSucceeded([optional status code override])` right on test response Task will throw if arrangement api call fails (make sure you've set expected status code or passed it in an argument)

- accessing original HttpResponseMessage

## How to use

TODO: write docs

but there is an [examples project](./examples/TestResponses.Example.IntegrationTests) that shows how test responses can be [created](./examples/TestResponses.Example.IntegrationTests/WeatherApi.cs) and [used](./examples/TestResponses.Example.IntegrationTests/WeatherApiTestsTestResponse.cs)

## How to create a new response type

- Create a type, inherited from a test response type. You can extend functionality of existing type or create a totally new one. For latter case, since almost every response can be represented as a stream, TestStreamResponse is a recommended choice

- Implement required methods
    - ReadResponse
        - Call base.ReadResponse if your class inherits from TestResponse children, to read (and use) values on their level
        - Use ResponseValue<T> to read and hold the value - it postpones exceptions to the moment of reading value, keeping test response ready to use even if reading was not successful.
        - Use GetOrThrow() in getter to return read value (or throw exception)
        - Expose value with name `As{ValueName}` - read value is more like a form of response than an attribute of response. One class can represent the response as string, as text and as dto at the same time. However, if your property is not polymorphic (header value, some state or value from response), `As` is not necessary.
    - CanHandleContent
        - Use protected ContentType property for brevity
        - You can use full HttpResponseMessage to detect if class should be able to read the response or not. If response doesn't fit, detector will find best-fit to display informative message during assertion.
        - Btw you can register your type for best-fit detection using TestResponseTypes Register or RegisterFromAssembly methods. Type must be non-abstract and have a ctor with HttpResponseMessage as its only parameter
    - GetInfoString
        - call base method to append information or start over with TestResponseFormatter.FormatStatusCodeInfo as base information info
        - use `Info name: actually info` template
        - Format response value if possible, so it's easy to read the fail message and see where the problem is
        - Use Formatter pattern class to hide formatting logic there. Do expose pure formatting methods so that they can be reused by other response types like FormatStatusCodeInfo
        - do not forget about case when test response is not read
    - AssertResponseSchema
        - Do override this method if your response type has a schema (to test json correctness for example. Response can be a valid text, but invalid json - AssertResponseSchema is used to validate that). Implementation can be a simple value property read to a discard - if json is not valid, deserialization probably failed, so reading value will throw needed exception

- Implement extension methods
    - AssertSucceeded
        - Wrap AssertSucceededAndReturn extension in an `AssertSucceeded` extension that uses correct types and fills property accessor for user

- Register type for actual response detection
  - Call `TestResponseTypes.Global.Register()`