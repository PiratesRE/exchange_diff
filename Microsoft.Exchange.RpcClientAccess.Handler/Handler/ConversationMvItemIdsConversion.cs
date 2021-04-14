using System;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal sealed class ConversationMvItemIdsConversion : ConversationMvItemIdsConversionBase
	{
		internal ConversationMvItemIdsConversion() : base(PropertyTag.ConversationItemIds, PropertyTag.ConversationItemIds)
		{
		}
	}
}
