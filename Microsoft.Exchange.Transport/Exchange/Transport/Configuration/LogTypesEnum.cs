using System;

namespace Microsoft.Exchange.Transport.Configuration
{
	[Flags]
	internal enum LogTypesEnum : long
	{
		None = 0L,
		InboxRules = 1L,
		TransportRules = 2L,
		Arbitration = 4L
	}
}
