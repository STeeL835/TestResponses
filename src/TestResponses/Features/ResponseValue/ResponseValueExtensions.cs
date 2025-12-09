namespace TestResponses;

public static class ResponseValueExtensions
{
    public static T? GetOrThrow<T>(this ResponseValue<T>? container)
    {
        if (container is null) 
            throw new TestResponseException($"Response is not read. Read response before accessing response content");

        return container.Value;
    }
}