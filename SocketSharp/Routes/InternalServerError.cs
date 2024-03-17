using System;

namespace SocketSharp.Routes;

public class InternalServerError : Route
{
    private string errorMessage = "";

    public string Name => "Internal server error";
    public string[] Methods => new string[] { "GET" };


    public string GETWithMessage(string message)
    {
        errorMessage = message;
        return GET();
    }
    public string GET()
    {
        return "500: Internal Server Error\n" + errorMessage;
    }
}