using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Conversations.Repositories
{
	internal static class XsoConversationRepositoryExtensions
	{
		public static List<IConversationTreeNode> GetTreeNodes(ICoreConversation conversation, byte[] syncState)
		{
			return XsoConversationRepositoryExtensions.CalculateTreeNodes(conversation, conversation.ConversationTree, syncState);
		}

		public static List<StoreObjectId> GetListStoreObjectIds(ICollection<IConversationTreeNode> treeNodes)
		{
			List<StoreObjectId> list = new List<StoreObjectId>();
			foreach (IConversationTreeNode conversationTreeNode in treeNodes)
			{
				list.AddRange(conversationTreeNode.ToListStoreObjectId());
			}
			return list;
		}

		public static bool IsValidSyncState(byte[] syncState)
		{
			return syncState != null && syncState.Length > 0;
		}

		public static IdAndSession GetSessionFromConversationId(IdConverter idConverter, BaseItemId conversationId, MailboxSearchLocation mailboxSearchLocation)
		{
			IdAndSession idAndSession = idConverter.ConvertConversationIdToIdAndSession(conversationId, mailboxSearchLocation == MailboxSearchLocation.ArchiveOnly);
			if (!(idAndSession.Session is MailboxSession))
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.ConversationSupportedOnlyForMailboxSession);
			}
			return idAndSession;
		}

		private static List<IConversationTreeNode> CalculateTreeNodes(ICoreConversation conversation, IConversationTree conversationTreeBase, byte[] syncState)
		{
			if (XsoConversationRepositoryExtensions.IsValidSyncState(syncState))
			{
				List<StoreObjectId> syncStateIds = XsoConversationRepositoryExtensions.GetSyncStateIds(conversation, syncState);
				return XsoConversationRepositoryExtensions.GetConversationTreeNodes(conversationTreeBase, syncStateIds);
			}
			return new List<IConversationTreeNode>(conversationTreeBase.ToList<IConversationTreeNode>());
		}

		private static List<StoreObjectId> GetSyncStateIds(ICoreConversation conversation, byte[] syncState)
		{
			KeyValuePair<List<StoreObjectId>, List<StoreObjectId>> keyValuePair = conversation.CalculateChanges(syncState);
			List<StoreObjectId> list = new List<StoreObjectId>();
			list.AddRange(keyValuePair.Key);
			list.AddRange(keyValuePair.Value);
			return list;
		}

		private static List<IConversationTreeNode> GetConversationTreeNodes(IConversationTree conversationTree, List<StoreObjectId> itemsToKeep)
		{
			List<IConversationTreeNode> list = new List<IConversationTreeNode>();
			foreach (StoreObjectId storeObjectId in itemsToKeep)
			{
				IConversationTreeNode item;
				if (conversationTree.TryGetConversationTreeNode(storeObjectId, out item))
				{
					list.Add(item);
				}
			}
			return list;
		}
	}
}
