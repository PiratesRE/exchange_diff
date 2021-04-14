using System;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	[Flags]
	internal enum JournalReportTags
	{
		None = 0,
		Sender = 1,
		MessageId = 2,
		Recipients = 4
	}
}
