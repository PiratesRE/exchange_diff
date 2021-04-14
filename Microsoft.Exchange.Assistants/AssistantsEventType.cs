using System;

namespace Microsoft.Exchange.Assistants
{
	internal enum AssistantsEventType
	{
		ServiceStarted,
		ServiceStopped,
		SuspendActivity,
		EndActivity,
		StartProcessingMailbox,
		EndProcessingMailbox,
		FilterMailbox,
		ErrorProcessingMailbox,
		SucceedOpenMailboxStoreSession,
		FailedOpenMailboxStoreSession,
		MailboxInteresting,
		MailboxNotInteresting,
		FolderSyncOperation,
		FolderSyncException,
		ReceivedQueriedMailboxes,
		EndGetMailboxes,
		NoMailboxes,
		NotStarted,
		NoJobs,
		BeginJob,
		EndJob,
		DriverNotStarted,
		JobAlreadyRunning,
		ErrorEnumeratingMailbox
	}
}
