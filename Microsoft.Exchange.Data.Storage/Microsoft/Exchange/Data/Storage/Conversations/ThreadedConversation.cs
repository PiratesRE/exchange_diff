using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ThreadedConversation : IThreadedConversation, ICoreConversation
	{
		internal ThreadedConversation(ConversationStateFactory stateFactory, ConversationDataExtractor dataExtractor, ConversationId conversationId, IConversationTree conversationTree, IList<IConversationThread> conversationThreads)
		{
			this.conversationTree = conversationTree;
			this.conversationId = conversationId;
			this.dataExtractor = dataExtractor;
			this.conversationThreads = conversationThreads;
			this.stateFactory = stateFactory;
		}

		public event OnBeforeItemLoadEventDelegate OnBeforeItemLoad
		{
			add
			{
				this.dataExtractor.OnBeforeItemLoad += value;
			}
			remove
			{
				this.dataExtractor.OnBeforeItemLoad -= value;
			}
		}

		public ConversationId ConversationId
		{
			get
			{
				return this.conversationId;
			}
		}

		public IConversationTreeNode RootMessageNode
		{
			get
			{
				return this.conversationTree.RootMessageNode;
			}
		}

		public IConversationTree ConversationTree
		{
			get
			{
				return this.conversationTree;
			}
		}

		public StoreObjectId RootMessageId
		{
			get
			{
				return this.conversationTree.RootMessageId;
			}
		}

		public string Topic
		{
			get
			{
				return this.ConversationTree.Topic;
			}
		}

		public byte[] SerializedTreeState
		{
			get
			{
				return this.ConversationState.SerializedState;
			}
		}

		public IEnumerable<IConversationThread> Threads
		{
			get
			{
				return this.conversationThreads;
			}
		}

		private ConversationState ConversationState
		{
			get
			{
				if (this.conversationState == null)
				{
					this.conversationState = this.stateFactory.Create(null);
				}
				return this.conversationState;
			}
		}

		public IConversationStatistics ConversationStatistics
		{
			get
			{
				return this.dataExtractor.OptimizationCounters;
			}
		}

		public ItemPart GetItemPart(StoreObjectId itemId)
		{
			return this.dataExtractor.GetItemPart(this.conversationTree, itemId);
		}

		public ParticipantSet AllParticipants(ICollection<IConversationTreeNode> loadedNodes = null)
		{
			return this.dataExtractor.CalculateAllRecipients(this.ConversationTree, loadedNodes);
		}

		public byte[] GetSerializedTreeStateWithNodesToExclude(ICollection<IConversationTreeNode> nodesToExclude)
		{
			ConversationState conversationState = this.stateFactory.Create(nodesToExclude);
			return conversationState.SerializedState;
		}

		public void LoadBodySummaries()
		{
			this.dataExtractor.LoadBodySummaries(this.conversationTree);
			this.SyncThreads();
		}

		public void LoadItemParts(ICollection<IConversationTreeNode> nodes)
		{
			this.LoadItemParts(from n in nodes
			select n.MainStoreObjectId);
		}

		public List<StoreObjectId> GetMessageIdsForPreread()
		{
			return this.dataExtractor.GetMessageIdsForPreread(this.ConversationTree);
		}

		public void LoadItemParts(ICollection<StoreObjectId> storeIds)
		{
			this.dataExtractor.LoadItemParts(this.conversationTree, storeIds);
			this.SyncThreads();
		}

		public KeyValuePair<List<StoreObjectId>, List<StoreObjectId>> CalculateChanges(byte[] olderState)
		{
			return this.ConversationState.CalculateChanges(olderState);
		}

		private void SyncThreads()
		{
			foreach (IConversationThread conversationThread in this.conversationThreads)
			{
				conversationThread.SyncThread();
			}
		}

		private readonly ConversationDataExtractor dataExtractor;

		private readonly ConversationStateFactory stateFactory;

		private readonly ConversationId conversationId;

		private ConversationState conversationState;

		private IConversationTree conversationTree;

		private IList<IConversationThread> conversationThreads;
	}
}
