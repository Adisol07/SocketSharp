using System;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Net.Security;

namespace SocketSharp;

public class WebServer
{
    private HttpListener? listener;

    public bool IsListening { get; private set; } = false;
    public string Prefix { get; private set; } = "";

    public RequestEventHandler? OnRequest;

    public WebServer(string url = "http://localhost:8080/")
    {
        Prefix = url;
        listener = new HttpListener();
        listener.Prefixes.Add(url);
    }

    public void Start()
    {
        listener?.Start();
        IsListening = true;
        listen();
    }
    public void Stop()
    {
        IsListening = false;
        _ = Task.Run(() =>
        {
            while (listener!.IsListening)
            {
                Thread.Sleep(100);
            }
        });
        listener?.Stop();
    }

    private void listen()
    {
        ThreadPool.QueueUserWorkItem(state =>
        {
            while (listener!.IsListening && IsListening)
            {
                HttpListenerContext? context = listener?.GetContext();
                HttpListenerRequest? request = context?.Request;

                handleRegularRequest(context!, request!); // For future SSL support
            }
        });
    }
    private void handleRegularRequest(HttpListenerContext context, HttpListenerRequest request)
    {
        string responseString = "<h1 style=\"font-size:56px;text-align:center;color:red;margin-top:50vh;margin-bottom:auto;\">Event handler is null!</h1>";
        if (OnRequest != null)
        {
            responseString = OnRequest.Invoke(request!.HttpMethod, request?.RawUrl!);
        }
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);

        HttpListenerResponse response = context!.Response;
        response.ContentType = "text/html";
        response.ContentLength64 = responseBytes.Length;
        response.Headers["Server"] = "SocketSharp Server";
        response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
        response.OutputStream.Close();
    }

    public delegate string RequestEventHandler(string method, string url);
}