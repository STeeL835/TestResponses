using TestResponses.Utilities;

namespace TestResponses.Tests.Tests.Utilities;

public class TypeExtensionsTests
{
    [Theory]
    [InlineData(typeof(int), "Int32")]
    [InlineData(typeof(string), "String")]
    [InlineData(typeof(List<>), "List<T>")]
    [InlineData(typeof(List<bool>), "List<Boolean>")]
    [InlineData(typeof(Dictionary<,>), "Dictionary<TKey, TValue>")]
    [InlineData(typeof(Dictionary<int,string>), "Dictionary<Int32, String>")]
    [InlineData(typeof(List<List<int>>), "List<List<Int32>>")]
    [InlineData(typeof(int[]), "Int32[]")]
    [InlineData(typeof(int[][]), "Int32[][]")]
    [InlineData(typeof(int[,]), "Int32[,]")]
    public void GetShortName_ShouldReturnValidNames(Type type, string expectedName)
    {
        var name = type.GetCompactName();
        
        name.Should().Be(expectedName);
    }
}