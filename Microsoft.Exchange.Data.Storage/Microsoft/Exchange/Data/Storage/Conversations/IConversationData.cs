using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationData
	{
		int GetNodeCount(bool includeSubmitted);

		IConversationTreeNode FirstNode { get; }

		Dictionary<IConversationTreeNode, ParticipantSet> LoadAddedParticipants();

		ParticipantTable LoadReplyAllParticipantsPerType();

		ParticipantSet LoadReplyAllParticipants(IConversationTreeNode node);

		Dictionary<IConversationTreeNode, IConversationTreeNode> BuildPreviousNodeGraph();

		IConversationTree GetNewestSubTree(int count);
	}
}
