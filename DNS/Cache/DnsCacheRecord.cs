using DNS.DnsPacket;

namespace DNS.Cache;

// ReSharper disable once InconsistentNaming
public record DnsCacheRecord(DateTime Created, IReadOnlyList<string> Name, HashSet<DnsRRData> RRRecords);