using System;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	internal class NullQueueFilter : IQueueFilter
	{
		public bool Match(LocalQueueInfo localQueue)
		{
			return true;
		}
	}
}
