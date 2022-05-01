// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace DNS.DnsPacket;

public enum QueryType : ushort
{
    A = 1,
    AAAA = 28,
    NS = 2,
    PTR = 12,
}