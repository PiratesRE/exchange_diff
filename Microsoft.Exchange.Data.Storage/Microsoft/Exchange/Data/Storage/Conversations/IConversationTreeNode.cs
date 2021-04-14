using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationTreeNode : IEnumerable<IConversationTreeNode>, IEnumerable, IComparable<IConversationTreeNode>
	{
		IList<IConversationTreeNode> ChildNodes { get; }

		ExDateTime? ReceivedTime { get; }

		bool TryAddChild(IConversationTreeNode node);

		void AddChild(IConversationTreeNode node);

		byte[] Index { get; }

		bool TryGetPropertyBag(StoreObjectId itemId, out IStorePropertyBag bag);

		IStorePropertyBag MainPropertyBag { get; }

		ConversationTreeNodeRelation GetRelationTo(IConversationTreeNode otherNode);

		void SortChildNodes(ConversationTreeSortOrder sortOrder);

		bool UpdatePropertyBag(StoreObjectId itemId, IStorePropertyBag bag);

		T GetValueOrDefault<T>(StoreObjectId itemId, PropertyDefinition propertyDefinition, T defaultValue = default(T));

		bool HasChildren { get; }

		ConversationId ConversationId { get; }

		ConversationId ConversationThreadId { get; }

		IConversationTreeNode ParentNode { get; set; }

		ConversationTreeSortOrder SortOrder { get; set; }

		void ApplyActionToChild(Action<IConversationTreeNode> action);

		bool IsSpecificMessageReplyStamped { get; }

		bool IsSpecificMessageReply { get; }

		bool HasBeenSubmitted { get; }

		StoreObjectId MainStoreObjectId { get; }

		bool HasData { get; }

		bool IsPartOf(StoreObjectId itemId);

		List<StoreObjectId> ToListStoreObjectId();

		T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue = default(T));

		bool HasAttachments { get; }

		List<IStorePropertyBag> StorePropertyBags { get; }
	}
}
