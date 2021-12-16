using IpLookupProxy.Api.DataAccess.Models;

namespace IpLookupProxy.Api.Models;

public record IpInfoResult
{
    public IpInfoRecord? IpInfoRecord { get; init; }
    public TimeSpan WaitTime { get; set; }

    public bool IsSuccess => IpInfoRecord is not null;

    public static IpInfoResult Success(IpInfoRecord ipInfoRecord)
    {
        return new IpInfoResult { IpInfoRecord = ipInfoRecord };
    }

    public static IpInfoResult Fail(TimeSpan waitTime)
    {
        return new IpInfoResult { WaitTime = waitTime };
    }
}