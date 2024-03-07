using System;
using System.Text;
using SocketSharp;

namespace DemoConsole1;
internal class Program
{
    public static void Main(string[] args)
    {
        TCPServer server = new TCPServer("127.0.0.1", 1234);
        server.OnClientConnected += (client) =>
        {
            Console.WriteLine("Connected: " + client);
            server?.Send(client, Encoding.UTF8.GetBytes("Hello World!"));
        };
        server.OnClientDisconnected += (client) =>
        {
            Console.WriteLine("Disconnected: " + client);
        };
        server.OnDataReceived += (client, data) =>
        {
            Console.WriteLine("Received: " + Encoding.UTF8.GetString(data) + " from: " + client);
        };

        server.Start();
        Console.ReadLine();
        server.Stop();
    }
}