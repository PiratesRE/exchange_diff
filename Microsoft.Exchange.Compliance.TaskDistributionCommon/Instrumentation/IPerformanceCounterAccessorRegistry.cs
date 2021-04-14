using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation
{
	internal interface IPerformanceCounterAccessorRegistry
	{
		IPerformanceCounterAccessor GetOrAddPerformanceCounterAccessor(string type);
	}
}
