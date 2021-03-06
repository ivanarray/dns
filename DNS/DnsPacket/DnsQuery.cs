namespace DNS.DnsPacket;

using static System.Buffers.Binary.BinaryPrimitives;

public record DnsQuery(IReadOnlyList<string> Name, QueryType Type, ushort Class, int ReadBytes)
{
    public static unsafe DnsQuery Parse(byte* pointer, byte* startDatagram)
    {
        var name = ByteHelper.ParseName(pointer, startDatagram);
        pointer += name.readLen;
        var type = (QueryType)ReadInt16BigEndian(new Span<byte>(pointer, 2));
        pointer += 2;
        var cls = (ushort)ReadInt16BigEndian(new Span<byte>(pointer, 2));

        return new DnsQuery(name.name, type, cls, 4 + name.readLen);
    }

    public byte[] GetBytes()
    {
        var res = new List<byte>();
        
        res.AddRange(Name.NameToBytes());
        res.AddRange(ByteHelper.GetBytes((ushort)Type));
        res.AddRange(ByteHelper.GetBytes(Class));
        
        return res.ToArray();
    }
}