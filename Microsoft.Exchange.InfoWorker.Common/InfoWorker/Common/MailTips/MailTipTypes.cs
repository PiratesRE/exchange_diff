using System;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	[Flags]
	public enum MailTipTypes
	{
		None = 0,
		AllUseThisForSerializationOnly = 1,
		OutOfOfficeMessage = 2,
		MailboxFullStatus = 4,
		CustomMailTip = 8,
		ExternalMemberCount = 16,
		TotalMemberCount = 32,
		MaxMessageSize = 64,
		DeliveryRestriction = 128,
		ModerationStatus = 256,
		InvalidRecipient = 512,
		Scope = 1024,
		All = 2046
	}
}
