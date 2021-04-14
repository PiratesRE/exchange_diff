using System;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	internal interface IQueueFilter
	{
		bool Match(LocalQueueInfo localQueue);
	}
}
