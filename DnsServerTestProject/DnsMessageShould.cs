using System;
using DNS.DnsPacket;
using FluentAssertions;
using NUnit.Framework;

namespace DnsServerTestProject;

public class DnsMessageShould
{
    [Test]
    public void DnsMessage_request_should()
    {
        var expected = new DnsMessage(DnsTestData.RequestHeaders, new[] { DnsTestData.RequestAAAA },
            ArraySegment<DnsRRData>.Empty);

        var actual = DnsMessage.Parse(DnsTestData.Query);

        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void DnsMessage_response_should()
    {
        var expected = new DnsMessage(DnsTestData.ResponseHeaders, new[] { DnsTestData.RequestA},
            new[] { DnsTestData.Answer, DnsTestData.Ns1, DnsTestData.Ns2 });

        var actual = DnsMessage.Parse(DnsTestData.Response);

        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void DnsMessage_getBytes_should()
    {
        var actual = DnsMessage.Parse(DnsTestData.Query).GetBytes();

        actual.Should().BeEquivalentTo(DnsTestData.Query);
    }
}