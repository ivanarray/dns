namespace DNS.DnsPacket;

public class DnsMessage
{
    public readonly DnsMessageHeaders Headers;
    public readonly IReadOnlyList<DnsQuery> Queries;
    public readonly IReadOnlyList<DnsRRData> RData;

    public DnsMessage(DnsMessageHeaders headers, IReadOnlyList<DnsQuery> queries, IReadOnlyList<DnsRRData> rData)
    {
        Headers = headers;
        Queries = queries;
        RData = rData;
    }

    public static unsafe DnsMessage Parse(byte[] datagram)
    {
        var headers = DnsMessageHeaders.Parse(datagram);
        var index = 12;
        var queries = new List<DnsQuery>();
        var answers = new List<DnsRRData>();
        fixed (byte* start = datagram)
        {
            var ptr = start + index;

            for (var i = 0; i < headers.QueryCount; i++)
            {
                var query = DnsQuery.Parse(ptr, start);
                ptr += query.ReadBytes;
                queries.Add(query);
            }

            var rrCount = headers.AnswersCount + headers.AuthoritySectionCount + headers.AdditionalRecordSecCount;

            for (var i = 0; i < rrCount; i++)
            {
                var rr = DnsRRData.Parse(ptr, start);
                ptr += rr.ReadBytes;
                answers.Add(rr);
            }
        }

        return new DnsMessage(headers, queries, answers);
    }
}