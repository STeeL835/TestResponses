using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.String;
using TestClientResponse.Tests.Utilities;

namespace TestClientResponse.Tests;

public class TestJsonResponseTests
{
    public record Weather(string City, DateOnly Date, int TemperatureC);
    
    [Fact]
    public async Task AsDto_ResponseIsRead_ShouldReadStringResponse()
    {
        const string json = """{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }""";
        var weather = new Weather("Saratov", DateOnly.Parse("2025-09-11"), 6);

        var httpResponse = await TestHttpClient.ReceiveResponse(r =>
            r.Respond(MediaTypeNames.Application.Json, json));

        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsString.Should().Be(json);
        testResponse.AsDto.Should().BeEquivalentTo(weather);
    }

    [Fact]
    public async Task AsDto_ResponseNotRead_ShouldThrowException()
    {
        const string json = """{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }""";
        
        var httpResponse = await TestHttpClient.ReceiveResponse(r =>
            r.Respond(MediaTypeNames.Application.Json, json));

        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        var action = () => testResponse.AsDto;

        testResponse.IsRead.Should().BeFalse();
        action.Should().Throw<TestResponseException>();
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task AsString_BadStatusCode_ShouldStillReadResponse(HttpStatusCode statusCode)
    {
        const string json = """{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }""";
        var weather = new Weather("Saratov", DateOnly.Parse("2025-09-11"), 6);

        var httpResponse = await TestHttpClient.ReceiveResponse(r =>
            r.Respond(statusCode, MediaTypeNames.Application.Json, json));

        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();

        testResponse.IsRead.Should().BeTrue();
        testResponse.StatusCode.Should().Be(statusCode);
        testResponse.AsString.Should().Be(json);
        testResponse.AsDto.Should().Be(weather);
    }
}