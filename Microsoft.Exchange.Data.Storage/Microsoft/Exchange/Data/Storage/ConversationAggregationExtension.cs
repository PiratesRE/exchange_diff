using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ConversationAggregationExtension : AggregationExtension
	{
		public ConversationAggregationExtension(MailboxSession mailboxSession)
		{
			this.clientInfoString = mailboxSession.ClientInfoString;
		}

		public override PropertyAggregationContext GetPropertyAggregationContext(IList<IStorePropertyBag> sources)
		{
			return new ConversationPropertyAggregationContext(sources, this.clientInfoString);
		}

		private readonly string clientInfoString;
	}
}
