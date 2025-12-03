using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.Tests.Utilities;
using TestClientResponse.Text;

namespace TestClientResponse.Tests.Tests.Features;

public class ExpectedStatusCodeTests
{
    [Fact]
    public async Task AssertExpectedStatusCode_StatusCodeMatches_ShouldNotThrow()
    {
        var responseMessage = await Receive(HttpStatusCode.OK);

        var testResponse = new TestTextResponse(responseMessage)
        {
            ExpectedStatusCode = 200
        };

        await testResponse.Read();
        
        var action = () => testResponse.AssertExpectedStatusCode();

        testResponse.ExpectedStatusCode.Range.Should().Be(200..200);
        action.Should().NotThrow<TestResponseAssertionException>();
    }
    
    [Fact]
    public async Task AssertExpectedStatusCode_StatusCodeDoesNotMatch_ShouldThrow()
    {
        var responseMessage = await Receive(HttpStatusCode.OK);

        var testResponse = new TestTextResponse(responseMessage)
        {
            ExpectedStatusCode = 201
        };

        await testResponse.Read();
        
        var action = () => testResponse.AssertExpectedStatusCode();
        
        action.Should().Throw<TestResponseAssertionException>();
    }
    
    [Fact]
    public async Task AssertExpectedStatusCode_StatusCodeNotSet_ShouldThrow()
    {
        var responseMessage = await Receive(HttpStatusCode.OK);

        var testResponse = new TestTextResponse(responseMessage);

        await testResponse.Read();
        
        var action = () => testResponse.AssertExpectedStatusCode();
        
        action.Should().Throw<TestResponseException>();
    }
    
    
    
    private async Task<HttpResponseMessage> Receive(HttpStatusCode statusCode)
    {
        var httpResponse = await TestHttpClient.ReceiveResponse(r =>
            r.Respond(statusCode, MediaTypeNames.Text.Plain, string.Empty));
        
        return httpResponse;
    }
}