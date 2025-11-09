using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.String;
using TestClientResponse.Tests.Utilities;

namespace TestClientResponse.Tests;

public class TestResponseConvertersTests
{
    [Fact]
    public async Task ReadAs_ShouldReturnReadTestResponse()
    {
        var getResponse = () => TestHttpClient.ReceiveResponse(r => r.Respond(MediaTypeNames.Text.Plain, "text"));

        var testResponse = await getResponse().ReadAs<TestStringResponse>();

        testResponse.IsRead.Should().BeTrue();
    }
    
    record AdvancedStringResponse(HttpResponseMessage HttpMessage, string Text) : TestStringResponse(HttpMessage);
    [Fact]
    public async Task ReadAs_DoesNotHaveCorrectCtor_ShouldThrowException()
    {
        var getResponse = () => TestHttpClient.ReceiveResponse(r => r.Respond(MediaTypeNames.Text.Plain, "text"));

        var action = () => getResponse().ReadAs<AdvancedStringResponse>();

        await action.Should().ThrowAsync<TestResponseException>();
    }
}