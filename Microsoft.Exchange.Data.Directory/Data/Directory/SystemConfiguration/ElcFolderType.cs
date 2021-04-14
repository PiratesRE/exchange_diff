using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum ElcFolderType
	{
		[LocDescription(DirectoryStrings.IDs.Calendar)]
		Calendar = 1,
		[LocDescription(DirectoryStrings.IDs.Contacts)]
		Contacts,
		[LocDescription(DirectoryStrings.IDs.DeletedItems)]
		DeletedItems,
		[LocDescription(DirectoryStrings.IDs.Drafts)]
		Drafts,
		[LocDescription(DirectoryStrings.IDs.Inbox)]
		Inbox,
		[LocDescription(DirectoryStrings.IDs.JunkEmail)]
		JunkEmail,
		[LocDescription(DirectoryStrings.IDs.Journal)]
		Journal,
		[LocDescription(DirectoryStrings.IDs.Notes)]
		Notes,
		[LocDescription(DirectoryStrings.IDs.Outbox)]
		Outbox,
		[LocDescription(DirectoryStrings.IDs.SentItems)]
		SentItems,
		[LocDescription(DirectoryStrings.IDs.Tasks)]
		Tasks,
		[LocDescription(DirectoryStrings.IDs.All)]
		All,
		[LocDescription(DirectoryStrings.IDs.Organizational)]
		ManagedCustomFolder,
		[LocDescription(DirectoryStrings.IDs.RssSubscriptions)]
		RssSubscriptions,
		[LocDescription(DirectoryStrings.IDs.SyncIssues)]
		SyncIssues,
		[LocDescription(DirectoryStrings.IDs.ConversationHistory)]
		ConversationHistory,
		[LocDescription(DirectoryStrings.IDs.PersonalFolder)]
		Personal,
		[LocDescription(DirectoryStrings.IDs.DumpsterFolder)]
		RecoverableItems,
		[LocDescription(DirectoryStrings.IDs.NonIpmRoot)]
		NonIpmRoot,
		[LocDescription(DirectoryStrings.IDs.LegacyArchiveJournals)]
		LegacyArchiveJournals
	}
}
