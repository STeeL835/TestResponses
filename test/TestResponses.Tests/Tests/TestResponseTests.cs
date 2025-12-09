using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestResponses.Tests.Utilities;
using TestResponses.Text;

namespace TestResponses.Tests.Tests;

public class TestResponseTests
{
    [Fact]
    public async Task Read_ReadTwice_ShouldNotReadAgain()
    {
        var httpResponse = await TestHttpClient.ReceiveResponse(r =>
            r.Respond(MediaTypeNames.Application.Octet, new MemoryStream()));

        var testResponse = new NonIdenpotentReadResponse(httpResponse);
        
        await testResponse.Read();
        await testResponse.Read();
        await testResponse.Read();
    }
    
    [Fact]
    public async Task AssertionException_ContainsToString()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var httpResponse = await TestHttpClient.ReceiveResponse(r =>
            r.Respond(MediaTypeNames.Text.Plain, text));
        
        var testResponse = new TestTextResponse(httpResponse);
        await testResponse.Read();
        
        var action = () => testResponse.AssertStatusCode(500);

        action.Should().Throw<TestResponseAssertionException>()
            .WithMessage($"*{testResponse}");
    }
}