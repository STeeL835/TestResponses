namespace TestResponses;

/// <summary>
/// Holds a value read from a TestResponse and postpones any read exception until the value is accessed.
/// </summary>
public class ResponseValue<T>
{
    private readonly TestResponse _testResponse;
    private T? _value;
    private Exception? _caughtException;

    /// <summary>
    /// Indicates the current read state of the value.
    /// </summary>
    public ResponseValueStatus Status { get; private set; } = ResponseValueStatus.NotRead;

    /// <summary>
    /// The read value. Throws if the value was not read successfully.
    /// </summary>
    public T? Value => GetOrThrow();

    /// <summary>
    /// Creates a response value container tied to the owning <see cref="TestResponse" />.
    /// </summary>
    public ResponseValue(TestResponse testResponse) => _testResponse = testResponse;

    /// <summary>
    /// Reads the value using the provided asynchronous reader delegate.
    /// </summary>
    /// <param name="reader">Function that reads the underlying value.</param>
    public async Task Read(Func<Task<T>> reader)
    {
        try
        {
            _value = await reader();
            Status = ResponseValueStatus.ReadSuccessfully;
        }
        catch (Exception ex)
        {
            _caughtException = ex;
            Status = ResponseValueStatus.ReadWithException;
        }
    }

    private T? GetOrThrow()
    {
        switch (Status)
        {
            case ResponseValueStatus.NotRead:
                throw new TestResponseException($"Response value is not read. Probably test response didn't read into a container");

            case ResponseValueStatus.ReadWithException:
                throw new TestResponseAssertionException($"""
                    Response could not be read as {typeof(T).Name} (see inner exception)
                    {_testResponse}
                    """, _caughtException);

            case ResponseValueStatus.ReadSuccessfully:
                return _value;

            default:
                throw new ArgumentOutOfRangeException(nameof(Status), Status, null);
        }
    }
}