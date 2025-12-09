using TestResponses.Empty;
using TestResponses.Features;
using TestResponses.Files;
using TestResponses.Json;
using TestResponses.Streams;
using TestResponses.Tests.Utilities;
using TestResponses.Text;

namespace TestResponses.Tests.Tests.Features.ResponseDetection;

public class TestResponseTypesTests
{
    [Fact]
    public void RegisterFromAssembly_ShouldOrderRegisteredTypesByDescendingSpecificity()
    {
        var detector = new TestResponseTypes();
        detector.RegisterFromAssemblies(TestAssembly.Instance);

        detector.List.Should().ContainInOrder([
            typeof(JsonPatchResponse),
            typeof(TestJsonResponse<>),
            typeof(TestTextResponse),
            typeof(TestStreamResponse),
        ], "types must be sorted from most specific to least");
    }
    
    [Fact]
    public void RegisterFromAssemblies_ShouldRegisterAllFittingTypes()
    {
        var detector = new TestResponseTypes();
        detector.RegisterFromAssemblies(TestAssembly.Instance);

        detector.List.Should().BeEquivalentTo([
            typeof(TestEmptyResponse),
            typeof(TestStreamResponse),
            typeof(TestFileResponse),
            typeof(TestJsonResponse<>),
            typeof(TestTextResponse),
            typeof(JsonPatchResponse),
            typeof(MarkdownResponse),
            typeof(CookieContentResponse),
            typeof(NonIdenpotentReadResponse),
        ]);

        detector.List.Should().NotContain(typeof(InvalidCtorResponse), "it has incompatible ctor");
        detector.List.Should().NotContain(typeof(HeaderContentResponse), "it is abstract");
    }

    [Theory]
    [InlineData(typeof(InvalidCtorResponse))]
    [InlineData(typeof(HeaderContentResponse))]
    [InlineData(typeof(HttpResponseMessage))]
    public void Register_ShouldRegisterAllFittingTypes(Type type)
    {
        var detector = new TestResponseTypes();
        
        var action = () => detector.Register(type);

        action.Should().Throw<ArgumentException>();
    }
}