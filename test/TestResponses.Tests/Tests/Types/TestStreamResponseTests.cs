using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestResponses.Streams;
using TestResponses.Tests.Utilities;
using TestResponses.Text;

namespace TestResponses.Tests.Tests.Types;

public class TestStreamResponseTests
{
    #region AsStream

    [Fact]
    public async Task AsStream_ResponseIsRead_ShouldReadResponseStream()
    {
        byte[] bytes = [4, 8, 15, 16, 23, 42];

        var httpResponse = await Receive(bytes);
        
        var testResponse = new TestStreamResponse(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsStream.ToByteArray().Should().BeSubsetOf(bytes);
    }
    
    [Fact]
    public async Task AsStream_ResponseNotRead_ShouldThrowException()
    {
        byte[] bytes = [4, 8, 15, 16, 23, 42];

        var httpResponse = await Receive(bytes);
        
        var testResponse = new TestStreamResponse(httpResponse);
        var action = () => testResponse.AsStream;

        testResponse.IsRead.Should().BeFalse();
        action.Should().Throw<TestResponseException>();
    }
    
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task AsStream_BadStatusCode_ShouldStillReadResponse(HttpStatusCode statusCode)
    {
        byte[] bytes = [4, 8, 15, 16, 23, 42];

        var httpResponse = await Receive(bytes, statusCode);
        
        var testResponse = new TestStreamResponse(httpResponse);
        await testResponse.Read();
        
        testResponse.IsRead.Should().BeTrue();
        testResponse.StatusCode.Should().Be(statusCode);
        testResponse.AsStream.ToByteArray().Should().BeSubsetOf(bytes);
    }

    #endregion

    #region ToString

    [Fact]
    public async Task ToString_ResponseIsNotRead_ShouldShowResponseAsNotRead()
    {
        byte[] bytes = [4, 8, 15, 16, 23, 42];

        var httpResponse = await Receive(bytes);
        
        var testResponse = new TestStreamResponse(httpResponse);
        
        testResponse.ToString().Should().Be("""
            Status code: 200 (OK)
            Response: *not read*
            """);
    }

    [Fact]
    public async Task ToString_ResponseIsRead_ButEmpty_ShouldShowResponseAsEmpty()
    {
        byte[] bytes = [];

        var httpResponse = await Receive(bytes);
        
        var testResponse = new TestStreamResponse(httpResponse);
        await testResponse.Read();
        
        testResponse.ToString().Should().Be($"""
            Status code: 200 (OK)
            Response: application/octet-stream stream 0 bytes long
            """);
    }
    
    [Fact]
    public async Task ToString_ResponseIsRead_ShouldShowResponseAsEmpty()
    {
        byte[] bytes = [4, 8, 15, 16, 23, 42];

        var httpResponse = await Receive(bytes);
        
        var testResponse = new TestStreamResponse(httpResponse);
        await testResponse.Read();
        
        testResponse.ToString().Should().Be("""
            Status code: 200 (OK)
            Response: application/octet-stream stream 6 bytes long
            """);
    }

    #endregion
    
    
    
    private Task<HttpResponseMessage> Receive(byte[] content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return TestHttpClient.ReceiveResponse(r =>
            r.Respond(statusCode, MediaTypeNames.Application.Octet, content.ToStream()));
    }
}