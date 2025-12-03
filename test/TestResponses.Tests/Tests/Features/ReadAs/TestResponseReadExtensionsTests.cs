using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestResponses.Features;
using TestResponses.Tests.Utilities;
using TestResponses.Text;

namespace TestResponses.Tests.Tests.Features.ReadAs;

public class TestResponseReadExtensionsTests
{
    [Fact]
    public async Task ReadAs_ShouldReturnReadTestResponse()
    {
        var getResponse = () => TestHttpClient.ReceiveResponse(r => r.Respond(MediaTypeNames.Text.Plain, "text"));

        var testResponse = await getResponse().ReadAs<TestTextResponse>();

        testResponse.IsRead.Should().BeTrue();
        testResponse.ExpectedStatusCode.Should().BeNull();
    }
    
    [Fact]
    public async Task ReadAs_WithExpectedStatusCode_ShouldReturnReadTestResponse()
    {
        var getResponse = () => TestHttpClient.ReceiveResponse(r => r.Respond(MediaTypeNames.Text.Plain, "text"));

        var testResponse = await getResponse().ReadAs<TestTextResponse>(expectedStatusCode: 200);

        testResponse.IsRead.Should().BeTrue();
        testResponse.ExpectedStatusCode!.Range.Should().Be(200..200);
    }

    [Fact]
    public async Task ReadAs_DoesNotHaveCorrectCtor_ShouldThrowException()
    {
        var getResponse = () => TestHttpClient.ReceiveResponse(r => r.Respond(MediaTypeNames.Text.Plain, "text"));

        var action = () => getResponse().ReadAs<InvalidCtorResponse>();

        await action.Should().ThrowAsync<TestResponseException>();
    }
}