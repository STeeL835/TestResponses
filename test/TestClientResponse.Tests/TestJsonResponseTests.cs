using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.Json;
using TestClientResponse.Tests.Dto;
using TestClientResponse.Tests.Utilities;

namespace TestClientResponse.Tests;

public class TestJsonResponseTests
{
    [Theory]
    [InlineData("<html><head><title>Service Not Started</title></head></html>")]
    [InlineData("[1, 2, 3]")]
    public async Task Read_ResponseIsNotDeserialized_NotJson_ShouldShowResponseAsString(string json)
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
    
    [Fact]
    public async Task AsDto_ResponseNotDeserialized_ShouldThrowException()
    {
        const string json = """[{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }]"""; // array

        var httpResponse = await Receive(json);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();
        
        var action = () => testResponse.AsDto;

        testResponse.IsRead.Should().BeTrue();
        testResponse.IsDtoReadSuccessfully.Should().BeFalse();
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
    
    #region Assertion exception

    [Fact]
    public async Task AssertionException_ResponseIsRead_ShouldHaveStatusAndResponse()
    {
        const string json = """{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }""";

        var httpResponse = await Receive(json);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();
        
        var action = () => testResponse.ShouldHaveStatusCode(500);

        action.Should().Throw<TestJsonResponseAssertionException>()
            .WithMessage("""
                *Status code: 200 (OK)
                Response:
                {
                  "City": "Saratov",
                  "Date": "2025-09-11",
                  "TemperatureC": 6
                }
                """);
    }
    
    [Fact]
    public async Task AssertionException_ResponseIsNotRead_ShouldHaveStatusAndResponse()
    {
        const string json = """{ "City": "Saratov", "Date": "2025-09-11", "TemperatureC": 6 }""";

        var httpResponse = await Receive(json);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        
        var action = () => testResponse.ShouldHaveStatusCode(500);

        action.Should().Throw<TestJsonResponseAssertionException>()
            .WithMessage("""
                *Status code: 200 (OK)
                Response:
                *not read*
                """);
    }
    
    [Fact]
    public async Task AssertionException_ResponseIsNotDeserialized_IncorrectType_ShouldShowResponseAsIndentedJson()
    {
        const string json = """{ "Id": 12, "Name": "John Doe" }""";

        var httpResponse = await Receive(json);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();
        
        var action = () => testResponse.ShouldHaveStatusCode(500);

        action.Should().Throw<TestJsonResponseAssertionException>()
            .WithMessage("""
                *Status code: 200 (OK)
                Response:
                {
                  "Id": 12,
                  "Name": "John Doe"
                }
                """);
    }
    
    [Fact]
    public async Task AssertionException_ResponseIsNotDeserialized_NotJson_ShouldShowResponseAsString()
    {
        const string json = "<html><head><title>Service Not Started</title></head></html>";

        var httpResponse = await Receive(json);
        
        var testResponse = new TestJsonResponse<Weather>(httpResponse);
        await testResponse.Read();
        
        var action = () => testResponse.ShouldHaveStatusCode(500);

        action.Should().Throw<TestJsonResponseAssertionException>()
            .WithMessage("""
                *Status code: 200 (OK)
                Response:
                <html><head><title>Service Not Started</title></head></html>
                """);
    }

    #endregion

    
    
    private Task<HttpResponseMessage> Receive(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return TestHttpClient.ReceiveResponse(r =>
            r.Respond(statusCode, MediaTypeNames.Application.Json, content));
    }
    
}
