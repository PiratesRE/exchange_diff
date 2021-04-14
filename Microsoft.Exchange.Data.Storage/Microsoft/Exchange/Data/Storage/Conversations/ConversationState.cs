using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConversationState
	{
		internal ConversationState(IMailboxSession session, IConversationTree conversationTree, ICollection<IConversationTreeNode> nodesToExclude)
		{
			this.conversationTree = conversationTree;
			this.session = session;
			this.treeNode = new MiniStateTreeNode(conversationTree, new Func<StoreObjectId, long>(this.StoreIdToIdHash), nodesToExclude);
			this.InitializeSerializedState();
		}

		internal byte[] SerializedState
		{
			get
			{
				return this.serializedState;
			}
		}

		internal KeyValuePair<List<StoreObjectId>, List<StoreObjectId>> CalculateChanges(byte[] otherSerializedState)
		{
			Util.ThrowOnNullOrEmptyArgument(otherSerializedState, "otherSerializedState");
			MiniStateTreeNode right = MiniStateTreeNode.DeSerialize(otherSerializedState);
			KeyValuePair<List<long>, List<long>> affectedIds = this.treeNode.GetAffectedIds(right);
			KeyValuePair<List<StoreObjectId>, List<StoreObjectId>> result = new KeyValuePair<List<StoreObjectId>, List<StoreObjectId>>(new List<StoreObjectId>(affectedIds.Key.Count), new List<StoreObjectId>(affectedIds.Value.Count));
			for (int i = 0; i < affectedIds.Key.Count; i++)
			{
				result.Key.Add(this.hashToIdMap[affectedIds.Key[i]]);
			}
			for (int j = 0; j < affectedIds.Value.Count; j++)
			{
				result.Value.Add(this.hashToIdMap[affectedIds.Value[j]]);
			}
			foreach (IConversationTreeNode conversationTreeNode in this.conversationTree)
			{
				ConversationTreeNode conversationTreeNode2 = (ConversationTreeNode)conversationTreeNode;
				foreach (StoreObjectId storeObjectId in conversationTreeNode2.ToListStoreObjectId())
				{
					bool valueOrDefault = conversationTreeNode2.GetValueOrDefault<bool>(storeObjectId, MessageItemSchema.IsDraft, false);
					if (valueOrDefault && !result.Key.Contains(storeObjectId) && !result.Value.Contains(storeObjectId))
					{
						result.Value.Add(storeObjectId);
					}
				}
			}
			return result;
		}

		private long StoreIdToIdHash(StoreObjectId id)
		{
			long midFromMessageId = this.session.IdConverter.GetMidFromMessageId(id);
			try
			{
				this.hashToIdMap.Add(midFromMessageId, id);
			}
			catch (ArgumentException)
			{
				throw new StoragePermanentException(ServerStrings.ConversationContainsDuplicateMids(this.session.MailboxOwner.LegacyDn, midFromMessageId));
			}
			return midFromMessageId;
		}

		private void InitializeSerializedState()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					this.treeNode.Serialize(binaryWriter, 0);
					this.serializedState = memoryStream.ToArray();
				}
			}
		}

		private readonly IMailboxSession session;

		private IConversationTree conversationTree;

		private MiniStateTreeNode treeNode;

		private Dictionary<long, StoreObjectId> hashToIdMap = new Dictionary<long, StoreObjectId>();

		private byte[] serializedState;
	}
}
