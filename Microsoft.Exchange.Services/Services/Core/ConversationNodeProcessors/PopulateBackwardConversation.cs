using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.ConversationNodeProcessors
{
	internal class PopulateBackwardConversation : IConversationNodeProcessor
	{
		public PopulateBackwardConversation(IStorePropertyBag backwardLink, IModernConversationNodeFactory conversationNodeFactory)
		{
			this.backwardLink = backwardLink;
			this.conversationNodeFactory = conversationNodeFactory;
		}

		public void ProcessNode(IConversationTreeNode node, ConversationNode serviceNode)
		{
			if (this.backwardLink == null)
			{
				return;
			}
			IRelatedItemInfo itemInfo;
			if (this.conversationNodeFactory.TryLoadRelatedItemInfo(this.backwardLink, out itemInfo))
			{
				serviceNode.BackwardMessage = BreadcrumbAdapterType.FromRelatedItemInfo(itemInfo);
			}
		}

		private readonly IStorePropertyBag backwardLink;

		private readonly IModernConversationNodeFactory conversationNodeFactory;
	}
}
