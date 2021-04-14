using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum UMMailboxPolicyEnabledFlags
	{
		None = 0,
		AllowMissedCallNotifications = 1,
		AllowSMSMessageWaitingNotifications = 2,
		AllowVirtualNumber = 4,
		AllowPinlessVoiceMailAccess = 8,
		AllowVoiceMailAnalysis = 16
	}
}
