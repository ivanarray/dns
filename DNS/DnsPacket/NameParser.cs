using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace DNS.DnsPacket;

public static unsafe class NameParser
{
    private const byte MarkTypeMask = 0b11000000;
    private const byte LinkMask = 0b00111111;

    public static (IReadOnlyList<string> name, int readLen) ParseName(byte* pointer, byte* startDatagram)
    {
        var result = new List<string>();
        var readLen = 1;
        while (true)
        {
            var mark = *pointer & MarkTypeMask;
            switch (mark)
            {
                case 0:
                {
                    var count = *pointer;
                    if (count == 0) return (result.ToArray(), readLen);
                    result.Add(Encoding.ASCII.GetString(++pointer, count));
                    readLen += 1 + count;
                    pointer += count;
                    continue;
                }
                case 192:
                {
                    var offset = (ushort)ReadInt16BigEndian(new[] { (byte)(*pointer & LinkMask), *++pointer });
                    readLen++;
                    var ptr = startDatagram + offset;
                    var pm = ParseName(ptr, startDatagram);
                    result.AddRange(pm.name);
                    return (result, readLen);
                }
            }

            throw new Exception($"Неизвестная метка {mark:X}");
        }
    }
}