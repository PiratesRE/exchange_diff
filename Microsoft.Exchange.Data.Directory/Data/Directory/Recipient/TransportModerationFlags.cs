using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	internal enum TransportModerationFlags
	{
		None = 0,
		BypassNestedModerationEnabled = 1,
		SendModerationNotificationsInternally = 2,
		SendModerationNotificationsExternally = 4
	}
}
