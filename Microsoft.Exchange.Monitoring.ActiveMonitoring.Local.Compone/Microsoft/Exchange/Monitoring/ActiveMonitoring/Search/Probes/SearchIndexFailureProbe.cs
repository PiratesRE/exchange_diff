using System;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchIndexFailureProbe : SearchProbeBase
	{
		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			long performanceCounterValue = SearchMonitoringHelper.GetPerformanceCounterValue("Search Content Processing", "# Completed Callbacks Total", "ContentEngineNode1");
			long performanceCounterValue2 = SearchMonitoringHelper.GetPerformanceCounterValue("Search Content Processing", "# Failed Callbacks Total", "ContentEngineNode1");
			base.Result.StateAttribute6 = (double)performanceCounterValue;
			base.Result.StateAttribute7 = (double)performanceCounterValue2;
			double num = 0.0;
			double num2 = 0.0;
			ProbeResult lastProbeResult = SearchMonitoringHelper.GetLastProbeResult(this, base.Broker, cancellationToken);
			if (lastProbeResult != null)
			{
				base.Result.StateAttribute8 = lastProbeResult.StateAttribute6;
				base.Result.StateAttribute9 = lastProbeResult.StateAttribute7;
				if (lastProbeResult.StateAttribute6 <= (double)performanceCounterValue && lastProbeResult.StateAttribute7 <= (double)performanceCounterValue2)
				{
					num = lastProbeResult.StateAttribute6;
					num2 = lastProbeResult.StateAttribute7;
				}
			}
			double num3 = (double)performanceCounterValue - num;
			double num4 = (double)performanceCounterValue2 - num2;
			double num5 = num3 + num4;
			if (num5 == 0.0)
			{
				return;
			}
			double num6 = num4 / num5;
			double num7 = double.Parse(base.Definition.Attributes["FailureRateThreshold"], CultureInfo.InvariantCulture.NumberFormat);
			if (num6 > num7)
			{
				string minutes = (base.Definition.RecurrenceIntervalSeconds / 60).ToString();
				throw new SearchProbeFailureException(Strings.SearchIndexFailure(num6.ToString("P"), num7.ToString("P"), num3.ToString(), num4.ToString(), minutes));
			}
		}
	}
}
