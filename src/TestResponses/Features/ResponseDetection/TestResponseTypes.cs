using System.Reflection;
using TestResponses.Empty;
using TestResponses.Files;
using TestResponses.Json;
using TestResponses.Streams;
using TestResponses.Text;

namespace TestResponses;

internal class TestResponseTypes
{
    public static TestResponseTypes Global { get; } = new();
    
    private List<Type> _registeredTypes = new()
    {
        typeof(TestEmptyResponse),
        typeof(TestJsonResponse),
        typeof(TestTextResponse),
        typeof(TestFileResponse),
        typeof(TestStreamResponse),
    };

    public IReadOnlyList<Type> List => _registeredTypes.AsReadOnly();

    /// <summary> Scans assemblies and registers all TestResponse types found </summary>
    public void RegisterFromAssemblies(params Assembly[] assemblies)
    {
        var typesToRegister = new List<Type>();
        
        foreach (var assembly in assemblies)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var assemblyTypes = assembly.GetTypes()
                .Where(IsTestResponseType)
                .Where(t => !_registeredTypes.Contains(t))
                .Where(CanTestResponseTypeBeInstantiated)
                .ToList();
            
            typesToRegister.AddRange(assemblyTypes);
        }
        
        Register(typesToRegister);
    }

    /// <summary> Registers TestResponse type for best-fit response detection (to provide correct response info) </summary>
    public void Register<T>() where T : TestResponse => Register(typeof(T));
    
    /// <summary> Registers TestResponse type for best-fit response detection (to provide correct response info) </summary>
    public void Register(params IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (_registeredTypes.Contains(type)) 
                throw new ArgumentException($"Type '{type}' is already registered");
            
            if (!IsTestResponseType(type)) 
                throw new ArgumentException($"Type '{type}' is not a TestResponse type");
            
            if (!CanTestResponseTypeBeInstantiated(type)) 
                throw new ArgumentException(
                    $"Type '{type}' can't be instantiated" +
                    $" (TestResponse should have a ctor with {nameof(HttpResponseMessage)} as its only parameter");
            
            _registeredTypes.Add(type);        
        }

        SortTypes();
    }

    /// <summary> Determines if a type is a valid TestResponse type </summary>
    private bool IsTestResponseType(Type type)
    {
        return type.IsAssignableTo(typeof(TestResponse));
    }
    
    /// <summary> Determines if a type is a valid TestResponse type </summary>
    private bool CanTestResponseTypeBeInstantiated(Type type)
    {
        if (type.IsAbstract) return false;
        
        var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
        
        var neededCtor = ctors.Where(c => c.GetParameters().Length == 1)
            .Where(c => c.GetParameters()[0].ParameterType == typeof(HttpResponseMessage));
        
        return neededCtor.Any();
    }

    /// <summary>
    /// Analyzes the type hierarchy and sorts types so that most inherited and precise are checked first (json before broader text)
    /// </summary>
    private void SortTypes()
    {
        var orderedTypes = _registeredTypes
            .Select(GetInheritancePathGroup)
            .OrderByDescending(g => g.FullNamePath);

        _registeredTypes = orderedTypes.Select(g => g.Type).ToList();
    }

    private TypeInheritanceInfo GetInheritancePathGroup(Type type)
    {
        var fullNamePath = type.FullName!;
        var shortNamePath = type.Name;
        var current = type;

        while (current!.BaseType != typeof(TestResponse))
        {
            current = current.BaseType!;
            fullNamePath = $"{current.FullName}-{fullNamePath}";
            shortNamePath = $"{current.Name}-{shortNamePath}";
        }
        
        return new(fullNamePath, shortNamePath, type);
    }

    private record TypeInheritanceInfo(string FullNamePath, string ShortNamePath, Type Type);
}