using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationClutterProcessor
	{
		int Process(bool markAsClutter, IConversationTree conversationTree, List<GroupOperationResult> results);
	}
}
