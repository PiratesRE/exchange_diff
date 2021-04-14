using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ConversationTypeEqualityComparer : IEqualityComparer<ConversationType>
	{
		public bool Equals(ConversationType x, ConversationType y)
		{
			return x.ConversationId != null && y.ConversationId != null && IdConverter.EwsIdToConversationId(x.ConversationId.Id).Equals(IdConverter.EwsIdToConversationId(y.ConversationId.Id));
		}

		public int GetHashCode(ConversationType obj)
		{
			return obj.GetHashCode();
		}
	}
}
