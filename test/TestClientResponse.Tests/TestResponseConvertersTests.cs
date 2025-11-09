using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.Tests.Utilities;
using TestClientResponse.Text;

namespace TestClientResponse.Tests;

public class TestResponseConvertersTests
{
    [Fact]
    public async Task ReadAs_ShouldReturnReadTestResponse()
    {
        var getResponse = () => TestHttpClient.ReceiveResponse(r => r.Respond(MediaTypeNames.Text.Plain, "text"));

        var testResponse = await getResponse().ReadAs<TestTextResponse>();

        testResponse.IsRead.Should().BeTrue();
    }
    
    record AdvancedTextResponse(HttpResponseMessage HttpMessage, string Text) : TestTextResponse(HttpMessage);
    [Fact]
    public async Task ReadAs_DoesNotHaveCorrectCtor_ShouldThrowException()
    {
        var getResponse = () => TestHttpClient.ReceiveResponse(r => r.Respond(MediaTypeNames.Text.Plain, "text"));

        var action = () => getResponse().ReadAs<AdvancedTextResponse>();

        await action.Should().ThrowAsync<TestResponseException>();
    }
}