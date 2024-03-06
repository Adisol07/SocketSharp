using System;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace SocketSharp;

public class TCPClient
{
    private TcpClient? client;
    private NetworkStream? stream;

    public bool IsListening { get; private set; } = false;

    public DataReceivedEventHandler? OnDataReceived;

    public TCPClient()
    { }

    public async Task Connect(string ipaddress, ushort port)
    {
        client = new TcpClient();
        await client.ConnectAsync(ipaddress, port);

        stream = client.GetStream();
        IsListening = true;
        listen();
    }
    public async Task Send(byte[] data)
    {
        await stream?.WriteAsync(data, 0, data.Length)!;
    }
    public void Disconnect()
    {
        IsListening = false;
        stream?.Close();
        client?.Close();
    }

    private void listen()
    {
        ThreadPool.QueueUserWorkItem(async state =>
        {
            while (IsListening && client!.Connected)
            {
                byte[] receivedData = await receiveData();
                if (OnDataReceived != null)
                    OnDataReceived.Invoke(receivedData);
            }
        });
    }
    private async Task<byte[]> receiveData()
    {
        byte[] buffer = new byte[1024];
        int bytesRead = await stream?.ReadAsync(buffer, 0, buffer.Length)!;
        byte[] receivedData = new byte[bytesRead];
        Array.Copy(buffer, receivedData, bytesRead);
        return receivedData;
    }

    public delegate void DataReceivedEventHandler(byte[] data);
}