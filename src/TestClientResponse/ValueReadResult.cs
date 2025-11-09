using System.Runtime.ExceptionServices;

namespace TestClientResponse;

/// <summary> Delays throwing exception until the moment the value is actually needed. </summary>
/// <remarks> <see cref="TestResponse.Read"/> method should not throw, because </remarks>
public record ValueReadResult<T>(T? Value, Exception? ExceptionHappenedDuringRead)
{
    public bool IsReadSuccessfully => ExceptionHappenedDuringRead == null;

    public T? GetOrThrow()
    {
        if (ExceptionHappenedDuringRead is not null)
            ExceptionDispatchInfo.Throw(ExceptionHappenedDuringRead);
        
        return Value;
    }
}