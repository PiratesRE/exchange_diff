using System;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	public enum EasFolderType
	{
		UserGeneric = 1,
		Inbox,
		Drafts,
		DeletedItems,
		SentItems,
		Outbox,
		Tasks,
		Calendar,
		Contacts,
		Notes,
		Journal,
		UserMail,
		UserCalendar,
		UserContacts,
		UserTasks,
		UserJournal,
		UserNotes,
		Unknown,
		RecipientInfo,
		JunkEmail,
		Chats,
		SyntheticRoot = 200,
		SyntheticIpmSubtree,
		OutOfRange = 255
	}
}
