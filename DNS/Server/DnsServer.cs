using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using DNS.DnsPacket;

namespace DNS.Server;

public class DnsServer : IDisposable
{
    private const int Port = 53;
    private const int SocketBufferSize = 512;
    private static readonly IPAddress Ip = new(new byte[] { 127, 0, 0, 1 });
    private static readonly IPEndPoint GoogleDns = new(new IPAddress(new byte[] { 8, 8, 8, 8 }), 53);
    private readonly Socket socket;
    private readonly Socket sender;

    private bool isExit;

    public DnsServer()
    {
        sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var localIep = new IPEndPoint(Ip, Port);
        socket.Bind(localIep);
    }

    public void Run()
    {
        Console.WriteLine("Сервер запущен!");
        while (!isExit)
        {
            var data = new byte[SocketBufferSize];
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            socket.ReceiveFrom(data, ref remoteEndPoint);
            Task.Run(() => HandleMessage(data, remoteEndPoint));
        }
    }

    public void Dispose()
    {
        isExit = true;
        socket.Dispose();
        sender.Dispose();
    }

    public void Exit()
    {
        Dispose();
    }

    private void HandleMessage(byte[] datagram, EndPoint remoteEndPoint)
    {
        var m = DnsMessage.Parse(datagram);
        if (m.Type != QR.REQUEST) return;
        sender.SendTo(datagram, GoogleDns);
        var buffer = new byte[SocketBufferSize];
        sender.Receive(buffer);
        var ma = DnsMessage.Parse(buffer);
        socket.SendTo(ma.GetBytes(), remoteEndPoint);
    }
}
