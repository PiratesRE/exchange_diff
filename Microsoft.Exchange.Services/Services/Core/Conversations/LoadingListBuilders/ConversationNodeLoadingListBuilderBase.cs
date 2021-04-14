using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Conversations;

namespace Microsoft.Exchange.Services.Core.Conversations.LoadingListBuilders
{
	internal abstract class ConversationNodeLoadingListBuilderBase
	{
		protected ConversationNodeLoadingListBuilderBase(IEnumerable<IConversationTreeNode> allNodes)
		{
			this.LoadingList = new ConversationNodeLoadingList(allNodes);
		}

		public ConversationNodeLoadingList Build()
		{
			this.MarkDependentNodesToBeLoaded();
			this.MarkNodesToBeIgnored();
			this.MarkNodesForProcessorsToBeLoaded();
			return this.LoadingList;
		}

		private protected ConversationNodeLoadingList LoadingList { protected get; private set; }

		protected abstract void MarkDependentNodesToBeLoaded();

		protected abstract void MarkNodesToBeIgnored();

		protected abstract void MarkNodesForProcessorsToBeLoaded();

		protected bool ShouldIgnore(IConversationTreeNode node, bool returnSubmittedItems)
		{
			return !node.HasData || (!returnSubmittedItems && node.HasBeenSubmitted);
		}
	}
}
