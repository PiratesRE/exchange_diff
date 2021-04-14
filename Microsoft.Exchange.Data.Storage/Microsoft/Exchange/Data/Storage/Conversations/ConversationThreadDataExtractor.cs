using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationThreadDataExtractor
	{
		internal ConversationThreadDataExtractor(ICollection<PropertyDefinition> propertiesToAggregate, IConversationTree conversationTree, bool isSingleThreadConversation)
		{
			this.conversationTree = conversationTree;
			this.isSingleThreadConversation = isSingleThreadConversation;
			this.propertiesToAggregate = propertiesToAggregate;
		}

		public IStorePropertyBag ConstructThreadAggregatedProperties(IConversationTree threadTree)
		{
			PropertyAggregationContext context = new PropertyAggregationContext(threadTree.StorePropertyBags.ToList<IStorePropertyBag>());
			return ApplicationAggregatedProperty.Aggregate(context, this.propertiesToAggregate);
		}

		public IStorePropertyBag CalculateBackwardPropertyBag(StoreObjectId rootNodeId)
		{
			if (this.isSingleThreadConversation)
			{
				return null;
			}
			IConversationTreeNode equivalentNodeFromConversationTree = this.GetEquivalentNodeFromConversationTree(rootNodeId);
			if (equivalentNodeFromConversationTree != this.conversationTree.RootMessageNode)
			{
				return equivalentNodeFromConversationTree.ParentNode.MainPropertyBag;
			}
			return null;
		}

		public Dictionary<StoreObjectId, List<IStorePropertyBag>> CalculateForwardMessagePropertyBags(ConversationId threadId, IEnumerable<StoreObjectId> threadNodesIds)
		{
			if (this.isSingleThreadConversation)
			{
				return null;
			}
			Dictionary<StoreObjectId, List<IStorePropertyBag>> dictionary = new Dictionary<StoreObjectId, List<IStorePropertyBag>>(threadNodesIds.Count<StoreObjectId>());
			foreach (StoreObjectId storeObjectId in threadNodesIds)
			{
				List<IStorePropertyBag> childNodesNotOnThreadTree = this.GetChildNodesNotOnThreadTree(threadId, storeObjectId);
				if (childNodesNotOnThreadTree.Count > 0)
				{
					dictionary.Add(storeObjectId, childNodesNotOnThreadTree);
				}
			}
			return dictionary;
		}

		public void SyncConversationTreeNodesContent(IEnumerable<IConversationTreeNode> threadNodes)
		{
			foreach (IConversationTreeNode conversationTreeNode in threadNodes)
			{
				IConversationTreeNode equivalentNodeFromConversationTree = this.GetEquivalentNodeFromConversationTree(conversationTreeNode.MainStoreObjectId);
				foreach (StoreObjectId itemId in conversationTreeNode.ToListStoreObjectId())
				{
					IStorePropertyBag bag;
					if (equivalentNodeFromConversationTree.TryGetPropertyBag(itemId, out bag))
					{
						conversationTreeNode.UpdatePropertyBag(itemId, bag);
					}
				}
			}
		}

		private IConversationTreeNode GetEquivalentNodeFromConversationTree(StoreObjectId itemId)
		{
			IConversationTreeNode result = null;
			if (!this.conversationTree.TryGetConversationTreeNode(itemId, out result))
			{
				LocalizedString localizedString = ServerStrings.ExItemNotFoundInConversation(itemId, this.conversationTree.RootMessageNode.ConversationId);
				ExTraceGlobals.ConversationTracer.TraceError((long)this.GetHashCode(), localizedString);
				throw new ObjectNotFoundException(localizedString);
			}
			return result;
		}

		private List<IStorePropertyBag> GetChildNodesNotOnThreadTree(ConversationId conversationThreadId, StoreObjectId itemId)
		{
			IConversationTreeNode equivalentNodeFromConversationTree = this.GetEquivalentNodeFromConversationTree(itemId);
			List<IStorePropertyBag> list = new List<IStorePropertyBag>();
			foreach (IConversationTreeNode conversationTreeNode in equivalentNodeFromConversationTree.ChildNodes)
			{
				if (!conversationTreeNode.ConversationThreadId.Equals(conversationThreadId))
				{
					list.Add(conversationTreeNode.MainPropertyBag);
				}
			}
			return list;
		}

		private readonly bool isSingleThreadConversation;

		private readonly IConversationTree conversationTree;

		private readonly ICollection<PropertyDefinition> propertiesToAggregate;
	}
}
