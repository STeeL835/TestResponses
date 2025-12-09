namespace TestResponses;

public class ResponseValue<T>
{
    private readonly TestResponse _testResponse;
    private T? _value;
    private Exception? _caughtException;

    public ResponseValueStatus Status { get; private set; } = ResponseValueStatus.NotRead;

    public T? Value => GetOrThrow();

    public ResponseValue(TestResponse testResponse) => _testResponse = testResponse;

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