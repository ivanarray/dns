﻿using static System.Buffers.Binary.BinaryPrimitives;

namespace DNS.DnsPacket;

// ReSharper disable InconsistentNaming
public record DnsRRData(
    IReadOnlyList<string> Name,
    QueryType Type,
    ushort Class,
    int TTL,
    ushort RDLength,
    byte[] RData,
    int ReadBytes)
{
    public static unsafe DnsRRData Parse(byte* pointer, byte* startDatagram)
    {
        var name = NameParser.ParseName(pointer, startDatagram);
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

        return new DnsRRData(name.name, type, cls, ttl, dataLen, data.ToArray(), 10 + name.readLen + dataLen);
    }
}