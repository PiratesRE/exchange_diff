using System;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.ConversationNodeProcessors
{
	internal class PopulateRootNode : IConversationNodeProcessor
	{
		public void ProcessNode(IConversationTreeNode node, ConversationNode serviceNode)
		{
			serviceNode.IsRootNode = true;
		}
	}
}
