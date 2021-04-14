using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal sealed class FolderUtility
	{
		public static ContentCountDisplay GetContentCountDisplay(object extendedFolderFlagValue, StoreObjectId folderId)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("FolderID");
			}
			if (extendedFolderFlagValue != null && extendedFolderFlagValue is ExtendedFolderFlags)
			{
				ExtendedFolderFlags valueToTest = (ExtendedFolderFlags)extendedFolderFlagValue;
				if (Utilities.IsFlagSet((int)valueToTest, 2))
				{
					return ContentCountDisplay.ItemCount;
				}
				if (Utilities.IsFlagSet((int)valueToTest, 1))
				{
					return ContentCountDisplay.UnreadCount;
				}
			}
			return FolderUtility.GetDefaultContentCountDisplay(folderId);
		}

		public static ContentCountDisplay GetDefaultContentCountDisplay(StoreObjectId folderId)
		{
			DefaultFolderType defaultFolderType = Utilities.GetDefaultFolderType(UserContextManager.GetUserContext().MailboxSession, folderId);
			if (defaultFolderType == DefaultFolderType.Root)
			{
				return ContentCountDisplay.None;
			}
			if (defaultFolderType == DefaultFolderType.Outbox || defaultFolderType == DefaultFolderType.Drafts || defaultFolderType == DefaultFolderType.JunkEmail)
			{
				return ContentCountDisplay.ItemCount;
			}
			return ContentCountDisplay.UnreadCount;
		}

		public static bool IsPrimaryMailFolder(StoreObjectId id, UserContext userContext)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return id.Equals(userContext.GetDeletedItemsFolderId(userContext.MailboxSession).StoreObjectId) || id.Equals(userContext.DraftsFolderId) || id.Equals(userContext.InboxFolderId) || id.Equals(userContext.JunkEmailFolderId) || id.Equals(userContext.SentItemsFolderId);
		}
	}
}
