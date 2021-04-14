using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationTreeNodeEqualityComparer : IEqualityComparer<IConversationTreeNode>
	{
		public int GetHashCode(IConversationTreeNode obj)
		{
			if (obj != null && obj.MainStoreObjectId != null)
			{
				return obj.MainStoreObjectId.GetHashCode();
			}
			return 0;
		}

		public bool Equals(IConversationTreeNode x, IConversationTreeNode y)
		{
			return object.ReferenceEquals(x, y) || (x != null && y != null && x.MainStoreObjectId != null && y.MainStoreObjectId != null && x.MainStoreObjectId.Equals(y.MainStoreObjectId));
		}

		public static readonly ConversationTreeNodeEqualityComparer Default = new ConversationTreeNodeEqualityComparer();
	}
}
