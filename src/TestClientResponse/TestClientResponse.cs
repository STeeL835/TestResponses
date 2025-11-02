using System.Net;

namespace TestClientResponse;

public abstract record TestResponse(HttpResponseMessage Response)
{
    public HttpStatusCode StatusCode => Response.StatusCode;

    public bool IsRead { get; protected set; }

    public abstract Task Read(); 
    
    protected T GetReadValue<T>(T value) // TODO: maybe find a better name
    {
        if (IsRead) return value;
        throw new TestResponseException($"Response is not read. Call {nameof(Read)}() before accessing response content");
    }
    
    // TODO, TEST: Extension on Task<HttpResponseMessage> for awaiting and reading
    // TODO, TEST: do not read again if already read
}