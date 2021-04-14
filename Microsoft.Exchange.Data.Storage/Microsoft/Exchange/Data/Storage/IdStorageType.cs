using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum IdStorageType : byte
	{
		MailboxItemSmtpAddressBased,
		PublicFolder,
		PublicFolderItem,
		MailboxItemMailboxGuidBased,
		ConversationIdMailboxGuidBased,
		ActiveDirectoryObject
	}
}
