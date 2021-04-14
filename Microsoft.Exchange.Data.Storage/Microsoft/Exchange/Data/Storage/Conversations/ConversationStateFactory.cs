using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ConversationStateFactory
	{
		internal ConversationStateFactory(IMailboxSession session, IConversationTree conversationTree)
		{
			this.session = session;
			this.conversationTree = conversationTree;
		}

		public ConversationState Create(ICollection<IConversationTreeNode> nodesToExclude = null)
		{
			return new ConversationState(this.session, this.conversationTree, nodesToExclude);
		}

		private readonly IMailboxSession session;

		private readonly IConversationTree conversationTree;
	}
}
