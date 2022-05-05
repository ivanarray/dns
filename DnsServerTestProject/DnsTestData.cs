using System;
using DNS.DnsPacket;

namespace DnsServerTestProject;

public static class DnsTestData
{
    public static readonly byte[] Query =
    {
        0, 0x03, 0x01, 0, 00, 0x01, 0, 0, 0, 0, 0, 0, 0x02, 0x65, 0x31, 0x02, 0x72, 0x75, 0, 0, 0x1c, 0, 0x01
    };

    public static readonly byte[] Response =
    {
        0, 0x02, 0x81, 0x80, 0, 0x01, 0, 0x01, 0, 0x02, 0, 0, 0x02, 0x65, 0x31, 0x02, 0x72, 0x75, 0, 0, 0x01, 0,
        0x01, 0xc0, 0x0c, 0, 0x01, 0, 0x01, 0, 0, 0x01, 0x17, 0, 0x04, 0xc3, 0x13, 0xdc, 0x18, 0xc0, 0x0c,
        0, 0x02, 0, 0x01, 0, 0, 0, 0xb1, 0, 0x0b, 0x02, 0x6e, 0x73, 0x05, 0x68, 0x73, 0x64, 0x72, 0x6e, 0xc0, 0x0f,
        0xc0, 0x0c, 0, 0x02, 0, 0x01, 0, 0, 0, 0xb1, 0, 0x0a, 0x03, 0x6e, 0x73, 0x31, 0x03, 0x6e, 0x67, 0x73, 0xc0, 0x0f
    };

    public static readonly DnsMessageHeaders RequestHeaders = new(3, new DnsHeadrsFlags(
        QR.REQUEST, DnsHeadresBits.RD, DnsOpcode.STANDART_QUERY, DnsRCode.NOERROR
    ), 1, 0, 0, 0);

    public static readonly DnsMessageHeaders ResponseHeaders = new(2, new DnsHeadrsFlags(
        QR.RESPONSE, DnsHeadresBits.RD | DnsHeadresBits.RA, DnsOpcode.STANDART_QUERY, DnsRCode.NOERROR
    ), 1, 1, 2, 0);

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once IdentifierTypo
    public static readonly DnsQuery RequestAAAA = new(
        new[] { "e1", "ru" },
        QueryType.AAAA,
        1,
        11
    );

    public static readonly DnsQuery RequestA = new(
        new[] { "e1", "ru" },
        QueryType.A,
        1,
        11
    );

    public static readonly DnsRRData Answer = new(
        new[] { "e1", "ru" },
        QueryType.A,
        1, 279, 4,
        new byte[] { 0xc3, 0x13, 0xdc, 0x18 },
        DateTime.Now,
        16);

    public static readonly DnsRRData Ns1 = new(
        new[] { "e1", "ru" },
        QueryType.NS,
        1, 177, 11,
        new byte[] { 0x02, 0x6e, 0x73, 0x05, 0x68, 0x73, 0x64, 0x72, 0x6e, 0xc0, 0x0f },
        DateTime.Now
        , 23);

    public static readonly DnsRRData Ns2 = new(
        new[] { "e1", "ru" },
        QueryType.NS,
        1, 177, 10,
        new byte[] { 0x03, 0x6e, 0x73, 0x31, 0x03, 0x6e, 0x67, 0x73, 0xc0, 0x0f },
        DateTime.Now
        , 22);
}