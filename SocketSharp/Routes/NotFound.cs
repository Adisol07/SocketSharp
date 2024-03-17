using System;

namespace SocketSharp.Routes;

public class NotFound : Route
{
    public string Name => "NotFound";
    public string[] Methods => new string[] { "GET" };

    public string GET()
    {
        return "404: Not Found";
    }
}