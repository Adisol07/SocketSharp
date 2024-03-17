using System;

namespace SocketSharp;

public class RoutedWebServer
{
    private WebServer www;

    public Dictionary<string, Route> Routes { get; }
    public Dictionary<string, Route> SpecialRoutes { get; }

    public RoutedWebServer(string url = "http://localhost:8080/")
    {
        Routes = new Dictionary<string, Route>();
        SpecialRoutes = new Dictionary<string, Route>(){
            { "404", new Routes.NotFound() },
            { "405", new Routes.MethodNotAllowed() },
        };

        www = new WebServer(url);

        www.OnRequest += handleOnRequest;
    }

    public void Start() => www.Start();
    public void Stop() => www.Stop();

    private string handleOnRequest(string method, string url)
    {
        try
        {
            if (Routes.ContainsKey(url))
            {
                Route route = Routes[url];
                if (!route.Methods.Contains(method))
                    return SpecialRoutes["405"].GET();
                switch (method)
                {
                    case "GET":
                        return route.GET();
                    default:
                        return SpecialRoutes["405"].GET();
                }
            }
            return SpecialRoutes["404"].GET();
        }
        catch
        {
            return SpecialRoutes["500"].GET();
        }
    }
}