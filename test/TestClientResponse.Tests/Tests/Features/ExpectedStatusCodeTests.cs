using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.Tests.Utilities;
using TestClientResponse.Text;

namespace TestClientResponse.Tests.Tests.Features;

public class ExpectedStatusCodeTests
{
    [Theory] // range
    [MemberData(nameof(ShouldHaveStatusCodeTests.IncorrectRangeTestCases),  MemberType = typeof(ShouldHaveStatusCodeTests))]
    public async Task ExpectedStatusCodes_InvalidStatusCode_ShouldThrow(Range range)
    {
        var responseMessage = await Receive(HttpStatusCode.OK);

        var action = () => new TestTextResponse(responseMessage)
        {
            ExpectedStatusCodes = range
        };

        action.Should().Throw<ArgumentException>();
    }
    
    [Theory] // singular
    [MemberData(nameof(ShouldHaveStatusCodeTests.IncorrectStatusTestCases),  MemberType = typeof(ShouldHaveStatusCodeTests))]
    public async Task ExpectedStatusCode_InvalidStatusCode_ShouldThrow(int statusCode)
    {
        var responseMessage = await Receive(HttpStatusCode.OK);

        var action = () => new TestTextResponse(responseMessage)
        {
            ExpectedStatusCode = statusCode
        };

        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public async Task ShouldHaveExpectedStatusCode_StatusCodeMatches_ShouldNotThrow()
    {
        var responseMessage = await Receive(HttpStatusCode.OK);

        var testResponse = new TestTextResponse(responseMessage)
        {
            ExpectedStatusCode = 200
        };

        await testResponse.Read();
        
        var action = () => testResponse.ShouldHaveExpectedStatusCode();

        testResponse.ExpectedStatusCodes.Should().Be(200..200);
        action.Should().NotThrow<TestResponseAssertionException>();
    }
    
    [Fact]
    public async Task ShouldHaveExpectedStatusCode_StatusCodeDoesNotMatch_ShouldThrow()
    {
        var responseMessage = await Receive(HttpStatusCode.OK);

        var testResponse = new TestTextResponse(responseMessage)
        {
            ExpectedStatusCode = 201
        };

        await testResponse.Read();
        
        var action = () => testResponse.ShouldHaveExpectedStatusCode();
        
        action.Should().Throw<TestResponseAssertionException>();
    }
    
    [Fact]
    public async Task ShouldHaveExpectedStatusCode_StatusCodeNotSet_ShouldThrow()
    {
        var responseMessage = await Receive(HttpStatusCode.OK);

        var testResponse = new TestTextResponse(responseMessage);

        await testResponse.Read();
        
        var action = () => testResponse.ShouldHaveExpectedStatusCode();
        
        action.Should().Throw<TestResponseException>();
    }
    
    
    
    private async Task<HttpResponseMessage> Receive(HttpStatusCode statusCode)
    {
        var httpResponse = await TestHttpClient.ReceiveResponse(r =>
            r.Respond(statusCode, MediaTypeNames.Text.Plain, string.Empty));
        
        return httpResponse;
    }
}