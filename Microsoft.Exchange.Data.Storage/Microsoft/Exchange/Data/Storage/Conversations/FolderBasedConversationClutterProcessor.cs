using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderBasedConversationClutterProcessor : IConversationClutterProcessor
	{
		public FolderBasedConversationClutterProcessor(MailboxSession mailboxSession)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			this.mailboxSession = mailboxSession;
		}

		public int Process(bool markAsClutter, IConversationTree conversationTree, List<GroupOperationResult> results)
		{
			ArgumentValidator.ThrowIfNull("results", results);
			int num = 0;
			StoreObjectId storeObjectId = markAsClutter ? this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox) : this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Clutter);
			StoreObjectId storeObjectId2 = markAsClutter ? this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Clutter) : this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			if (storeObjectId == null || storeObjectId2 == null)
			{
				return num;
			}
			List<StoreObjectId> list = new List<StoreObjectId>();
			foreach (IConversationTreeNode conversationTreeNode in conversationTree)
			{
				for (int i = 0; i < conversationTreeNode.StorePropertyBags.Count; i++)
				{
					StoreId id = conversationTreeNode.StorePropertyBags[i].TryGetProperty(ItemSchema.Id) as StoreId;
					if (storeObjectId.Equals(conversationTreeNode.StorePropertyBags[i].TryGetProperty(StoreObjectSchema.ParentItemId) as StoreObjectId))
					{
						list.Add(StoreId.GetStoreObjectId(id));
					}
				}
			}
			if (list.Count > 0)
			{
				try
				{
					using (Folder folder = Folder.Bind(this.mailboxSession, storeObjectId))
					{
						using (Folder folder2 = Folder.Bind(this.mailboxSession, storeObjectId2))
						{
							GroupOperationResult groupOperationResult = folder.MoveItems(folder2.Id, list.ToArray());
							results.Add(groupOperationResult);
							if (groupOperationResult.OperationResult != OperationResult.Failed)
							{
								num += groupOperationResult.ObjectIds.Count;
							}
						}
					}
				}
				catch (LocalizedException storageException)
				{
					results.Add(new GroupOperationResult(OperationResult.Failed, list, storageException));
				}
			}
			return num;
		}

		private readonly MailboxSession mailboxSession;
	}
}
