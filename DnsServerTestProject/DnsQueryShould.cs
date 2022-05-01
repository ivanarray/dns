using System.Net;
using DNS.DnsPacket;
using FluentAssertions;
using NUnit.Framework;


namespace DnsServerTestProject;

public class DnsQueryShould
{
    [Test]
    public unsafe void QueryParse_should()
    {
        var expected = new DnsQuery(
            new[] { "e1", "ru" },
            QueryType.AAAA,
            1,
            11
        );
        fixed (byte* start = DnsTestData.Query)
        {
            var ptr = start + 12;
            var actual = DnsQuery.Parse(ptr, start);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}