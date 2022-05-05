using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using DNS.Cache;
using DNS.DnsPacket;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DnsServerTestProject;

public class SerializationShould
{
    private static readonly DnsMessage Message = DnsMessage.Parse(DnsTestData.Response);
    private static readonly JsonSerializer Serializer = new JsonSerializer();
    private StringWriter writer = new();

    [SetUp]
    public void RefreshWriter() => writer = new();

    [Test]
    public void CacheRecord_should_be_serializable()
    {
        var rec = new DnsCacheRecord(DateTime.Now, Message.Queries[0].Name, Message.RData.ToHashSet());

        Serializer.Serialize(writer, rec);
        var s = writer.ToString();
        var reader = new JsonTextReader(new StringReader(s));

        var actual = Serializer.Deserialize<DnsCacheRecord>(reader);

        actual.Should().BeEquivalentTo(rec);
    }

    [Test]
    public void DnsRrData_should_be_serializable()
    {
        Serializer.Serialize(writer, Message.RData[0]);
        var reader = new JsonTextReader(new StringReader(writer.ToString()));

        var actual = Serializer.Deserialize<DnsRRData>(reader);

        actual.Should().BeEquivalentTo(Message.RData[0]);
    }

    [Test]
    [Ignore("")]
    public void DnsCache_should_be_serializable()
    {
        var cache = new DnsCache(new ConcurrentDictionary<string, DnsCacheRecord>());

        foreach (var data in Message.RData)
        {
            cache.Add(Message.Queries[0].Name, data);
        }

        Serializer.Serialize(writer, cache);

        var s = writer.ToString();
        var reader = new JsonTextReader(new StringReader(s));
        var actual = Serializer.Deserialize<DnsCache>(reader);

        actual.Should().BeEquivalentTo(cache);
    }
}