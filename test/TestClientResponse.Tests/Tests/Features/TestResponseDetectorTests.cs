using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.Empty;
using TestClientResponse.Json;
using TestClientResponse.Tests.Utilities;
using TestClientResponse.Text;
using TestClientResponse.Unknown;

namespace TestClientResponse.Tests.Tests.Features;

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
}