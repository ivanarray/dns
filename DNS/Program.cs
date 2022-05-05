using DNS.Server;

namespace DNS;

internal static class Program
{
    public static void Main(string[] args)
    {
        new DnsServer().Run();
    }
}