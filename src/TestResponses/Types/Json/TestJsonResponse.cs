using System.Text.Json;
using TestResponses.Text;

namespace TestResponses.Json;

public class TestJsonResponse(HttpResponseMessage httpResponse) : TestTextResponse(httpResponse)
{
    public static TestJsonResponseConfiguration GlobalJsonConfig = TestJsonResponseConfiguration.Default;
    public TestJsonResponseConfiguration JsonConfig { get; init; } = GlobalJsonConfig;
    
    public T? As<T>() => JsonSerializer.Deserialize<T>(AsText, JsonConfig.SerializerOptions);
    
    internal override bool CanHandleContent() => ContentType is not null && ContentType.Contains("json");
    
    protected override string GetInfoString() => TestJsonResponseFormatter.Format(this);
}