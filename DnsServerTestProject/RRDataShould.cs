using DNS.DnsPacket;
using FluentAssertions;
using NUnit.Framework;

namespace DnsServerTestProject;

// ReSharper disable once InconsistentNaming
public class RRDataShould
{
    [Test]
    public unsafe void RRDataParse_should()
    {
        var expected = new DnsRRData(
            new[] { "e1", "ru" },
            QueryType.A,
            1, 279, 4,
            new byte[] { 0xc3, 0x13, 0xdc, 0x18 },
            16);

        fixed (byte* startDatagram = DnsTestData.Response)
        {
            var start = startDatagram + 23;
            var actual = DnsRRData.Parse(start, startDatagram);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}