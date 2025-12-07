using TestResponses.Features;

namespace TestResponses.Text;

public static class TestTextResponseExtensions
{
    public static Task<string> AssertSucceeded(this Task<TestTextResponse> responseTask, UniStatusCode? withStatusCode = null) 
        => responseTask.AssertSucceededAndReturn(r =>r.AsText, withStatusCode);
}