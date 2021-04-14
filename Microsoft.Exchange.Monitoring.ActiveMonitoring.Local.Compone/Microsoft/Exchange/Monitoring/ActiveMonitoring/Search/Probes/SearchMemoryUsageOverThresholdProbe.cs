using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Search.Engine;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchMemoryUsageOverThresholdProbe : SearchProbeBase
	{
		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			long num = SearchConfig.Instance.SearchWorkingSetMemoryUsageThreshold + SearchConfig.Instance.SearchWorkingSetMemoryUsageThreshold * (long)SearchConfig.Instance.SearchWorkingSetMemoryUsageFloatingRate / 100L;
			long searchMemoryUsage = SearchMemoryModel.GetSearchMemoryUsage();
			if (searchMemoryUsage > num)
			{
				throw new SearchProbeFailureException(Strings.SearchMemoryUsageOverThreshold(((double)searchMemoryUsage / 1024.0 / 1024.0).ToString("0.00")));
			}
		}
	}
}
