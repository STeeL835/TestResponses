using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RichardSzalay.MockHttp;
using TestResponses.Features;
using TestResponses.Json;
using TestResponses.Tests.Dto;
using TestResponses.Tests.Utilities;

namespace TestResponses.Tests.Tests.Types;

public class TestJsonResponseTests
{
    [Theory]
    [InlineData("<html><head><title>Service Not Started</title></head></html>")]
    [InlineData("[1, 2, 3]")]
    public async Task Read_ResponseIsNotDeserialized_NotJson_ShouldNotThrow(string json)
    {
        var httpResponse = await Receive(json);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        var reading = () => testResponse.Read();

        await reading.Should().NotThrowAsync("test could check for 500 response for example, and it can be not even json");
    }
    
    #region AsDto

    [Fact]
    public async Task AsDto_ResponseIsRead_Object_ShouldReadObjectResponse()
    {
        const string json = """{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }""";
        var weather = new Weather("Saratov", DateOnly.Parse("2025-09-11"), 6);

        var httpResponse = await Receive(json);

        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsText.Should().Be(json);
        testResponse.AsDto.Should().BeEquivalentTo(weather);
    }

    [Fact]
    public async Task AsDto_ResponseIsRead_String_ShouldReadStringResponse()
    {
        const string text = "Lorem ipsum dolor sit amet";
        const string json = $"""
            "{text}"
            """;

        var httpResponse = await Receive(json);

        var testResponse = new TestJsonResponse<string>(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsText.Should().Be(json);
        testResponse.AsDto.Should().BeEquivalentTo(text);
    }

    [Fact]
    public async Task AsDto_ResponseIsRead_Null_ShouldReadDtoAsNull()
    {
        const string json = "null";

        var httpResponse = await Receive(json);

        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsText.Should().Be(json);
        testResponse.AsDto.Should().BeNull();
    }
    
    [Fact]
    public async Task AsDto_ResponseIsRead_JsonWithComments_ShouldReadObjectResponse()
    {
        const string json = """
            // this is a comment
            { "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }
            """;
        var weather = new Weather("Saratov", DateOnly.Parse("2025-09-11"), 6);

        var httpResponse = await Receive(json);

        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsText.Should().Be(json);
        testResponse.AsDto.Should().BeEquivalentTo(weather);
    }

    [Fact]
    public async Task AsDto_ResponseNotRead_ShouldThrowException()
    {
        const string json = """{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }""";

        var httpResponse = await Receive(json);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        var action = () => testResponse.AsDto;

        testResponse.IsRead.Should().BeFalse();
        action.Should().Throw<TestResponseException>();
    }
    
    [Theory]
    [InlineData("""[{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }]""")] // array
    [InlineData("""{ "Town": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }""")] // unknown property
    [InlineData("""{ "City": "Saratov", "TemperatureC": 6 }""")] // missing non-null property
    public async Task AsDto_ResponseNotDeserialized_ShouldThrowException(string json)
    {
        var httpResponse = await Receive(json);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();
        
        var action = () => testResponse.AsDto;

        testResponse.IsRead.Should().BeTrue();
        action.Should().Throw<TestResponseAssertionException>();
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task AsDto_BadStatusCode_ShouldStillReadResponse(HttpStatusCode statusCode)
    {
        const string json = """{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }""";
        var weather = new Weather("Saratov", DateOnly.Parse("2025-09-11"), 6);

        var httpResponse = await Receive(json, statusCode);

        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();

        testResponse.IsRead.Should().BeTrue();
        testResponse.StatusCode.Should().Be(statusCode);
        testResponse.AsText.Should().Be(json);
        testResponse.AsDto.Should().Be(weather);
    }

    #endregion

    #region As<T>

    [Fact]
    public async Task AsDto_ResponseIsRead_ButWrongType_ShouldDeserializeIntoNeededType()
    {
        const string json = """{ "type": "NotFound", "title": "Not found", "status": 404, "detail": "City 'Pokrovsk' not found, maybe you meant 'Engels'" }""";
        var expectedDetails = new ProblemDetails()
        {
            Type = "NotFound",
            Status = 404,
            Title = "Not found",
            Detail = "City 'Pokrovsk' not found, maybe you meant 'Engels'",
        };

        var httpResponse = await Receive(json, HttpStatusCode.NotFound);

        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsText.Should().Be(json);

        var explicitResponse = testResponse.As<ProblemDetails>();
        explicitResponse.Should().BeEquivalentTo(expectedDetails);
    }

    #endregion
    
    #region ToString

    [Fact]
    public async Task ToString_ResponseIsNotRead_ShouldShowResponseAsNotRead()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var httpResponse = await Receive(text);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        
        testResponse.ToString().Should().Be("""
            Status code: 200 (OK)
            Response:
            *not read*
            """);
    }

    [Fact]
    public async Task ToString_ResponseIsRead_Empty_ShouldShowResponseAsEmpty()
    {
        var text = "";
        
        var httpResponse = await Receive(text);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();
        
        testResponse.ToString().Should().Be("""
            Status code: 200 (OK)
            Response:
            *empty*
            """);
    }
    
    [Fact]
    public async Task ToString_ResponseIsRead_NotJson_ShouldShowResponseAsEmpty()
    {
        const string html = "<html><head><title>Service Not Started</title></head></html>";
        
        var httpResponse = await Receive(html);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();
        
        testResponse.ToString().Should().Be("""
            Status code: 200 (OK)
            Response:
            <html><head><title>Service Not Started</title></head></html>
            """);
    }
    
    [Fact]
    public async Task ToString_ResponseIsRead_WrongJsonModel_ShouldShowResponseAsEmpty()
    {
        const string json = """{ "Id": 12, "Name": "John Doe" }""";
        
        var httpResponse = await Receive(json);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();
        
        testResponse.ToString().Should().Be("""
            Status code: 200 (OK)
            Response:
            {
              "Id": 12,
              "Name": "John Doe"
            }
            """);
    }
    
    [Fact]
    public async Task ToString_ResponseIsRead_CorrectJson_ShouldShowResponseAsEmpty()
    {
        const string json = """{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }""";
        
        var httpResponse = await Receive(json);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();
        
        testResponse.ToString().Should().Be("""
            Status code: 200 (OK)
            Response:
            {
              "City": "Saratov",
              "Date": "2025-09-11",
              "TemperatureC": 6
            }
            """);
    }

    #endregion
    
    #region Assertion exception
    
    [Fact]
    public async Task AssertionException_ContainsToString()
    {
        var json = """{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }""";
        
        var httpResponse = await Receive(json);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();
        
        var action = () => testResponse.AssertStatusCode(500);

        action.Should().Throw<TestResponseAssertionException>()
            .WithMessage($"*{testResponse}");
    }

    #endregion
    
    
    
    private Task<HttpResponseMessage> Receive(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return TestHttpClient.ReceiveResponse(r =>
            r.Respond(statusCode, MediaTypeNames.Application.Json, content));
    }
}
