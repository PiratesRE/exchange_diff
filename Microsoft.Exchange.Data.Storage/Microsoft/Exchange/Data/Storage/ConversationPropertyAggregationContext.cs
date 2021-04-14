using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ConversationPropertyAggregationContext : PropertyAggregationContext
	{
		public ConversationPropertyAggregationContext(IList<IStorePropertyBag> sources, string clientInfoString) : base(sources)
		{
			this.clientInfoString = clientInfoString;
		}

		public string ClientInfoString
		{
			get
			{
				return this.clientInfoString;
			}
		}

		private readonly string clientInfoString;
	}
}
