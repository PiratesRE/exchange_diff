using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ContentAggregationFlags
	{
		None = 0,
		HasSubscriptions = 1
	}
}
