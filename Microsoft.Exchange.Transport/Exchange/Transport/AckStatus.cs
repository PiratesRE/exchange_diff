using System;

namespace Microsoft.Exchange.Transport
{
	internal enum AckStatus
	{
		Pending,
		Success,
		Retry,
		Fail,
		Expand,
		Relay,
		SuccessNoDsn,
		Resubmit,
		Quarantine,
		Skip
	}
}
