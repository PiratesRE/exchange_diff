using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PropertyAggregationContext
	{
		public PropertyAggregationContext(IList<IStorePropertyBag> sources)
		{
			Util.ThrowOnNullArgument(sources, "sources");
			this.sources = sources;
		}

		public IList<IStorePropertyBag> Sources
		{
			get
			{
				return this.sources;
			}
		}

		private readonly IList<IStorePropertyBag> sources;
	}
}
