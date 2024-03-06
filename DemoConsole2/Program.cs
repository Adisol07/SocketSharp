using System;
using System.Text;
using SocketSharp;

namespace DemoConsole2;
internal class Program
{
    public static void Main(string[] args)
    {
        TCPClient client = new TCPClient();
        client.OnDataReceived += (data) =>{
            Console.WriteLine(Encoding.UTF8.GetString(data));
            client.Send(Encoding.UTF8.GetBytes("Hello World!"));
        };
        client.Connect("127.0.0.1", 1234);

        Console.ReadLine();
        client.Disconnect();
    }
}