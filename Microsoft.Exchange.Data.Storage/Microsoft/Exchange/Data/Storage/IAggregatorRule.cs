using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAggregatorRule
	{
		void BeginAggregation();

		void EndAggregation();

		void AddToAggregation(IStorePropertyBag propertyBag);
	}
}
