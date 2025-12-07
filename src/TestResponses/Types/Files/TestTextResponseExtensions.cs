using TestResponses.Features;

namespace TestResponses.Files;

public static class TestFileResponseExtensions
{
    public static Task<ResponseFile> AssertSucceeded(this Task<TestFileResponse> responseTask, UniStatusCode? withStatusCode = null) 
        => responseTask.AssertSucceededAndReturn(r => r.AsFile, withStatusCode);
}