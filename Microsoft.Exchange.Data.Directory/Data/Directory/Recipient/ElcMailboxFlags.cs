using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum ElcMailboxFlags
	{
		None = 0,
		ExpirationSuspended = 1,
		ElcV2 = 2,
		DisableCalendarLogging = 4,
		LitigationHold = 8,
		SingleItemRecovery = 16,
		ValidArchiveDatabase = 32,
		ShouldUseDefaultRetentionPolicy = 128,
		EnableSiteMailboxMessageDedup = 256
	}
}
