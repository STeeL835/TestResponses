using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.String;
using TestClientResponse.Tests.Utilities;

namespace TestClientResponse.Tests;

public class ShouldHaveStatusCodeTests
{
    // TestStringResponse is used for tests, but this method is available in all TestResponse inheritors 
    
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Created)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData((HttpStatusCode)299)]
    public async Task Range_StatusCodeMatches_ShouldNotThrow(HttpStatusCode statusCode)
    {
        var testResponse = await GetResponseWithCode(statusCode).AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(200..299);

        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task Range_StatusCodeDoesNotMatch_ShouldThrow(HttpStatusCode statusCode)
    {
        var testResponse = await GetResponseWithCode(statusCode).AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(400..499);
        
        action.Should().Throw<TestStringResponseAssertionException>()
            .WithMessage("Response status code is not in range 400..499*");
    }

    public static TheoryData<Range> IncorrectRangeTestCases =>
    [
        (200..),
        (..299),
        (^200..299),
        (200..^299),
        (200..600),
        (99..500),
        (299..200)
    ];
    [Theory, MemberData(nameof(IncorrectRangeTestCases))]
    public async Task Range_IncorrectValues_ShouldThrow(Range statusCodeRange)
    {
        var testResponse = await GetResponseWithCode(HttpStatusCode.OK).AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(statusCodeRange);
        
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task Enum_StatusCodeMatches_ShouldNotThrow()
    {
        var testResponse = await GetResponseWithCode(HttpStatusCode.NotFound).AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(HttpStatusCode.NotFound);

        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(HttpStatusCode.OK)] // 200
    [InlineData(HttpStatusCode.InternalServerError)] // 500
    [InlineData(HttpStatusCode.BadRequest)] // 400
    public async Task Enum_StatusCodeDoesNotMatch_ShouldThrow(HttpStatusCode statusCode)
    {
        var testResponse = await GetResponseWithCode(statusCode).AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(HttpStatusCode.MethodNotAllowed); // 405
        
        action.Should().Throw<TestStringResponseAssertionException>()
            .WithMessage("Response status code is not 405*");
    }

    [Fact]
    public async Task Int_StatusCodeMatches_ShouldNotThrow()
    {
        var testResponse = await GetResponseWithCode(HttpStatusCode.Unauthorized).AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(401);

        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(HttpStatusCode.OK)] // 200
    [InlineData(HttpStatusCode.InternalServerError)] // 500
    [InlineData(HttpStatusCode.BadRequest)] // 400
    public async Task Int_StatusCodeDoesNotMatch_ShouldThrow(HttpStatusCode statusCode)
    {
        var testResponse = await GetResponseWithCode(statusCode).AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(429);
        
        action.Should().Throw<TestStringResponseAssertionException>()
            .WithMessage("Response status code is not 429*");
    }

    [Theory]
    [InlineData(-1)] 
    [InlineData(99)] 
    [InlineData(600)]
    public async Task Int_StatusCodeIsInvalid_ShouldThrow(int expectedStatusCode)
    {
        var testResponse = await GetResponseWithCode(HttpStatusCode.OK).AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(expectedStatusCode);

        action.Should().Throw<ArgumentException>();
    }

    
    
    private Task<HttpResponseMessage> GetResponseWithCode(HttpStatusCode statusCode)
    {
        return TestHttpClient.ReceiveResponse(r =>
            r.Respond(statusCode, MediaTypeNames.Text.Plain, string.Empty));
    }
}
