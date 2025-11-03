using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.String;
using TestClientResponse.Tests.Utilities;

namespace TestClientResponse.Tests;

public class TestStringResponseTests
{
    [Fact]
    public async Task AsString_ResponseIsRead_ShouldReadStringResponse()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var httpResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(MediaTypeNames.Text.Plain, text));
        
        var testResponse = new TestStringResponse(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsString.Should().Be(text);
    }
    
    [Fact]
    public async Task AsString_ResponseNotRead_ShouldThrowException()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var httpResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(MediaTypeNames.Text.Plain, text));
        
        var testResponse = new TestStringResponse(httpResponse);
        var action = () => testResponse.AsString;

        testResponse.IsRead.Should().BeFalse();
        action.Should().Throw<TestResponseException>();
    }
    
    [Fact]
    public async Task Extension_ShouldReadResponse()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsString.Should().Be(text);
    }
}