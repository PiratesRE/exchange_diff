using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.Conversations;

namespace Microsoft.Exchange.Services.Core.Conversations.LoadingListBuilders
{
	internal class ConversationNodeDiagnosticsLoadingListBuilder : ConversationNodeLoadingListBuilderBase
	{
		public ConversationNodeDiagnosticsLoadingListBuilder(IEnumerable<IConversationTreeNode> allNodes) : base(allNodes)
		{
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
		}

		protected override void MarkNodesForProcessorsToBeLoaded()
		{
		}
	}
}
