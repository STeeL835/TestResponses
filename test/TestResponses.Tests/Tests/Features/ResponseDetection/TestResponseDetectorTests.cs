using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestResponses.Empty;
using TestResponses.Features;
using TestResponses.Json;
using TestResponses.Tests.Utilities;
using TestResponses.Text;
using TestResponses.Unknown;

namespace TestResponses.Tests.Tests.Features.ResponseDetection;

public class TestResponseDetectorTests
{
    [Fact]
    public async Task ReadAsBestFitResponse_Empty_ShouldReturnReadText()
    {
        var httpResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(HttpStatusCode.NoContent));

        var bestfit = await TestResponseDetector.ReadAsBestFitResponse(httpResponse);

        bestfit.Should().BeOfType<TestEmptyResponse>();
    }
    
    [Fact]
    public async Task ReadAsBestFitResponse_Text_ShouldReturnReadText()
    {
        var httpResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(MediaTypeNames.Text.Plain, "text"));

        var bestfit = await TestResponseDetector.ReadAsBestFitResponse(httpResponse);
        
        bestfit.Should().BeOfType<TestTextResponse>()
            .Which.AsText.Should().Be("text");
    }
    
    [Fact]
    public async Task ReadAsBestFitResponse_Json_ShouldReturnReadText()
    {
        var httpResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(MediaTypeNames.Application.Json, """[{ "key": "value" }]"""));

        var bestfit = await TestResponseDetector.ReadAsBestFitResponse(httpResponse);
        
        bestfit.Should().BeOfType<TestJsonResponse<object>>()
            .Which.AsText.Should().Be("""[{ "key": "value" }]""");
    }
    
    [Fact]
    public async Task ReadAsBestFitResponse_Unknown_ShouldReturnReadText()
    {
        var httpResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(mediaType: MediaTypeNames.Font.Ttf, content: MemoryStream.Null));

        var bestfit = await TestResponseDetector.ReadAsBestFitResponse(httpResponse);

        bestfit.Should().BeOfType<TestUnknownResponse>();
    }
    
    [Fact]
    public async Task ToString_TestEmptyResponse_ActuallyJson_ShouldReturnJsonInfo()
    {
        var httpResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(mediaType: MediaTypeNames.Application.Json, content: "{ \"a\": 3 }"));

        var testResponse = new TestEmptyResponse(httpResponse);
        await testResponse.Read();

        testResponse.ToString().Should().Contain("""
            {
              "a": 3
            }
            """);
    }
}