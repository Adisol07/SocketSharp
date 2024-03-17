using System;

namespace SocketSharp;

public class FileWebServer
{
    private WebServer www;

    public string Directory { get; set; }
    public Route NotFound { get; set; }

    public FileWebServer(string directory, string url = "http://localhost:8080/")
    {
        NotFound = new Routes.NotFound();
        Directory = directory;
        www = new WebServer(url);

        www.OnRequest += handleOnRequest;
    }

    public void Start() => www.Start();
    public void Stop() => www.Stop();

    private string handleOnRequest(string method, string url)
    {
        if (method == "GET")
        {
            if (File.Exists(Directory + url))
            {
                return File.ReadAllText(Directory + url);
            }
            return NotFound.GET();
        }
        else if (method == "POST")
        {
            return "405_1: This method is yet to be implemented";
        }
        else
        {
            return "405: Method Not Allowed";
        }
    }
}