using System;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	internal interface IConversationTreeNode
	{
		IConversationTreeNode ParentNode { get; }

		IDocument ConversationMessage { get; }
	}
}
