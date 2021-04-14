using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupJoinRequestMessage
	{
		internal static void SendMessage(MailboxSession mailboxSession, ADUser groupAdUser, string attachedMessageBody)
		{
			ArgumentValidator.ThrowIfNull("groupMailbox", mailboxSession);
			ArgumentValidator.ThrowIfNull("groupAdUser", groupAdUser);
			ArgumentValidator.ThrowIfNull("attachedMessageBody", attachedMessageBody);
			StoreObjectId storeObjectId = mailboxSession.GetDefaultFolderId(DefaultFolderType.TemporarySaves);
			if (storeObjectId == null)
			{
				storeObjectId = mailboxSession.CreateDefaultFolder(DefaultFolderType.TemporarySaves);
			}
			using (GroupMailboxJoinRequestMessageItem groupMailboxJoinRequestMessageItem = GroupMailboxJoinRequestMessageItem.Create(mailboxSession, storeObjectId))
			{
				new GroupJoinRequestMessageComposer(mailboxSession, groupAdUser, attachedMessageBody).WriteToMessage(groupMailboxJoinRequestMessageItem);
				groupMailboxJoinRequestMessageItem.Send();
			}
		}
	}
}
