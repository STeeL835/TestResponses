using System.Reflection;

namespace TestClientResponse.Tests;

public static class TestAssembly
{
    public static Assembly Instance { get; } = typeof(TestAssembly).Assembly;
}