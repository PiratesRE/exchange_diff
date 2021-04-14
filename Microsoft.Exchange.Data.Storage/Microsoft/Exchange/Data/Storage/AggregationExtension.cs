using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AggregationExtension
	{
		public virtual void BeforeAggregation(IList<IStorePropertyBag> sources)
		{
		}

		public abstract PropertyAggregationContext GetPropertyAggregationContext(IList<IStorePropertyBag> sources);
	}
}
