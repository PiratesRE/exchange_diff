using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[Flags]
	[Serializable]
	public enum AggregationSubscriptionType
	{
		Unknown = 0,
		Pop = 2,
		DeltaSyncMail = 4,
		IMAP = 16,
		AllEMail = 22,
		Facebook = 32,
		LinkedIn = 64,
		AllThatSupportSendAs = 22,
		AllThatSupportPolicyInducedDeletion = 96,
		AllThatSupportSendAsAndPeopleConnect = 118,
		All = 255
	}
}
