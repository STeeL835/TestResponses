using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.Tests.Utilities;

namespace TestClientResponse.Tests;

public class TestStringResponseTests
{
    [Fact]
    public async Task ShouldReadStringResponse()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var httpResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(MediaTypeNames.Text.Plain, text));
        
        var testResponse = new TestStringResponse(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.AsString.Should().Be(text);
    }
}