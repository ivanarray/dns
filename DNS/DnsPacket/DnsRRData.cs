using static System.Buffers.Binary.BinaryPrimitives;

namespace DNS.DnsPacket;

// ReSharper disable InconsistentNaming
public record DnsRRData(
    IReadOnlyList<string> Name,
    QueryType Type,
    ushort Class,
    int TTL,
    ushort RDLength,
    byte[] RData,
    DateTime Created,
    int ReadBytes)
{
    public bool IsValidData => Created + TimeSpan.FromSeconds(TTL) > DateTime.Now;

    public static unsafe DnsRRData Parse(byte* pointer, byte* startDatagram)
    {
        var name = ByteHelper.ParseName(pointer, startDatagram);
        pointer += name.readLen;
        var type = (QueryType)ReadInt16BigEndian(new Span<byte>(pointer, 2));
        pointer += 2;
        var cls = (ushort)ReadInt16BigEndian(new Span<byte>(pointer, 2));
        pointer += 2;
        var ttl = ReadInt32BigEndian(new Span<byte>(pointer, 4));
        pointer += 4;
        var dataLen = (ushort)ReadInt16BigEndian(new Span<byte>(pointer, 2));
        pointer += 2;
        var data = new Span<byte>(pointer, dataLen);

        return new DnsRRData(name.name, type, cls, ttl, dataLen, data.ToArray(), DateTime.Now,
            10 + name.readLen + dataLen);
    }

    public byte[] GetBytes()
    {
        var res = new List<byte>();
        res.AddRange(Name.NameToBytes());
        res.AddRange(ByteHelper.GetBytes((ushort)Type));
        res.AddRange(ByteHelper.GetBytes(Class));

        var bTtl = new byte[4];
        WriteInt32BigEndian(bTtl, TTL);
        res.AddRange(bTtl);
        res.AddRange(ByteHelper.GetBytes(RDLength));
        res.AddRange(RData);

        return res.ToArray();
    }
}