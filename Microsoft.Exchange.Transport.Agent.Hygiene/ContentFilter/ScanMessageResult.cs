using System;

namespace Microsoft.Exchange.Transport.Agent.ContentFilter
{
	internal enum ScanMessageResult : uint
	{
		Error,
		OK,
		Pending,
		FilterNotInitialized,
		UnableToProcessMessage,
		Timedout = 4294967294U,
		Shutdown
	}
}
