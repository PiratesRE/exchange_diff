using System;

namespace Microsoft.Exchange.Assistants.Logging
{
	internal enum MailboxSlaEventType
	{
		StartProcessingMailbox,
		EndProcessingMailbox,
		MailboxInteresting,
		MailboxNotInteresting,
		FilterMailbox,
		SucceedOpenMailboxStoreSession,
		FailedOpenMailboxStoreSession,
		ErrorProcessingMailbox
	}
}
