using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Conversations
{
	internal sealed class ConversationNodeLoadingList
	{
		public ConversationNodeLoadingList(IEnumerable<IConversationTreeNode> allNodes)
		{
			this.nodes = new Dictionary<IConversationTreeNode, ConversationNodeLoadingList.NodeStatus>(ConversationTreeNodeBase.EqualityComparer);
			foreach (IConversationTreeNode conversationTreeNode in allNodes)
			{
				if (this.nodes.ContainsKey(conversationTreeNode))
				{
					ExTraceGlobals.GetConversationItemsTracer.TraceDebug<ConversationId>((long)this.GetHashCode(), "ConversationNodeLoadingList.ctr: Duplicate node present in allNodes: ConversationId: {0}", conversationTreeNode.ConversationId);
				}
				this.nodes[conversationTreeNode] = ConversationNodeLoadingList.NodeStatus.NotToLoad;
			}
		}

		public IEnumerable<IConversationTreeNode> ToBeLoaded
		{
			get
			{
				return from pair in this.nodes
				where pair.Value == ConversationNodeLoadingList.NodeStatus.ToBeLoaded
				select pair.Key;
			}
		}

		public IEnumerable<IConversationTreeNode> NotToBeLoaded
		{
			get
			{
				return from pair in this.nodes
				where pair.Value == ConversationNodeLoadingList.NodeStatus.NotToLoad
				select pair.Key;
			}
		}

		public IEnumerable<IConversationTreeNode> ToBeIgnored
		{
			get
			{
				return from pair in this.nodes
				where pair.Value == ConversationNodeLoadingList.NodeStatus.ToBeIgnored
				select pair.Key;
			}
		}

		public void MarkToBeLoaded(IConversationTreeNode treeNode)
		{
			this.MarkAs(treeNode, ConversationNodeLoadingList.NodeStatus.ToBeLoaded);
		}

		public void MarkToBeIgnored(IConversationTreeNode treeNode)
		{
			this.MarkAs(treeNode, ConversationNodeLoadingList.NodeStatus.ToBeIgnored);
		}

		private void MarkAs(IConversationTreeNode treeNode, ConversationNodeLoadingList.NodeStatus status)
		{
			this.nodes[treeNode] = status;
		}

		private readonly Dictionary<IConversationTreeNode, ConversationNodeLoadingList.NodeStatus> nodes;

		private enum NodeStatus
		{
			NotToLoad,
			ToBeLoaded,
			ToBeIgnored
		}
	}
}
