using System;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Flags]
	public enum VirusScanFlags
	{
		IsOutbound = 1,
		AllowUserOptOut = 2,
		SenderNotification = 4,
		OutboundSenderNotification = 8,
		RecipientNotification = 16,
		AdminNotification = 32,
		OutboundAdminNotification = 64
	}
}
