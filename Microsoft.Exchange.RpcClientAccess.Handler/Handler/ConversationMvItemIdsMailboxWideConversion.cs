using System;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal sealed class ConversationMvItemIdsMailboxWideConversion : ConversationMvItemIdsConversionBase
	{
		internal ConversationMvItemIdsMailboxWideConversion() : base(PropertyTag.ConversationItemIdsMailboxWide, PropertyTag.ConversationItemIdsMailboxWide)
		{
		}
	}
}
