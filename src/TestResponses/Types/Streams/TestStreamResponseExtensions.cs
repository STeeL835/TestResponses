namespace TestResponses.Streams;

public static class TestStreamResponseExtensions
{
    public static Task<Stream> AssertSucceeded(this Task<TestStreamResponse> responseTask, UniStatusCode? withStatusCode = null) 
        => responseTask.AssertSucceededAndReturn(r => r.AsStream, withStatusCode);
}