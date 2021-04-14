using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public enum SpecialFolders : short
	{
		Regular,
		MailboxRoot,
		Finder,
		Views,
		CommonViews,
		Schedule,
		Shortcuts,
		DeferredAction,
		SpoolerQueue,
		TopofInformationStore,
		Inbox,
		Outbox,
		SentItems,
		DeletedItems,
		Calendar,
		Contacts,
		Drafts,
		Tasks,
		Notes,
		Journal,
		Conversations,
		MaterializedRestrictionRoot,
		NumberOfSpecialFolders
	}
}
