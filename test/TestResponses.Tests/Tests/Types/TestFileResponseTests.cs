using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using RichardSzalay.MockHttp;
using TestResponses.Files;
using TestResponses.Tests.Utilities;

namespace TestResponses.Tests.Tests.Types;

public class TestFileResponseTests
{
    #region FileName
    
    [Fact]
    public async Task FileName_ResponseIsNotRead_ShouldReadFileName()
    {
        var httpResponse = await Receive("info.txt", [4, 8, 15, 16, 23, 42]);
        
        var testResponse = new TestFileResponse(httpResponse);

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.IsRead.Should().BeFalse();
        testResponse.FileName.Should().Be("info.txt");
    }
    
    [Fact]
    public async Task FileName_FilenamesDiffer_ShouldUseStarName()
    {
        var httpResponse = await Receive(fileName: "wrong", fileNameStar: "right", fileContent: [4, 8, 15, 16, 23, 42]);
        
        var testResponse = new TestFileResponse(httpResponse);

        testResponse.FileName.Should().Be("right");
    }
    
    [Fact]
    public async Task FileName_FilenamesMissing_ShouldReturnNull()
    {
        var httpResponse = await Receive(fileName: null, fileNameStar: null, fileContent: [4, 8, 15, 16, 23, 42]);
        
        var testResponse = new TestFileResponse(httpResponse);

        testResponse.FileName.Should().BeNull();
    }

    #endregion
    
    #region AsFile

    [Fact]
    public async Task AsFile_ResponseIsRead_ShouldReadResponseFile()
    {
        var httpResponse = await Receive("info.txt", [4, 8, 15, 16, 23, 42]);
        
        var testResponse = new TestFileResponse(httpResponse);
        await testResponse.Read();

        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        testResponse.IsRead.Should().BeTrue();
        testResponse.AsFile.Name.Should().Be("info.txt");
        testResponse.AsFile.Stream.ToByteArray().Should().BeSubsetOf([4, 8, 15, 16, 23, 42]);
    }
    
    [Fact]
    public async Task AsFile_FilenamesDiffer_ShouldUseStarName()
    {
        var httpResponse = await Receive(fileName: "wrong", fileNameStar: "right", fileContent: [4, 8, 15, 16, 23, 42]);
        
        var testResponse = new TestFileResponse(httpResponse);
        await testResponse.Read();

        testResponse.AsFile.Name.Should().Be("right");
        testResponse.AsFile.Stream.ToByteArray().Should().BeSubsetOf([4, 8, 15, 16, 23, 42]);
    }
    
    [Fact]
    public async Task AsFile_FilenameStarMissing_ShouldUseRegularName()
    {
        var httpResponse = await Receive(fileName: "old", fileNameStar: null, fileContent: [4, 8, 15, 16, 23, 42]);
        
        var testResponse = new TestFileResponse(httpResponse);
        await testResponse.Read();

        testResponse.AsFile.Name.Should().Be("old");
        testResponse.AsFile.Stream.ToByteArray().Should().BeSubsetOf([4, 8, 15, 16, 23, 42]);
    }
    
    [Fact]
    public async Task AsFile_BothFilenamesMissing_ShouldUseNullName()
    {
        var httpResponse = await Receive(fileName: null, fileNameStar: null, fileContent: [4, 8, 15, 16, 23, 42]);
        
        var testResponse = new TestFileResponse(httpResponse);
        await testResponse.Read();

        testResponse.AsFile.Name.Should().BeNull();
        testResponse.AsFile.Stream.ToByteArray().Should().BeSubsetOf([4, 8, 15, 16, 23, 42]);
    }

    
    [Fact]
    public async Task AsFile_ResponseNotRead_ShouldThrowException()
    {
        var httpResponse = await Receive("info.txt", [4, 8, 15, 16, 23, 42]);
        
        var testResponse = new TestFileResponse(httpResponse);
        var action = () => testResponse.AsFile;

        testResponse.IsRead.Should().BeFalse();
        action.Should().Throw<TestResponseException>();
    }
    
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task AsFile_BadStatusCode_ShouldStillReadResponse(HttpStatusCode statusCode)
    {
        var httpResponse = await Receive("info.txt", [8,0,8,0,5,5,5,3,5,3,5], statusCode: statusCode);
        
        var testResponse = new TestFileResponse(httpResponse);
        await testResponse.Read();
        
        testResponse.IsRead.Should().BeTrue();
        testResponse.StatusCode.Should().Be(statusCode);
        testResponse.AsFile.Name.Should().Be("info.txt");
        testResponse.AsFile.Stream.ToByteArray().Should().BeSubsetOf([8,0,8,0,5,5,5,3,5,3,5]);
    }

    #endregion

    #region ToString

    [Fact]
    public async Task ToString_ResponseIsNotRead_ShouldShowResponseAsNotRead()
    {
        var httpResponse = await Receive("info.txt", [4, 8, 15, 16, 23, 42]);
        
        var testResponse = new TestFileResponse(httpResponse);
        
        testResponse.ToString().Should().Be("""
            Status code: 200 (OK)
            Response: *not read*
            """);
    }

    [Fact]
    public async Task ToString_ResponseIsRead_ButEmpty_ShouldShowResponseAsEmpty()
    {
        var httpResponse = await Receive("info.txt", []);
        
        var testResponse = new TestFileResponse(httpResponse);
        await testResponse.Read();
        
        testResponse.ToString().Should().Be("""
            Status code: 200 (OK)
            File name: info.txt
            File size: 0B (0 bytes)
            """);
    }
    
    [Fact]
    public async Task ToString_ResponseIsRead_ShouldShowResponseAsEmpty()
    {
        var bytes = new byte[1536];
        Random.Shared.NextBytes(bytes);
        
        var httpResponse = await Receive("some.bin", bytes);
        
        var testResponse = new TestFileResponse(httpResponse);
        await testResponse.Read();

        var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        testResponse.ToString().Should().Be($"""
            Status code: 200 (OK)
            File name: some.bin
            File size: 1{separator}54KB (1536 bytes)
            """);
    }

    #endregion
    
    #region Configuration

    [Fact]
    public async Task Config_ReplacedFormatter_ShouldUseProvidedFormatter()
    {
        var httpResponse = await Receive("info.txt", [4, 8, 15, 16, 23, 42]);

        var testResponse = new TestFileResponse(httpResponse)
        {
            FileConfig = new()
            {
                Formatter = new TestResponseDelegateFormatter<TestFileResponse>(r => "format")
            }
        };
        await testResponse.Read();

        testResponse.ToString().Should().Be("format");
    }
    
    #endregion
    
    
    private Task<HttpResponseMessage> Receive(string? fileName, byte[] fileContent,
        string? fileNameStar = null, 
        HttpStatusCode statusCode = HttpStatusCode.OK, 
        string contentType = MediaTypeNames.Application.Octet)
    {
        var content = new StreamContent(fileContent.ToStream());
        content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileName = fileName,
            FileNameStar = fileNameStar,
        };
        
        return TestHttpClient.ReceiveResponse(r => r.Respond(statusCode, content));
    }
}