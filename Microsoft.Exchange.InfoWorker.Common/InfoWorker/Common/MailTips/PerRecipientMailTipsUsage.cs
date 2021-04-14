using System;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	[Flags]
	public enum PerRecipientMailTipsUsage
	{
		None = 0,
		AutoReply = 1,
		MailboxFull = 2,
		CustomMailTip = 4,
		External = 8,
		Restricted = 16,
		Moderated = 32,
		Invalid = 64,
		Scope = 128
	}
}
