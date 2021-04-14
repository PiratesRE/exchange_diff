using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IModernConversationNodeFactory
	{
		ConversationNode CreateInstance(IConversationTreeNode treeNode);

		bool TryLoadRelatedItemInfo(IConversationTreeNode treeNode, out IRelatedItemInfo relatedItem);

		bool TryLoadRelatedItemInfo(IStorePropertyBag storePropertyBag, out IRelatedItemInfo relatedItem);
	}
}
