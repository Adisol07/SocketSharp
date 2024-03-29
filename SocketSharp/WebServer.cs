using System;
using System.Text;
using System.Net;

namespace SocketSharp;

public class WebServer
{
    private HttpListener? listener;

    public bool IsListening { get; private set; } = false;

    public RequestEventHandler? OnRequest;

    public WebServer(string url = "http://localhost:8080/")
    {
        listener = new HttpListener();
        listener.Prefixes.Add(url);
    }

    public void Start()
    {
        listener?.Start();
        IsListening = true;
        listen();
    }

    private void listen()
    {
        ThreadPool.QueueUserWorkItem(state => {
            while (listener!.IsListening && IsListening)
            {
                HttpListenerContext? context = listener?.GetContext();
                HttpListenerRequest? request = context?.Request;

                string responseString = "<h1 style=\"font-size:56px;text-align:center;color:red;margin-top:50vh;margin-bottom:auto;\">Event handler is null!</h1>";
                if (OnRequest != null)
                {
                    responseString = OnRequest.Invoke(request!.HttpMethod, request?.RawUrl!);
                }
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);

                HttpListenerResponse response = context!.Response;
                response.ContentType = "text/html";
                response.ContentLength64 = responseBytes.Length;
                response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
                response.OutputStream.Close();
            }
        });
    }

    public delegate string RequestEventHandler(string method, string url);
}