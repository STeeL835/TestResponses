using TestResponses.Text;

namespace TestResponses.Json;

public class TestJsonResponse(HttpResponseMessage httpResponse) : TestTextResponse(httpResponse)
{
    public static TestJsonResponseConfiguration GlobalJsonConfig = TestJsonResponseConfiguration.Default;
    public TestJsonResponseConfiguration JsonConfig { get; init; } = GlobalJsonConfig;

    public T? As<T>() => JsonConfig.Serializer.Deserialize<T>(AsText);
    
    internal override bool CanHandleContent() => ContentType is not null && ContentType.Contains("json");
    
    protected override string GetInfoString() => TestJsonResponseFormatter.Format(this);
}