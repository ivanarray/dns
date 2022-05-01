using System.Net;
using System.Net.Sockets;

namespace DNS;

public class DnsServer
{
    private const int Port = 53;
    private static readonly IPAddress Ip = new(new byte[] { 127, 0, 0, 1 });
    private readonly Socket socket;

    public DnsServer()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var localIep = new IPEndPoint(Ip, Port);
        socket.Bind(localIep);
    }

    public void Run()
    {
        Console.WriteLine("Ссервер запущет!");
        while (true)
        {
            var data = new byte[256];
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var bytes = socket.ReceiveFrom(data, ref remoteEndPoint);
            Task.Run(()=>HandleMessage(data,remoteEndPoint));
            Console.WriteLine($"Получено {bytes} байтов");
        }
    }

    private void HandleMessage(byte[] data, EndPoint remoteEndPoint)
    {
        var m = DnsPacket.DnsMessage.Parse(data);
    }
}