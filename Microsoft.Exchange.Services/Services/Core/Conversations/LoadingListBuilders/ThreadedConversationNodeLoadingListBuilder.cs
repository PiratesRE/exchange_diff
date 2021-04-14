using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.Conversations;

namespace Microsoft.Exchange.Services.Core.Conversations.LoadingListBuilders
{
	internal class ThreadedConversationNodeLoadingListBuilder : ConversationNodeLoadingListBuilderBase
	{
		public ThreadedConversationNodeLoadingListBuilder(IEnumerable<IConversationTreeNode> allNodes, ConversationRequestArguments requestArguments) : base(allNodes)
		{
			this.requestArguments = requestArguments;
		}

		protected override void MarkDependentNodesToBeLoaded()
		{
			List<IConversationTreeNode> list = base.LoadingList.NotToBeLoaded.ToList<IConversationTreeNode>();
			foreach (IConversationTreeNode treeNode in list)
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
		}

		protected override void MarkNodesForProcessorsToBeLoaded()
		{
		}

		private readonly ConversationRequestArguments requestArguments;
	}
}
