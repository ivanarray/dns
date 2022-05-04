using DNS.DnsPacket;
using FluentAssertions;
using NUnit.Framework;

namespace DnsServerTestProject;

public class NameParseShould
{
    [Test]
    public unsafe void NameParse_when_no_jumps_should()
    {
        var startOfQuery = 12;
        fixed (byte* start = DnsTestData.Query)
        {
            var ptr = start + startOfQuery;
            var actual = ByteHelper.ParseName(ptr, start);

            actual.name.Should().BeEquivalentTo("e1", "ru");
            actual.readLen.Should().Be(7);
        }
    }

    [Test]
    public unsafe void NameParse_when_has_jumps_should()
    {
        var startOfResponse = 23;
        fixed (byte* start = DnsTestData.Response)
        {
            var ptr = start + startOfResponse;
            var actual = ByteHelper.ParseName(ptr, start);

            actual.name.Should().BeEquivalentTo("e1", "ru");
            actual.readLen.Should().Be(2);
        }
    }
}