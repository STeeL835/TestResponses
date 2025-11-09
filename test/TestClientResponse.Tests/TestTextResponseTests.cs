using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.Tests.Utilities;
using TestClientResponse.Text;

namespace TestClientResponse.Tests;

public class TestTextResponseTests
{
    #region AsText

    [Fact]
    public async Task AsText_ResponseIsRead_ShouldReadResponseText()
    {
        var text = "Lorem ipsum dolor sit amet";

        var httpResponse = await Receive(text);
        
        var testResponse = new TestTextResponse(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsText.Should().Be(text);
    }
    
    [Fact]
    public async Task AsText_ResponseNotRead_ShouldThrowException()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var httpResponse = await Receive(text);
        
        var testResponse = new TestTextResponse(httpResponse);
        var action = () => testResponse.AsText;

        testResponse.IsRead.Should().BeFalse();
        action.Should().Throw<TestResponseException>();
    }
    
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task AsText_BadStatusCode_ShouldStillReadResponse(HttpStatusCode statusCode)
    {
        var text = "Lorem ipsum dolor sit amet";

        var httpResponse = await Receive(text, statusCode);
        
        var testResponse = new TestTextResponse(httpResponse);
        await testResponse.Read();
        
        testResponse.IsRead.Should().BeTrue();
        testResponse.StatusCode.Should().Be(statusCode);
        testResponse.AsText.Should().Be(text);
    }

    #endregion

    #region Assertion exception

    [Fact]
    public async Task AssertionException_ResponseIsRead_ShouldHaveStatusAndResponse()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var httpResponse = await Receive(text);
        
        var testResponse = new TestTextResponse(httpResponse);
        await testResponse.Read();
        
        var action = () => testResponse.ShouldHaveStatusCode(500);

        action.Should().Throw<TestTextResponseAssertionException>()
            .WithMessage("""
                *Status code: 200 (OK)
                Response: 
                Lorem ipsum dolor sit amet
                """);
    }
    
    [Fact]
    public async Task AssertionException_ResponseIsNotRead_ShouldHaveStatusAndResponse()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var httpResponse = await Receive(text);
        
        var testResponse = new TestTextResponse(httpResponse);
        
        var action = () => testResponse.ShouldHaveStatusCode(500);

        action.Should().Throw<TestTextResponseAssertionException>()
            .WithMessage("""
                *Status code: 200 (OK)
                Response: 
                *not read*
                """);
    }

    #endregion
    
    [Fact]
    public async Task ShouldSucceed_SuccessStatusCode_ShouldReturnResponseText()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var clientGetResponse = () => Receive(text).ReadAs<TestTextResponse>();

        var response = await clientGetResponse().ShouldSucceed();

        response.Should().Be(text);
    }
    
    [Fact]
    public async Task ShouldSucceed_FailStatusCode_ShouldReturnResponseText()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var clientGetResponse = () => Receive(text, HttpStatusCode.BadRequest).ReadAs<TestTextResponse>();

        var action = () => clientGetResponse().ShouldSucceed();

        await action.Should().ThrowAsync<TestTextResponseAssertionException>();
    }
    
    
    
    private Task<HttpResponseMessage> Receive(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return TestHttpClient.ReceiveResponse(r =>
            r.Respond(statusCode, MediaTypeNames.Text.Plain, content));
    }
}