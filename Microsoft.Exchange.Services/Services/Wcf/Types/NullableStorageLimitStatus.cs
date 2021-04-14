using System;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	public enum NullableStorageLimitStatus
	{
		NullStorageLimitStatus = -1,
		BelowLimit = 1,
		IssueWarning,
		ProhibitSend = 4,
		NoChecking = 8,
		MailboxDisabled = 16
	}
}
