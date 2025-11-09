using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.String;
using TestClientResponse.Tests.Utilities;

namespace TestClientResponse.Tests;

public class TestStringResponseTests
{
    #region AsString

    [Fact]
    public async Task AsString_ResponseIsRead_ShouldReadStringResponse()
    {
        var text = "Lorem ipsum dolor sit amet";

        var httpResponse = await Receive(text);
        
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
        
        var httpResponse = await Receive(text);
        
        var testResponse = new TestStringResponse(httpResponse);
        var action = () => testResponse.AsString;

        testResponse.IsRead.Should().BeFalse();
        action.Should().Throw<TestResponseException>();
    }
    
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task AsString_BadStatusCode_ShouldStillReadResponse(HttpStatusCode statusCode)
    {
        var text = "Lorem ipsum dolor sit amet";

        var httpResponse = await Receive(text, statusCode);
        
        var testResponse = new TestStringResponse(httpResponse);
        await testResponse.Read();
        
        testResponse.IsRead.Should().BeTrue();
        testResponse.StatusCode.Should().Be(statusCode);
        testResponse.AsString.Should().Be(text);
    }

    #endregion

    #region Assertion exception

    [Fact]
    public async Task AssertionException_ResponseIsRead_ShouldHaveStatusAndResponse()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var httpResponse = await Receive(text);
        
        var testResponse = new TestStringResponse(httpResponse);
        await testResponse.Read();
        
        var action = () => testResponse.ShouldHaveStatusCode(500);

        action.Should().Throw<TestStringResponseAssertionException>()
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
        
        var testResponse = new TestStringResponse(httpResponse);
        
        var action = () => testResponse.ShouldHaveStatusCode(500);

        action.Should().Throw<TestStringResponseAssertionException>()
            .WithMessage("""
                *Status code: 200 (OK)
                Response: 
                *not read*
                """);
    }

    #endregion
    
    [Fact]
    public async Task ShouldSucceed_SuccessStatusCode_ShouldReturnResponseString()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var clientGetResponse = () => Receive(text).ReadAs<TestStringResponse>();

        var response = await clientGetResponse().ShouldSucceed();

        response.Should().Be(text);
    }
    
    [Fact]
    public async Task ShouldSucceed_FailStatusCode_ShouldReturnResponseString()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var clientGetResponse = () => Receive(text, HttpStatusCode.BadRequest).ReadAs<TestStringResponse>();

        var action = () => clientGetResponse().ShouldSucceed();

        await action.Should().ThrowAsync<TestStringResponseAssertionException>();
    }
    
    
    
    private Task<HttpResponseMessage> Receive(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return TestHttpClient.ReceiveResponse(r =>
            r.Respond(statusCode, MediaTypeNames.Text.Plain, content));
    }
}