using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICoreConversation
	{
		StoreObjectId RootMessageId { get; }

		IConversationTreeNode RootMessageNode { get; }

		IConversationTree ConversationTree { get; }

		ConversationId ConversationId { get; }

		IConversationStatistics ConversationStatistics { get; }

		string Topic { get; }

		byte[] SerializedTreeState { get; }

		byte[] GetSerializedTreeStateWithNodesToExclude(ICollection<IConversationTreeNode> nodesToExclude);

		void LoadBodySummaries();

		void LoadItemParts(ICollection<IConversationTreeNode> nodes);

		KeyValuePair<List<StoreObjectId>, List<StoreObjectId>> CalculateChanges(byte[] olderState);

		ItemPart GetItemPart(StoreObjectId itemId);

		ParticipantSet AllParticipants(ICollection<IConversationTreeNode> loadedNodes = null);

		event OnBeforeItemLoadEventDelegate OnBeforeItemLoad;

		List<StoreObjectId> GetMessageIdsForPreread();
	}
}
