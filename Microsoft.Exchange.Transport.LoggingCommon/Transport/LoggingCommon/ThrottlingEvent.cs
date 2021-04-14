using System;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal enum ThrottlingEvent
	{
		Throttle,
		Unthrottle,
		Warning,
		SummaryThrottle,
		Reset,
		SummaryWarning
	}
}
