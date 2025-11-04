using System.Net;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestClientResponse.String;
using TestClientResponse.Tests.Utilities;

namespace TestClientResponse.Tests;

public class TestStringResponseTests // TODO: maybe group by feature, not by class (like, reading/status codes
{
    [Fact]
    public async Task AsString_ResponseIsRead_ShouldReadStringResponse()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var httpResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(MediaTypeNames.Text.Plain, text));
        
        var testResponse = new TestStringResponse(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsString.Should().Be(text);
    }
    
    [Fact]
    public async Task AsString_ResponseNotRead_ShouldThrowException()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var httpResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(MediaTypeNames.Text.Plain, text));
        
        var testResponse = new TestStringResponse(httpResponse);
        var action = () => testResponse.AsString;

        testResponse.IsRead.Should().BeFalse();
        action.Should().Throw<TestResponseException>();
    }
    
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task AsTestStringResponse_BadStatusCode_ShouldStillReadResponse(HttpStatusCode statusCode)
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
            r.Respond(statusCode, MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        testResponse.IsRead.Should().BeTrue();
        testResponse.StatusCode.Should().Be(statusCode);
        testResponse.AsString.Should().Be(text);
    }
    
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Created)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData((HttpStatusCode)299)]
    public async Task ShouldHaveStatusCode_Range_StatusCodeMatches_ShouldNotThrow(HttpStatusCode statusCode)
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
                r.Respond(statusCode, MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(200..299);

        action.Should().NotThrow();
        testResponse.AsString.Should().Be(text);
    }
    
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task ShouldHaveStatusCode_Range_StatusCodeDoesNotMatch_ShouldThrow(HttpStatusCode statusCode)
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
                r.Respond(statusCode, MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(400..499);
        
        action.Should().Throw<TestStringResponseAssertionException>()
            .WithMessage("Response status code is not in range 400..499*");
    }

    public static IEnumerable<object[]> ShouldHaveStatusCode_Range_IncorrectValues_ShouldThrow_TestCases()
    {
        yield return [200..];
        yield return [(..299)]; // it's spread operator without parentheses
        yield return [^200..299];
        yield return [200..^299];
        yield return [200..600];
        yield return [99..500];
        yield return [299..200]; // TODO: make TestCase class 
    }
    [Theory, MemberData(nameof(ShouldHaveStatusCode_Range_IncorrectValues_ShouldThrow_TestCases))]
    public async Task ShouldHaveStatusCode_Range_IncorrectValues_ShouldThrow(Range statusCodeRange)
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
                r.Respond(MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(statusCodeRange);
        
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public async Task ShouldHaveStatusCode_Enum_StatusCodeMatches_ShouldNotThrow()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
                r.Respond(HttpStatusCode.NotFound, MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(HttpStatusCode.NotFound);

        action.Should().NotThrow();
    }
    
    [Theory]
    [InlineData(HttpStatusCode.OK)] // 200
    [InlineData(HttpStatusCode.InternalServerError)] // 500
    [InlineData(HttpStatusCode.BadRequest)] // 400
    public async Task ShouldHaveStatusCode_Enum_StatusCodeDoesNotMatch_ShouldThrow(HttpStatusCode statusCode)
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
                r.Respond(statusCode, MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(HttpStatusCode.MethodNotAllowed); // 405
        
        action.Should().Throw<TestStringResponseAssertionException>()
            .WithMessage("Response status code is not 405*");
    }
    
    [Fact]
    public async Task ShouldHaveStatusCode_Int_StatusCodeMatches_ShouldNotThrow()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
                r.Respond(HttpStatusCode.Unauthorized, MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(401);

        action.Should().NotThrow();
    }
    
    [Theory]
    [InlineData(HttpStatusCode.OK)] // 200
    [InlineData(HttpStatusCode.InternalServerError)] // 500
    [InlineData(HttpStatusCode.BadRequest)] // 400
    public async Task ShouldHaveStatusCode_Int_StatusCodeDoesNotMatch_ShouldThrow(HttpStatusCode statusCode)
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
                r.Respond(statusCode, MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(429);
        
        action.Should().Throw<TestStringResponseAssertionException>()
            .WithMessage("Response status code is not 429*");
    }
    
    [Theory]
    [InlineData(-1)] 
    [InlineData(99)] 
    [InlineData(600)]
    public async Task ShouldHaveStatusCode_Int_StatusCodeIsInvalid_ShouldThrow(int expectedStatusCode)
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
                r.Respond(MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(expectedStatusCode);

        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public async Task AssertionException_ShouldHaveStatusAndResponse()
    {
        var text = "Lorem ipsum dolor sit amet";
        
        var testResponse = await TestHttpClient.ReceiveResponse(r => 
                r.Respond(MediaTypeNames.Text.Plain, text))
            .AsTestStringResponse();
        
        var action = () => testResponse.ShouldHaveStatusCode(500);

        action.Should().Throw<TestStringResponseAssertionException>()
            .WithMessage("""
            *Status code: 200 (OK)
            Response: 
            Lorem ipsum dolor sit amet
            """);
    }
}