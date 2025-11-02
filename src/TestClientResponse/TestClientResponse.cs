using System.Net;

namespace TestClientResponse;

public abstract record TestResponse(HttpResponseMessage Response)
{
    public HttpStatusCode StatusCode => Response.StatusCode;

    public abstract Task Read(); 
    // TODO, TEST: property to check if read
    // TODO, TEST: Extension on Task<HttpResponseMessage> for awaiting and reading
}