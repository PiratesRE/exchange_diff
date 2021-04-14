using System;

namespace Microsoft.Exchange.Data.Mapi
{
	public enum StorageLimitStatus
	{
		BelowLimit = 1,
		IssueWarning,
		ProhibitSend = 4,
		NoChecking = 8,
		MailboxDisabled = 16
	}
}
