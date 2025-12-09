namespace TestResponses.Tests.Utilities;

public static class StreamConverters
{
    public static MemoryStream ToStream(this byte[] byteArray)
    {
        var memoryStream = new MemoryStream(byteArray);
        return memoryStream;
    }

    public static byte[] ToByteArray(this Stream stream)
    {
        if (stream is not MemoryStream memoryStream)
        {
            memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
        }
        
        
        return memoryStream.ToArray();
    }
}