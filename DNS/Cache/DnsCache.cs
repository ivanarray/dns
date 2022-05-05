using System.Collections.Concurrent;
using DNS.DnsPacket;
using Newtonsoft.Json;

namespace DNS.Cache;

[JsonObject(MemberSerialization.OptIn)]
public class DnsCache : IDisposable
{
    [JsonProperty] private readonly ConcurrentDictionary<string, DnsCacheRecord> dictionary;
    [JsonIgnore] private bool isDisposed;

    public static DnsCache Load()
    {
        var serializer = new JsonSerializer();
        using var fileStream = File.Open("./cache.json", FileMode.OpenOrCreate);
        using var textReader = new StreamReader(fileStream);
        using var jsonReader = new JsonTextReader(textReader);
        var result = serializer.Deserialize<DnsCache>(jsonReader);
        result?.CleanOldRecords();
        return result ?? new DnsCache(new ConcurrentDictionary<string, DnsCacheRecord>());
    }

    public DnsCache(ConcurrentDictionary<string, DnsCacheRecord> dictionary)
    {
        this.dictionary = dictionary;
    }

    public void Add(IReadOnlyList<string> name, DnsRRData data)
    {
        var key = string.Concat(name);
        if (!dictionary.ContainsKey(key))
        {
            dictionary[key] = new DnsCacheRecord(DateTime.Now, name, new HashSet<DnsRRData>());
        }

        dictionary[key].RRRecords.Add(data);
    }

    public void Add(IReadOnlyList<string> name, IEnumerable<DnsRRData> data)
    {
        var key = string.Concat(name);
        if (!dictionary.ContainsKey(key))
        {
            dictionary[key] = new DnsCacheRecord(DateTime.Now, name, new HashSet<DnsRRData>());
        }

        dictionary[key].RRRecords.UnionWith(data);
    }


    public bool Contains(IReadOnlyList<string> name, QueryType queryType)
    {
        var key = string.Concat(name);
        return dictionary.ContainsKey(key) && dictionary[key].RRRecords.Any(x => x.Type == queryType);
    }

    public IReadOnlyList<DnsRRData> Get(IReadOnlyList<string> name, QueryType queryType) =>
        dictionary[string.Concat(name)].RRRecords.Where(x => x.Type == queryType).ToArray();

    public void Dispose()
    {
        if (isDisposed) return;
        isDisposed = true;
        using var fileStream = File.Open("./cache.json", FileMode.OpenOrCreate);
        using var textWriter = new StreamWriter(fileStream);
        using var jsonWriter = new JsonTextWriter(textWriter);
        new JsonSerializer().Serialize(jsonWriter, this);
    }

    public void CleanOldRecords()
    {
        foreach (var pair in dictionary)
        {
            foreach (var r in pair.Value.RRRecords.Where(r => !r.IsValidData))
            {
                pair.Value.RRRecords.Remove(r);
            }

            if (pair.Value.RRRecords.Count == 0) dictionary.TryRemove(pair.Key, out var a);
        }
    }
}