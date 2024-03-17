using System;

namespace SocketSharp.Routes;

public class MethodNotAllowed : Route
{
    public string Name => "Method not allowed";

    public string[] Methods => new string[] { "GET" };

    public string GET()
    {
        return "405: Method Not Allowed";
    }
}