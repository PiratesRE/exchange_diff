using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Conversations.LoadingListBuilders
{
	internal class ConversationNodeLoadingListBuilder : ConversationNodeLoadingListBuilderBase
	{
		public ConversationNodeLoadingListBuilder(IEnumerable<IConversationTreeNode> allNodes, IConversationTreeNode rootTreeNode, ConversationRequestArguments requestArguments) : base(allNodes)
		{
			this.rootTreeNode = rootTreeNode;
			this.requestArguments = requestArguments;
		}

		protected override void MarkDependentNodesToBeLoaded()
		{
			List<IConversationTreeNode> list = base.LoadingList.NotToBeLoaded.ToList<IConversationTreeNode>();
			List<IConversationTreeNode> newestConversationNodesFrom = this.GetNewestConversationNodesFrom(list);
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug<int, int>((long)this.GetHashCode(), "ConversationNodeLoadingListBuilder.MarkDependentNodesToBeLoaded: Unloaded nodes: {0}, nodes to load: {1}", list.Count, newestConversationNodesFrom.Count);
			if (this.rootTreeNode != null && this.rootTreeNode.HasData && list.Contains(this.rootTreeNode))
			{
				ExTraceGlobals.GetConversationItemsTracer.TraceDebug((long)this.GetHashCode(), "ConversationNodeLoadingListBuilder.MarkDependentNodesToBeLoaded: Adding root node to loading list");
				newestConversationNodesFrom.Add(this.rootTreeNode);
			}
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug<int>((long)this.GetHashCode(), "ConversationNodeLoadingListBuilder.MarkDependentNodesToBeLoaded: Marking {0} nodes to be loaded", newestConversationNodesFrom.Count);
			foreach (IConversationTreeNode treeNode in newestConversationNodesFrom)
			{
				base.LoadingList.MarkToBeLoaded(treeNode);
			}
		}

		protected override void MarkNodesToBeIgnored()
		{
			IConversationTreeNode[] array = base.LoadingList.ToBeLoaded.ToArray<IConversationTreeNode>();
			foreach (IConversationTreeNode conversationTreeNode in array)
			{
				if (base.ShouldIgnore(conversationTreeNode, this.requestArguments.ReturnSubmittedItems))
				{
					base.LoadingList.MarkToBeIgnored(conversationTreeNode);
				}
			}
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug<int>((long)this.GetHashCode(), "ConversationNodeLoadingListBuilder.MarkNodesToBeIgnored: Marking {0} nodes to be ignored", base.LoadingList.ToBeIgnored.Count<IConversationTreeNode>());
		}

		protected override void MarkNodesForProcessorsToBeLoaded()
		{
			this.MarkInReplyToNodesToBeLoaded();
		}

		protected virtual List<IConversationTreeNode> GetNewestConversationNodesFrom(List<IConversationTreeNode> unloadedNodes)
		{
			return ConversationTreeNodeBase.TrimToNewest(unloadedNodes, this.requestArguments.MaxItemsToReturn);
		}

		private void MarkInReplyToNodesToBeLoaded()
		{
			IConversationTreeNode[] array = (from node in base.LoadingList.ToBeLoaded
			where node.ParentNode != null && node.ParentNode.HasData
			select node.ParentNode).ToArray<IConversationTreeNode>();
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug<int>((long)this.GetHashCode(), "ConversationNodeLoadingListBuilder.MarkInReplyToNodesToBeLoaded: Adding in-reply-to nodes to loading list. Number of in-reply-to nodes: {0}", array.Length);
			foreach (IConversationTreeNode treeNode in array)
			{
				base.LoadingList.MarkToBeLoaded(treeNode);
			}
		}

		private readonly IConversationTreeNode rootTreeNode;

		private readonly ConversationRequestArguments requestArguments;
	}
}
