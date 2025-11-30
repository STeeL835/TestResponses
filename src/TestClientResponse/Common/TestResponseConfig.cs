using TestClientResponse.Empty;
using TestClientResponse.Json;
using TestClientResponse.Text;

namespace TestClientResponse;

internal static class TestResponseConfig
{
    public static IReadOnlyList<Type> TestResponseTypes => [
        typeof(TestEmptyResponse),
        typeof(TestJsonResponse<>),
        typeof(TestTextResponse),
    ];
    
    // TODO: Register types from side assemblies (can be abstract, generic and not have ctor with only HttpResponse)
    // TODO: What if type is duplicated
    // TODO: Autoregister from our assemblies
    // TODO: hierarchy can be a list, just group by inheritance branch then sort by inheritance depth and put Unknown as fallback
}