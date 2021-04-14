using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MessageHeaderResultFactory : StandardResultFactory
	{
		internal MessageHeaderResultFactory(RopId ropId) : base(ropId)
		{
		}

		public abstract RecipientCollector CreateRecipientCollector(MessageHeader messageHeader, PropertyTag[] extraPropertyTags, Encoding string8Encoding);
	}
}
