using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketSharp;

public class TCPServer
{
    private TcpListener? listener;

    public Dictionary<string, TcpClient>? Clients { get; private set; }
    public bool IsListening { get; private set; } = false;

    public ClientConnectedEventHandler? OnClientConnected;
    public ClientDisconnectedEventHandler? OnClientDisconnected;
    public DataReceivedEventHandler? OnDataReceived;

    public TCPServer(IPAddress ipaddress, ushort port)
    {
        listener = new TcpListener(ipaddress, port);
        Clients = new Dictionary<string, TcpClient>();
    }
    public TCPServer(string ipaddress, ushort port)
    {
        listener = new TcpListener(IPAddress.Parse(ipaddress), port);
        Clients = new Dictionary<string, TcpClient>();
    }
    public TCPServer(ushort port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        Clients = new Dictionary<string, TcpClient>();
    }

    public void Start()
    {
        listener?.Start();
        IsListening = true;
        listen();
    }
    public void Stop()
    {
        foreach (var pair in Clients!)
        {
            pair.Value.Close();
        }
        Clients.Clear();
        IsListening = false;
        listener?.Stop();
    }
    public async Task Send(string client, byte[] data)
    {
        if (Clients?.ContainsKey(client) == false)
            throw new ArgumentException("Client is not connected to server!", "client");
        TcpClient c = Clients?[client]!;
        NetworkStream stream = c.GetStream()!;
        await stream.WriteAsync(data, 0, data.Length);
    }

    private void listen()
    {
        ThreadPool.QueueUserWorkItem(state =>
        {
            while (IsListening)
            {
                TcpClient client = listener!.AcceptTcpClient();
                Clients?.Add(client.Client.RemoteEndPoint?.ToString()!, client);

                if (OnClientConnected != null)
                    OnClientConnected.Invoke(client.Client.RemoteEndPoint?.ToString()!);

                _ = Task.Run(() => handleClient(client));
            }
        });
    }

    private async Task handleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[1024];
        int bytes;

        try
        {
            while ((bytes = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                byte[] data = new byte[bytes];
                Array.Copy(buffer, data, bytes);

                if (OnDataReceived != null)
                    OnDataReceived.Invoke(client.Client.RemoteEndPoint?.ToString()!, data);
            }
        }
        catch (IOException) 
        {}
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
        finally
        {
            if (OnClientDisconnected != null)
                OnClientDisconnected.Invoke(client.Client.RemoteEndPoint?.ToString()!);

            Clients?.Remove(client.Client.RemoteEndPoint?.ToString()!);
            client.Close();
        }
    }

    public delegate void ClientConnectedEventHandler(string client);
    public delegate void ClientDisconnectedEventHandler(string client);
    public delegate void DataReceivedEventHandler(string client, byte[] data);
}