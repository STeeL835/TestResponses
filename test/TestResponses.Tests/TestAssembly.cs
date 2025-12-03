using System.Reflection;

namespace TestResponses.Tests;

public static class TestAssembly
{
    public static Assembly Instance { get; } = typeof(TestAssembly).Assembly;
}