using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.ConversationNodeProcessors
{
	internal class PopulateInReplyTo : IConversationNodeProcessor
	{
		public PopulateInReplyTo(IModernConversationNodeFactory conversationNodeFactory, Dictionary<IConversationTreeNode, IConversationTreeNode> previousNodeMap, Dictionary<IConversationTreeNode, ConversationNode> serviceNodesMap)
		{
			this.conversationNodeFactory = conversationNodeFactory;
			this.previousNodeMap = previousNodeMap;
			this.serviceNodesMap = serviceNodesMap;
		}

		public void ProcessNode(IConversationTreeNode node, ConversationNode serviceNode)
		{
			if (this.ShouldGenerateInReplyToData(node))
			{
				IRelatedItemInfo relatedItemInfo = null;
				if (!this.TryLoadReplyToInformation(node.ParentNode, out relatedItemInfo))
				{
					this.conversationNodeFactory.TryLoadRelatedItemInfo(node.ParentNode, out relatedItemInfo);
				}
				if (relatedItemInfo != null)
				{
					serviceNode.InReplyToItem = InReplyToAdapterType.FromRelatedItemInfo(relatedItemInfo);
				}
			}
		}

		private bool ShouldGenerateInReplyToData(IConversationTreeNode node)
		{
			IConversationTreeNode y;
			if (!this.previousNodeMap.TryGetValue(node, out y))
			{
				return false;
			}
			if (node.IsSpecificMessageReplyStamped)
			{
				return node.IsSpecificMessageReply;
			}
			return !ConversationTreeNodeBase.EqualityComparer.Equals(node.ParentNode, y);
		}

		private bool TryLoadReplyToInformation(IConversationTreeNode parentNode, out IRelatedItemInfo relatedItemInfo)
		{
			relatedItemInfo = null;
			ConversationNode conversationNode;
			if (parentNode.HasData && this.serviceNodesMap.TryGetValue(parentNode, out conversationNode))
			{
				if (conversationNode.ItemCount > 0 && !conversationNode.Items[0].ContainsOnlyMandatoryProperties && conversationNode.Items[0] is IRelatedItemInfo)
				{
					relatedItemInfo = (conversationNode.Items[0] as IRelatedItemInfo);
				}
				return true;
			}
			return false;
		}

		private readonly IModernConversationNodeFactory conversationNodeFactory;

		private readonly Dictionary<IConversationTreeNode, IConversationTreeNode> previousNodeMap;

		private readonly Dictionary<IConversationTreeNode, ConversationNode> serviceNodesMap;
	}
}
