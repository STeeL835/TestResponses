namespace TestResponses.Utilities;

public static class TypeExtensions
{
    /// <summary>
    /// Returns a short type name (without namespaces), but supports generic parameters too
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetCompactName(this Type type)
    {
        return type.IsGenericType 
            ? $"{type.Name.AsSpan()[..type.Name.IndexOf('`')]}<{string.Join(", ", type.GetGenericArguments().Select(GetCompactName))}>" 
            : type.Name;
    }
}