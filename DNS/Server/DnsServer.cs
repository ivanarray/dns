using System.Net;
using System.Net.Sockets;
using DNS.Cache;
using DNS.DnsPacket;
using Timer = System.Timers.Timer;

namespace DNS.Server;

public class DnsServer : IDisposable
{
    private const int Port = 53;
    private const int SocketBufferSize = 512;
    private static readonly IPAddress Ip = new(new byte[] { 127, 0, 0, 1 });
    private static readonly IPEndPoint GoogleDns = new(new IPAddress(new byte[] { 8, 8, 8, 8 }), 53);
    private readonly Socket listener;
    private readonly Socket sender;
    private readonly DnsCache cache;
    private readonly Timer cacheCleanerTimer;

    private bool isExit;

    public DnsServer()
    {
        cacheCleanerTimer = new Timer(60 * 1000);
        sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var localIep = new IPEndPoint(Ip, Port);
        listener.Bind(localIep);
        cache = DnsCache.Load();
        cacheCleanerTimer.Elapsed += (_, _) =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Очистка старых записей]");
            Console.ResetColor();
            Task.Run(cache.CleanOldRecords);
        };
        cacheCleanerTimer.Start();
        cacheCleanerTimer.AutoReset = true;
    }

    public void Run()
    {
        Console.WriteLine("Сервер запущен!");
        Task.Run(() =>
        {
            while (!isExit)
            {
                var data = new byte[SocketBufferSize];
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                listener.ReceiveFrom(data, ref remoteEndPoint);
                HandleMessage(data, remoteEndPoint);
            }
        });
        while (Console.ReadLine()?.ToLower() is not "exit")
        {
        }

        Dispose();
    }

    public void Dispose()
    {
        if (isExit) return;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[Закрываю]");
        Console.ResetColor();
        isExit = true;
        listener.Dispose();
        sender.Dispose();
        cache.Dispose();
        cacheCleanerTimer.Close();
    }

    private void HandleMessage(byte[] datagram, EndPoint remoteEndPoint)
    {
        var m = DnsMessage.Parse(datagram);

        if (m.Type != QR.REQUEST) return;

        if (cache.Contains(m.Queries[0].Name, m.Queries[0].Type))
        {
            SendFromCache(remoteEndPoint, m);
            return;
        }

        sender.SendTo(datagram, GoogleDns);
        var buffer = new byte[SocketBufferSize];
        sender.Receive(buffer);
        var ma = DnsMessage.Parse(buffer);
        cache.Add(ma.Queries[0].Name, ma.RData);
        listener.SendTo(ma.GetBytes(), remoteEndPoint);
    }

    private void SendFromCache(EndPoint remoteEndPoint, DnsMessage m)
    {
        var rr = cache.Get(m.Queries[0].Name, m.Queries[0].Type);
        var ans = rr.Where(x => x.Type is QueryType.A or QueryType.AAAA).ToArray();
        var ns = rr.Where(x => x.Type is QueryType.NS).ToArray();
        var oth = rr.Where(x => x.Type is QueryType.PTR).ToArray();
        var res = new List<DnsRRData>();
        res.AddRange(ans);
        res.AddRange(ns);
        res.AddRange(oth);

        var mes = new DnsMessage(m.Headers with
        {
            Flags = m.Headers.Flags with
            {
                Type = QR.RESPONSE,
                Bits = DnsHeadresBits.RA | m.Headers.Flags.Bits
            },
            AnswersCount = (ushort)ans.Length,
            AuthoritySectionCount = (ushort)ns.Length,
            AdditionalRecordSecCount = (ushort)oth.Length
        }, m.Queries, res.ToArray());
        listener.SendTo(mes.GetBytes(), remoteEndPoint);
    }
}
