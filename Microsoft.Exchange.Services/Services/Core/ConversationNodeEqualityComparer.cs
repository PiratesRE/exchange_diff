using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ConversationNodeEqualityComparer : IEqualityComparer<ConversationNode>
	{
		public bool Equals(ConversationNode x, ConversationNode y)
		{
			if (x.InternetMessageId == y.InternetMessageId)
			{
				return x.ParentInternetMessageId == y.ParentInternetMessageId;
			}
			return x.InternetMessageId != null && y.InternetMessageId != null && x.InternetMessageId == y.InternetMessageId;
		}

		public int GetHashCode(ConversationNode obj)
		{
			return obj.GetHashCode();
		}
	}
}
