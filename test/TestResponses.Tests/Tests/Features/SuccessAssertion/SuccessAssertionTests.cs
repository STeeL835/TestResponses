using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestResponses.Features;
using TestResponses.Json;
using TestResponses.Tests.Dto;
using TestResponses.Tests.Utilities;
using TestResponses.Text;

namespace TestResponses.Tests.Tests.Features.SuccessAssertion;

public class SuccessAssertionTests
{
    // based on text response
    
    [Fact]
    public async Task AssertSucceededAndReturn_SuccessStatusCode_ShouldReturnResponseText()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var clientGetResponse = () => Receive(text).ReadAs<TestTextResponse>();

        var response = await clientGetResponse().AssertSucceededAndReturn(r => r.AsText);

        response.Should().Be(text);
    }
    
    [Fact]
    public async Task AssertSucceededAndReturn_InvalidContent_ShouldThrow()
    {
        var response = TestHttpClient.ReceiveResponse(r =>
            r.Respond(MediaTypeNames.Image.Webp, new MemoryStream()))
            .ReadAs<TestTextResponse>();

        var action = () => response.AssertSucceededAndReturn(r => r.AsText);

        await action.Should().ThrowAsync<TestResponseAssertionException>();
    }
    
    [Fact]
    public async Task AssertSucceededAndReturn_IncorrectSchema_ShouldThrow()
    {
        var text = "Lorem ipsum dolor sit amet";
        var json = $"\"{text}\"";
        
        var testResponseTask = Receive(json).ReadAs<TestJsonResponse<Weather>>();

        var assertion = () => testResponseTask.AssertSucceededAndReturn(r => r.AsDto);

        await assertion.Should().ThrowAsync<TestResponseAssertionException>();
    }

    
    [Fact]
    public async Task ShouldSucceed_FailStatusCode_NoStatusCodeSet_ShouldNotThrow()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var clientGetResponse = () => Receive(text, HttpStatusCode.BadRequest).ReadAs<TestTextResponse>();

        var action = () => clientGetResponse().AssertSucceededAndReturn(r => r.AsText);

        await action.Should().NotThrowAsync<TestResponseAssertionException>();
    }
    
    [Fact]
    public async Task ShouldSucceed_FailStatusCode_ExpectedStatusCodeSet_ShouldThrow()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var clientGetResponse = () => Receive(text, HttpStatusCode.BadRequest).ReadAs<TestTextResponse>(200);

        var action = () => clientGetResponse().AssertSucceededAndReturn(r => r.AsText);

        await action.Should().ThrowAsync<TestResponseAssertionException>();
    }
    
    [Fact]
    public async Task ShouldSucceed_FailStatusCode_SetByParameter_ShouldThrow()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var clientGetResponse = () => Receive(text, HttpStatusCode.BadRequest).ReadAs<TestTextResponse>();

        var action = () => clientGetResponse().AssertSucceededAndReturn(r => r.AsText, 200);

        await action.Should().ThrowAsync<TestResponseAssertionException>();
    }
    
    [Fact]
    public async Task ShouldSucceed_FailStatusCodeByExpected_SuccessStatusCodeByParameter_ShouldNotThrow()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var clientGetResponse = () => Receive(text, HttpStatusCode.BadRequest).ReadAs<TestTextResponse>(200);

        var action = () => clientGetResponse().AssertSucceededAndReturn(r => r.AsText, HttpStatusCode.BadRequest);

        await action.Should().NotThrowAsync<TestResponseAssertionException>();
    }

    
    
    private Task<HttpResponseMessage> Receive(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return TestHttpClient.ReceiveResponse(r =>
            r.Respond(statusCode, MediaTypeNames.Text.Plain, content));
    }
}