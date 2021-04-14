using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum TransportModerationNotificationFlags
	{
		Never = 0,
		Internal = 1,
		Always = 3
	}
}
