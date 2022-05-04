using static System.Buffers.Binary.BinaryPrimitives;

namespace DNS.DnsPacket;

public record DnsMessageHeaders(
    ushort Id,
    DnsHeadrsFlags Flags,
    ushort QuestionCount,
    ushort AnswersCount,
    ushort AuthoritySectionCount,
    ushort AdditionalRecordSecCount)
{
    public static DnsMessageHeaders Parse(byte[] datagram)
    {
        var id = (ushort)ReadInt16BigEndian(new Span<byte>(datagram, 0, 2));
        var rawFlags = ReadInt16BigEndian(new Span<byte>(datagram, 2, 2));
        var flags = DnsHeadrsFlags.Parse(rawFlags);
        var queryCount = (ushort)ReadInt16BigEndian(new Span<byte>(datagram, 4, 2));
        var answerCount = (ushort)ReadInt16BigEndian(new Span<byte>(datagram, 6, 2));
        var authSecCount = (ushort)ReadInt16BigEndian(new Span<byte>(datagram, 8, 2));
        var additRecSecCount = (ushort)ReadInt16BigEndian(new Span<byte>(datagram, 10, 2));

        return new DnsMessageHeaders(id, flags,
            queryCount, answerCount,
            authSecCount, additRecSecCount);
    }

    public byte[] GetBytes()
    {
        var result = new List<byte>();
        result.AddRange(ByteHelper.GetBytes(Id));
        result.AddRange(ByteHelper.GetBytes(Flags.FlagsBytes));
        result.AddRange(ByteHelper.GetBytes(QuestionCount));
        result.AddRange(ByteHelper.GetBytes(AnswersCount));
        result.AddRange(ByteHelper.GetBytes(AuthoritySectionCount));
        result.AddRange(ByteHelper.GetBytes(AdditionalRecordSecCount));
        return result.ToArray();
    }
}