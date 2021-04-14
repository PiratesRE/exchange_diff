using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationTree : ICollection<IConversationTreeNode>, IEnumerable<IConversationTreeNode>, IEnumerable
	{
		bool TryGetConversationTreeNode(StoreObjectId storeObjectId, out IConversationTreeNode conversationTreeNode);

		string Topic { get; }

		byte[] ConversationCreatorSID { get; }

		EffectiveRights EffectiveRights { get; }

		IConversationTreeNode RootMessageNode { get; }

		StoreObjectId RootMessageId { get; }

		int GetNodeCount(bool includeSubmitted);

		void Sort(ConversationTreeSortOrder sortOrder);

		void ExecuteSortedAction(ConversationTreeSortOrder sortOrder, SortedActionDelegate action);

		bool IsPropertyLoaded(PropertyDefinition propertyDefinition);

		IEnumerable<IStorePropertyBag> StorePropertyBags { get; }

		Dictionary<IConversationTreeNode, IConversationTreeNode> BuildPreviousNodeGraph();

		T GetValueOrDefault<T>(StoreObjectId itemId, PropertyDefinition propertyDefinition, T defaultValue = default(T));
	}
}
