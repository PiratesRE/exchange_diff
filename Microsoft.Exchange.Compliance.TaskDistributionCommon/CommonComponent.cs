using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon
{
	public enum CommonComponent
	{
		MessageReceiver = 1,
		MessageProcessor,
		CriticalCache,
		BestEffortCache,
		PerformanceCounterRegistry
	}
}
