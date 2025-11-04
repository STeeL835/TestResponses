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
    
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task AsTestStringResponse_BadStatusCode_ShouldStillReadResponse(HttpStatusCode statusCode)
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(statusCode, MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        testResponse.IsRead.Should().BeTrue();
        testResponse.StatusCode.Should().Be(statusCode);
        testResponse.AsString.Should().Be(text);
    }
    
    [Fact]
    public async Task AssertionException_ShouldHaveStatusAndResponse()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
                r.Respond(MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(500);

        action.Should().Throw<TestStringResponseAssertionException>()
            .WithMessage("""
            *Status code: 200 (OK)
            Response: 
            Lorem ipsum dolor sit amet
            """);
    }
}