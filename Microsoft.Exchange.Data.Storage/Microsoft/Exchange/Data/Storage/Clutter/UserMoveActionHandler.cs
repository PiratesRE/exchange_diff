using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UserMoveActionHandler
	{
		private UserMoveActionHandler(MailboxSession mailboxSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, IList<StoreObjectId> itemIds, bool isUserInitiatedMove)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("sourceFolderId", sourceFolderId);
			ArgumentValidator.ThrowIfNull("destinationFolderId", destinationFolderId);
			ArgumentValidator.ThrowIfNull("itemIds", itemIds);
			if (sourceFolderId.Equals(destinationFolderId))
			{
				throw new ArgumentException("sourceFolderId should not equal destinationFolderId");
			}
			this.mailboxSession = mailboxSession;
			this.sourceFolderId = sourceFolderId;
			this.destinationFolderId = destinationFolderId;
			this.itemIds = (from id in itemIds
			where id != null
			select id).Distinct<StoreObjectId>().ToList<StoreObjectId>();
			this.isUserInitiatedMove = isUserInitiatedMove;
			this.interpretedClutterAction = this.InterpretClutterAction();
		}

		public static bool TryCreate(MailboxSession mailboxSession, FolderChangeOperation operation, FolderChangeOperationFlags flags, GroupOperationResult result, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, out UserMoveActionHandler handler)
		{
			handler = null;
			if (operation == FolderChangeOperation.Move && result != null && result.OperationResult != OperationResult.Failed && result.ResultObjectIds != null && result.ResultObjectIds.Count > 0 && ClutterUtilities.IsClutterEnabled(mailboxSession, mailboxSession.MailboxOwner.GetConfiguration()) && sourceFolderId != null && destinationFolderId != null && !sourceFolderId.Equals(destinationFolderId))
			{
				bool flag = mailboxSession.LogonType == LogonType.Owner || flags.HasFlag(FolderChangeOperationFlags.ClutterActionByUserOverride);
				handler = new UserMoveActionHandler(mailboxSession, sourceFolderId, destinationFolderId, result.ResultObjectIds, flag);
				return true;
			}
			return false;
		}

		public void HandleUserMoves()
		{
			if (this.interpretedClutterAction == null)
			{
				return;
			}
			List<StoreObjectId> list = new List<StoreObjectId>();
			HashSet<ConversationId> hashSet = new HashSet<ConversationId>();
			foreach (StoreObjectId storeObjectId in this.itemIds)
			{
				using (Item item = Item.Bind(this.mailboxSession, storeObjectId, UserMoveActionHandler.itemPropertiesToLoad))
				{
					if (this.IsProcessingNecessary(item))
					{
						item.OpenAsReadWrite();
						item[InternalSchema.IsClutter] = this.interpretedClutterAction.Value;
						if (this.isUserInitiatedMove)
						{
							item[InternalSchema.IsClutterOverridden] = true;
							ConversationId valueOrDefault = item.GetValueOrDefault<ConversationId>(ItemSchema.ConversationId);
							if (valueOrDefault != null && !hashSet.Contains(valueOrDefault))
							{
								hashSet.Add(valueOrDefault);
							}
							list.Add(storeObjectId);
						}
						item.Save(SaveMode.ResolveConflicts);
					}
				}
			}
			if (this.mailboxSession.ActivitySession != null)
			{
				this.mailboxSession.ActivitySession.CaptureMarkAsClutterOrNotClutter(list.ToDictionary((StoreObjectId id) => id, (StoreObjectId id) => this.interpretedClutterAction.Value));
			}
			this.HandleFutureMessages(hashSet);
		}

		private bool? InterpretClutterAction()
		{
			StoreObjectId defaultFolderId = this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Clutter);
			if (this.destinationFolderId.Equals(defaultFolderId))
			{
				return new bool?(true);
			}
			if (!this.IsFolderClutternessAmbiguous(this.destinationFolderId) && (this.sourceFolderId.Equals(defaultFolderId) || this.IsFolderClutternessAmbiguous(this.sourceFolderId)))
			{
				return new bool?(false);
			}
			return null;
		}

		private bool IsProcessingNecessary(Item item)
		{
			return item.GetValueOrDefault<bool>(ItemSchema.IsClutter, false) ^ this.interpretedClutterAction.Value;
		}

		private void HandleFutureMessages(HashSet<ConversationId> affectedConversations)
		{
			foreach (ConversationId conversationId in affectedConversations)
			{
				Conversation conversation = Conversation.Load(this.mailboxSession, conversationId, UserMoveActionHandler.conversationPropertiesToLoad);
				if (!this.interpretedClutterAction.Value)
				{
					conversation.AlwaysClutterOrUnclutter(new bool?(false), false);
				}
				else if (this.IsAllConversationClutter(conversation))
				{
					conversation.AlwaysClutterOrUnclutter(new bool?(true), false);
				}
			}
		}

		private bool IsAllConversationClutter(Conversation conversation)
		{
			if (this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Clutter) == null)
			{
				return false;
			}
			foreach (IConversationTreeNode conversationTreeNode in conversation.ConversationTree)
			{
				for (int i = 0; i < conversationTreeNode.StorePropertyBags.Count; i++)
				{
					if (!conversationTreeNode.StorePropertyBags[i].GetValueOrDefault<bool>(ItemSchema.IsClutter, false))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool IsFolderClutternessAmbiguous(StoreObjectId folderId)
		{
			ArgumentValidator.ThrowIfNull("folderId", folderId);
			return folderId.Equals(this.mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems)) || folderId.Equals(this.mailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDeletions));
		}

		private static readonly PropertyDefinition[] itemPropertiesToLoad = new PropertyDefinition[]
		{
			ItemSchema.OriginalDeliveryFolderInfo
		};

		private static readonly PropertyDefinition[] conversationPropertiesToLoad = new PropertyDefinition[]
		{
			ItemSchema.IsClutter,
			MessageItemSchema.IsClutterOverridden
		};

		private readonly MailboxSession mailboxSession;

		private readonly StoreObjectId sourceFolderId;

		private readonly StoreObjectId destinationFolderId;

		private readonly IList<StoreObjectId> itemIds;

		private readonly bool? interpretedClutterAction;

		private readonly bool isUserInitiatedMove;
	}
}
