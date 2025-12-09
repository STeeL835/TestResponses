using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestResponses.Tests.Utilities;

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
}