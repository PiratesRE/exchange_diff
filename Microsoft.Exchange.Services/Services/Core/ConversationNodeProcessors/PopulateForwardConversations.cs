using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.ConversationNodeProcessors
{
	internal class PopulateForwardConversations : IConversationNodeProcessor
	{
		public PopulateForwardConversations(Dictionary<StoreObjectId, List<IStorePropertyBag>> forwardLinks, IModernConversationNodeFactory conversationNodeFactory)
		{
			this.forwardLinks = forwardLinks;
			this.conversationNodeFactory = conversationNodeFactory;
		}

		public void ProcessNode(IConversationTreeNode node, ConversationNode serviceNode)
		{
			if (this.forwardLinks == null)
			{
				return;
			}
			List<IStorePropertyBag> list;
			if (this.forwardLinks.TryGetValue(node.MainStoreObjectId, out list))
			{
				List<BreadcrumbAdapterType> list2 = new List<BreadcrumbAdapterType>();
				foreach (IStorePropertyBag storePropertyBag in list)
				{
					IRelatedItemInfo itemInfo;
					if (this.conversationNodeFactory.TryLoadRelatedItemInfo(storePropertyBag, out itemInfo))
					{
						list2.Add(BreadcrumbAdapterType.FromRelatedItemInfo(itemInfo));
					}
				}
				serviceNode.ForwardMessages = list2.ToArray();
			}
		}

		private readonly Dictionary<StoreObjectId, List<IStorePropertyBag>> forwardLinks;

		private readonly IModernConversationNodeFactory conversationNodeFactory;
	}
}
