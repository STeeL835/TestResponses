namespace TestResponses.Features;

public static class ResponseValue
{
    public static async Task<ResponseValue<T>> Create<T>(TestResponse testResponse, Func<Task<T?>> reader)
    {
        var container = new ResponseValue<T>(testResponse);
        await container.Read(reader);
        return container;
    }
}