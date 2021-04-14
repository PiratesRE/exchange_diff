using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	public enum RequestStatus
	{
		[LocDescription(DirectoryStrings.IDs.MailboxMoveStatusNone)]
		None,
		[LocDescription(DirectoryStrings.IDs.MailboxMoveStatusQueued)]
		Queued,
		[LocDescription(DirectoryStrings.IDs.MailboxMoveStatusInProgress)]
		InProgress,
		[LocDescription(DirectoryStrings.IDs.MailboxMoveStatusAutoSuspended)]
		AutoSuspended,
		[LocDescription(DirectoryStrings.IDs.MailboxMoveStatusCompletionInProgress)]
		CompletionInProgress,
		[LocDescription(DirectoryStrings.IDs.MailboxMoveStatusSynced)]
		Synced,
		[LocDescription(DirectoryStrings.IDs.MailboxMoveStatusCompleted)]
		Completed = 10,
		[LocDescription(DirectoryStrings.IDs.MailboxMoveStatusCompletedWithWarning)]
		CompletedWithWarning,
		[LocDescription(DirectoryStrings.IDs.MailboxMoveStatusSuspended)]
		Suspended = 98,
		[LocDescription(DirectoryStrings.IDs.MailboxMoveStatusFailed)]
		Failed
	}
}
