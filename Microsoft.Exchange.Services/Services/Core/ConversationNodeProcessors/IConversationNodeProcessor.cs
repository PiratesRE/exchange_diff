using System;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.ConversationNodeProcessors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IConversationNodeProcessor
	{
		void ProcessNode(IConversationTreeNode node, ConversationNode serviceNode);
	}
}
