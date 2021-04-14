using System;

namespace Microsoft.Exchange.Net
{
	[Flags]
	internal enum DnsQueryOptions
	{
		None = 0,
		AcceptTruncatedResponse = 1,
		UseTcpOnly = 2,
		NoRecursion = 4,
		BypassCache = 8,
		FailureTolerant = 16
	}
}
