using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationTreeNodeChronologicalComparer : IComparer<IConversationTreeNode>
	{
		public int Compare(IConversationTreeNode left, IConversationTreeNode right)
		{
			ExDateTime? receivedTime = left.ReceivedTime;
			ExDateTime? receivedTime2 = right.ReceivedTime;
			if (receivedTime == null && receivedTime2 == null)
			{
				return 0;
			}
			if (receivedTime == null)
			{
				return 1;
			}
			if (receivedTime2 == null)
			{
				return -1;
			}
			return receivedTime.Value.CompareTo(receivedTime2.Value);
		}

		public static ConversationTreeNodeChronologicalComparer Default = new ConversationTreeNodeChronologicalComparer();
	}
}
