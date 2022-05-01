using System;
using DNS.DnsPacket;
using FluentAssertions;
using NUnit.Framework;
using static System.Buffers.Binary.BinaryPrimitives;


namespace DnsServerTestProject;

public class HeadersShould
{
    [Test]
    public void FlagParse_in_query_should()
    {
        var headers = DnsHeadrsFlags.Parse(ReadInt16BigEndian(new Span<byte>(DnsTestData.Query, 2, 2)));

        headers.Type.Should().Be(QR.REQUEST);
        headers.RCode.Should().Be(DnsRCode.NOERROR);
        headers.Opcode.Should().Be(DnsOpcode.STANDART_QUERY);
    }

    [Test]
    public void FlagParse_in_response_should()
    {
        var headers = DnsHeadrsFlags.Parse(ReadInt16BigEndian(new Span<byte>(DnsTestData.Response, 2, 2)));

        headers.Type.Should().Be(QR.RESPONSE);
        headers.RCode.Should().Be(DnsRCode.NOERROR);
        headers.Opcode.Should().Be(DnsOpcode.STANDART_QUERY);
        headers.Flags.Should().HaveFlag(DnsHeadresBits.RD);
        headers.Flags.Should().HaveFlag(DnsHeadresBits.RA);
    }

    [Test]
    public void HeadersParse_in_query_should()
    {
        var actual = DnsMessageHeaders.Parse(DnsTestData.Query);
        var expected = new DnsMessageHeaders(3, new DnsHeadrsFlags(
            QR.REQUEST, DnsHeadresBits.RD, DnsOpcode.STANDART_QUERY, DnsRCode.NOERROR
        ), 1, 0, 0, 0);

        actual.Should().Be(expected);
    }

    [Test]
    public void HeadersParse_in_response_should()
    {
        var actual = DnsMessageHeaders.Parse(DnsTestData.Response);
        var expected = new DnsMessageHeaders(2, new DnsHeadrsFlags(
            QR.RESPONSE, DnsHeadresBits.RD | DnsHeadresBits.RA, DnsOpcode.STANDART_QUERY, DnsRCode.NOERROR
        ), 1, 1, 2, 0);

        actual.Should().Be(expected);
    }
}