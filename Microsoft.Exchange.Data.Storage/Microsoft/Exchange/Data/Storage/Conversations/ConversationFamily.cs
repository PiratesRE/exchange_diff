using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationFamily : IBreadcrumbsSource, IConversation, ICoreConversation, IConversationData
	{
		internal ConversationFamily(IMailboxSession mailboxSession, ConversationDataExtractor dataExtractor, ConversationId conversationId, IConversationTree conversationTree, IConversationTreeFactory selectedConversationTreeFactory) : this(mailboxSession, dataExtractor, conversationId, conversationTree, conversationId, conversationTree, selectedConversationTreeFactory)
		{
			this.isSingleConversationFamily = true;
		}

		internal ConversationFamily(IMailboxSession mailboxSession, ConversationDataExtractor dataExtractor, ConversationId conversationFamilyId, IConversationTree conversationFamilyTree, ConversationId selectedConversationId, IConversationTree selectedConversationTree, IConversationTreeFactory selectedConversationTreeFactory)
		{
			this.conversationFamilyTree = conversationFamilyTree;
			this.selectedConversationTree = selectedConversationTree;
			this.conversationFamilyId = conversationFamilyId;
			this.dataExtractor = dataExtractor;
			this.mailboxSession = mailboxSession;
			this.selectedConversationId = selectedConversationId;
			this.selectedConversationTreeFactory = selectedConversationTreeFactory;
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

		public IStorePropertyBag BackwardMessagePropertyBag
		{
			get
			{
				if (this.backwardMessagePropertyBag == null)
				{
					this.backwardMessagePropertyBag = this.CalculateBackwardPropertyBag(this.selectedConversationTree.RootMessageNode);
				}
				return this.backwardMessagePropertyBag;
			}
		}

		public Dictionary<StoreObjectId, List<IStorePropertyBag>> ForwardConversationRootMessagePropertyBags
		{
			get
			{
				if (this.forwardConversationRootMessagePropertyBags == null)
				{
					this.forwardConversationRootMessagePropertyBags = this.CalculateForwardMessagePropertyBags(this.selectedConversationTree);
				}
				return this.forwardConversationRootMessagePropertyBags;
			}
		}

		public ItemPart GetItemPart(StoreObjectId itemId)
		{
			return this.dataExtractor.GetItemPart(this.conversationFamilyTree, itemId);
		}

		public IConversationTree ConversationTree
		{
			get
			{
				return this.selectedConversationTree;
			}
		}

		public StoreObjectId RootMessageId
		{
			get
			{
				return this.ConversationTree.RootMessageId;
			}
		}

		public int ItemCount
		{
			get
			{
				return this.selectedConversationTree.Count;
			}
		}

		public ConversationId ConversationId
		{
			get
			{
				return this.selectedConversationId;
			}
		}

		public IConversationTree ConversationFamilyTree
		{
			get
			{
				return this.conversationFamilyTree;
			}
		}

		public StoreObjectId ConversationFamilyRootMessageId
		{
			get
			{
				return this.conversationFamilyTree.RootMessageId;
			}
		}

		public int ConversationFamilyItemCount
		{
			get
			{
				return this.conversationFamilyTree.Count;
			}
		}

		public ConversationId ConversationFamilyId
		{
			get
			{
				return this.conversationFamilyId;
			}
		}

		public string Topic
		{
			get
			{
				return this.ConversationTree.Topic;
			}
		}

		public EffectiveRights EffectiveRights
		{
			get
			{
				return this.ConversationTree.EffectiveRights;
			}
		}

		public byte[] SerializedTreeState
		{
			get
			{
				return this.ConversationState.SerializedState;
			}
		}

		public IConversationStatistics ConversationStatistics
		{
			get
			{
				return this.dataExtractor.OptimizationCounters;
			}
		}

		protected bool IsSingleConversationFamily
		{
			get
			{
				return this.isSingleConversationFamily;
			}
		}

		private ConversationState ConversationState
		{
			get
			{
				if (this.conversationState == null)
				{
					this.conversationState = new ConversationState(this.mailboxSession, this.selectedConversationTree, null);
				}
				return this.conversationState;
			}
		}

		public byte[] GetSerializedTreeStateWithNodesToExclude(ICollection<IConversationTreeNode> nodesToExclude)
		{
			ConversationState conversationState = new ConversationState(this.mailboxSession, this.selectedConversationTree, nodesToExclude);
			return conversationState.SerializedState;
		}

		public void LoadBodySummaries()
		{
			this.dataExtractor.LoadBodySummaries(this.conversationFamilyTree);
			this.SyncConversationTrees();
		}

		public void LoadItemParts(ICollection<IConversationTreeNode> nodes)
		{
			this.LoadItemParts(from n in nodes
			select n.MainStoreObjectId);
		}

		public ParticipantSet AllParticipants(ICollection<IConversationTreeNode> loadedNodes = null)
		{
			return this.dataExtractor.CalculateAllRecipients(this.ConversationTree, loadedNodes);
		}

		public void LoadItemParts(ICollection<StoreObjectId> storeIds)
		{
			this.dataExtractor.LoadItemParts(this.conversationFamilyTree, storeIds);
			this.SyncConversationTrees();
		}

		public void TrimToNewest(int count)
		{
			if (this.dataExtractor.ConversationDataLoaded)
			{
				throw new InvalidOperationException("LoadItemParts or LoadBodySummary already called");
			}
			this.selectedConversationTree = this.selectedConversationTreeFactory.GetNewestSubTree(this.selectedConversationTree, count);
		}

		public KeyValuePair<List<StoreObjectId>, List<StoreObjectId>> CalculateChanges(byte[] olderState)
		{
			return this.ConversationState.CalculateChanges(olderState);
		}

		public ParticipantTable LoadReplyAllParticipantsPerType()
		{
			return this.dataExtractor.LoadReplyAllParticipantsPerType(this.selectedConversationTree);
		}

		public ParticipantSet LoadReplyAllParticipants(IConversationTreeNode node)
		{
			return this.dataExtractor.LoadReplyAllParticipants(this.selectedConversationTree, node);
		}

		public IConversationTreeNode RootMessageNode
		{
			get
			{
				return this.selectedConversationTree.RootMessageNode;
			}
		}

		public int GetNodeCount(bool includeSubmitted)
		{
			return this.selectedConversationTree.GetNodeCount(includeSubmitted);
		}

		public IConversationTreeNode FirstNode
		{
			get
			{
				return this.RootMessageNode;
			}
		}

		public byte[] ConversationCreatorSID
		{
			get
			{
				return this.selectedConversationTree.ConversationCreatorSID;
			}
		}

		public Dictionary<IConversationTreeNode, ParticipantSet> LoadAddedParticipants()
		{
			return this.dataExtractor.LoadAddedParticipants(this.selectedConversationTree);
		}

		public List<StoreObjectId> GetMessageIdsForPreread()
		{
			return this.dataExtractor.GetMessageIdsForPreread(this.selectedConversationTree);
		}

		public IStorePropertyBag GetItemPropertyBag(StoreObjectId itemId)
		{
			List<IStorePropertyBag> itemPropertyBags = this.GetItemPropertyBags(itemId);
			if (itemPropertyBags.Count <= 0)
			{
				return null;
			}
			return itemPropertyBags[0];
		}

		public List<IStorePropertyBag> GetItemPropertyBags(StoreObjectId itemId)
		{
			List<IStorePropertyBag> list = new List<IStorePropertyBag>();
			IConversationTreeNode conversationTreeNode;
			if (this.conversationFamilyTree.TryGetConversationTreeNode(itemId, out conversationTreeNode))
			{
				foreach (StoreObjectId itemId2 in conversationTreeNode.ToListStoreObjectId())
				{
					IStorePropertyBag item;
					if (conversationTreeNode.TryGetPropertyBag(itemId2, out item))
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public StoreObjectId GetParentId(StoreObjectId itemId, bool allowCrossConversation)
		{
			IConversationTree conversationTree = allowCrossConversation ? this.conversationFamilyTree : this.selectedConversationTree;
			if (itemId == conversationTree.RootMessageId)
			{
				return null;
			}
			IConversationTreeNode conversationTreeNode;
			if (conversationTree.TryGetConversationTreeNode(itemId, out conversationTreeNode) && conversationTreeNode.ParentNode != null && conversationTreeNode.ParentNode.HasData)
			{
				return conversationTreeNode.ParentNode.MainStoreObjectId;
			}
			return null;
		}

		public IEnumerable<StoreObjectId> EnumerateAncestorsOfSelectedConversation()
		{
			return this.EnumerateBackwardInFamilyFromNode(this.selectedConversationTree.RootMessageNode);
		}

		public Dictionary<IConversationTreeNode, IConversationTreeNode> BuildPreviousNodeGraph()
		{
			return this.selectedConversationTree.BuildPreviousNodeGraph();
		}

		public IConversationTree GetNewestSubTree(int count)
		{
			return this.selectedConversationTreeFactory.GetNewestSubTree(this.selectedConversationTree, count);
		}

		private IEnumerable<StoreObjectId> EnumerateBackwardInFamilyFromNode(IConversationTreeNode startNode)
		{
			if (startNode != null)
			{
				IConversationTreeNode nodeOnFamilyTree = this.GetEquivalentNodeFromFamilyTree(startNode);
				if (nodeOnFamilyTree != null && nodeOnFamilyTree != this.conversationFamilyTree.RootMessageNode)
				{
					IConversationTreeNode ancestor = nodeOnFamilyTree.ParentNode;
					while (ancestor != null && ancestor.HasData)
					{
						yield return ancestor.MainStoreObjectId;
						ancestor = ancestor.ParentNode;
					}
				}
			}
			yield break;
		}

		private IStorePropertyBag CalculateBackwardPropertyBag(IConversationTreeNode rootNodeFromSelectedConversation)
		{
			if (this.IsSingleConversationFamily)
			{
				return null;
			}
			if (rootNodeFromSelectedConversation == null)
			{
				return null;
			}
			IConversationTreeNode equivalentNodeFromFamilyTree = this.GetEquivalentNodeFromFamilyTree(rootNodeFromSelectedConversation);
			if (equivalentNodeFromFamilyTree != this.conversationFamilyTree.RootMessageNode)
			{
				return equivalentNodeFromFamilyTree.ParentNode.MainPropertyBag;
			}
			return null;
		}

		private Dictionary<StoreObjectId, List<IStorePropertyBag>> CalculateForwardMessagePropertyBags(IConversationTree selectedConversationTree)
		{
			if (this.IsSingleConversationFamily)
			{
				return null;
			}
			Dictionary<StoreObjectId, List<IStorePropertyBag>> dictionary = new Dictionary<StoreObjectId, List<IStorePropertyBag>>(selectedConversationTree.Count);
			ConversationId conversationId = this.ConversationId;
			foreach (IConversationTreeNode conversationTreeNode in selectedConversationTree)
			{
				IConversationTreeNode equivalentNodeFromFamilyTree = this.GetEquivalentNodeFromFamilyTree(conversationTreeNode);
				List<IStorePropertyBag> childNodesNotOnSelectedConversation = this.GetChildNodesNotOnSelectedConversation(conversationId, equivalentNodeFromFamilyTree);
				if (childNodesNotOnSelectedConversation.Count > 0)
				{
					dictionary.Add(conversationTreeNode.MainStoreObjectId, childNodesNotOnSelectedConversation);
				}
			}
			return dictionary;
		}

		private IConversationTreeNode GetEquivalentNodeFromFamilyTree(IConversationTreeNode nodeFromSelectedConversation)
		{
			IConversationTreeNode result = null;
			if (nodeFromSelectedConversation == null || !this.conversationFamilyTree.TryGetConversationTreeNode(nodeFromSelectedConversation.MainStoreObjectId, out result))
			{
				LocalizedString localizedString = ServerStrings.ExItemNotFoundInConversation((nodeFromSelectedConversation != null) ? nodeFromSelectedConversation.MainStoreObjectId : null, this.ConversationFamilyId);
				ExTraceGlobals.ConversationTracer.TraceError((long)this.GetHashCode(), localizedString);
				throw new ObjectNotFoundException(localizedString);
			}
			return result;
		}

		private void SyncConversationTrees()
		{
			if (this.IsSingleConversationFamily)
			{
				return;
			}
			this.backwardMessagePropertyBag = null;
			this.forwardConversationRootMessagePropertyBags = null;
			foreach (IConversationTreeNode conversationTreeNode in this.selectedConversationTree)
			{
				IConversationTreeNode equivalentNodeFromFamilyTree = this.GetEquivalentNodeFromFamilyTree(conversationTreeNode);
				foreach (StoreObjectId itemId in conversationTreeNode.ToListStoreObjectId())
				{
					IStorePropertyBag bag;
					if (equivalentNodeFromFamilyTree.TryGetPropertyBag(itemId, out bag))
					{
						conversationTreeNode.UpdatePropertyBag(itemId, bag);
					}
				}
			}
		}

		private List<IStorePropertyBag> GetChildNodesNotOnSelectedConversation(ConversationId selectedConversationId, IConversationTreeNode node)
		{
			List<IStorePropertyBag> list = new List<IStorePropertyBag>();
			foreach (IConversationTreeNode conversationTreeNode in node.ChildNodes)
			{
				if (conversationTreeNode.ConversationId != null && !conversationTreeNode.ConversationId.Equals(selectedConversationId))
				{
					list.Add(conversationTreeNode.MainPropertyBag);
				}
			}
			return list;
		}

		private readonly IConversationTree conversationFamilyTree;

		private readonly ConversationDataExtractor dataExtractor;

		private readonly IMailboxSession mailboxSession;

		private readonly ConversationId conversationFamilyId;

		private readonly bool isSingleConversationFamily;

		private readonly IConversationTreeFactory selectedConversationTreeFactory;

		private IStorePropertyBag backwardMessagePropertyBag;

		private ConversationState conversationState;

		private Dictionary<StoreObjectId, List<IStorePropertyBag>> forwardConversationRootMessagePropertyBags;

		private IConversationTree selectedConversationTree;

		private ConversationId selectedConversationId;
	}
}
