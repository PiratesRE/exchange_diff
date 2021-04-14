using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.ConversationNodeProcessors
{
	internal class PopulateNewParticipantsProperty : IConversationNodeProcessor
	{
		public PopulateNewParticipantsProperty(Dictionary<IConversationTreeNode, ParticipantSet> newParticipantsPerNode, IParticipantResolver resolver)
		{
			this.newParticipantsPerNode = newParticipantsPerNode;
			this.resolver = resolver;
		}

		public void ProcessNode(IConversationTreeNode node, ConversationNode serviceNode)
		{
			ParticipantSet participants;
			if (this.newParticipantsPerNode.TryGetValue(node, out participants))
			{
				serviceNode.NewParticipants = this.resolver.ResolveToEmailAddressWrapper(participants);
			}
		}

		private readonly Dictionary<IConversationTreeNode, ParticipantSet> newParticipantsPerNode;

		private readonly IParticipantResolver resolver;
	}
}
