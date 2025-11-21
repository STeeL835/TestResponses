using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.Tests.Utilities;
using TestClientResponse.Text;

namespace TestClientResponse.Tests.Tests.Features;

public class AssertStatusCodeTests
{
    // TestTextResponse is used for tests, but this method is available in all TestResponse inheritors 
    
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Created)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData((HttpStatusCode)299)]
    public async Task Range_StatusCodeMatches_ShouldNotThrow(HttpStatusCode statusCode)
    {
        var testResponse = await GetReadTestResponseWithCode(statusCode);
        
        var action = () => testResponse.AssertStatusCode(200..299);

        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task Range_StatusCodeDoesNotMatch_ShouldThrow(HttpStatusCode statusCode)
    {
        var testResponse = await GetReadTestResponseWithCode(statusCode);
        
        var action = () => testResponse.AssertStatusCode(400..499);
        
        action.Should().Throw<TestTextResponseAssertionException>()
            .WithMessage("Response status code is not in range 400..499*");
    }

    
    [Fact]
    public async Task Enum_StatusCodeMatches_ShouldNotThrow()
    {
        var testResponse = await GetReadTestResponseWithCode(HttpStatusCode.NotFound);
        
        var action = () => testResponse.AssertStatusCode(HttpStatusCode.NotFound);

        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(HttpStatusCode.OK)] // 200
    [InlineData(HttpStatusCode.InternalServerError)] // 500
    [InlineData(HttpStatusCode.BadRequest)] // 400
    public async Task Enum_StatusCodeDoesNotMatch_ShouldThrow(HttpStatusCode statusCode)
    {
        var testResponse = await GetReadTestResponseWithCode(statusCode);
        
        var action = () => testResponse.AssertStatusCode(HttpStatusCode.MethodNotAllowed); // 405
        
        action.Should().Throw<TestTextResponseAssertionException>()
            .WithMessage("Response status code is not 405*");
    }

    [Fact]
    public async Task Int_StatusCodeMatches_ShouldNotThrow()
    {
        var testResponse = await GetReadTestResponseWithCode(HttpStatusCode.Unauthorized);
        
        var action = () => testResponse.AssertStatusCode(401);

        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(HttpStatusCode.OK)] // 200
    [InlineData(HttpStatusCode.InternalServerError)] // 500
    [InlineData(HttpStatusCode.BadRequest)] // 400
    public async Task Int_StatusCodeDoesNotMatch_ShouldThrow(HttpStatusCode statusCode)
    {
        var testResponse = await GetReadTestResponseWithCode(statusCode);
        
        var action = () => testResponse.AssertStatusCode(429);
        
        action.Should().Throw<TestTextResponseAssertionException>()
            .WithMessage("Response status code is not 429*");
    }
    
    
    private async Task<TestTextResponse> GetReadTestResponseWithCode(HttpStatusCode statusCode)
    {
        var httpResponse = await TestHttpClient.ReceiveResponse(r =>
            r.Respond(statusCode, MediaTypeNames.Text.Plain, string.Empty));
        
        var testResponse = new TestTextResponse(httpResponse);
        
        await testResponse.Read();

        return testResponse;
    }
}
