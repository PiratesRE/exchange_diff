using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchTransportAgentProbe : SearchProbeBase
	{
		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			int @int = base.AttributeHelper.GetInt("MinimumProcessedDocuments", true, 0, null, null);
			double @double = base.AttributeHelper.GetDouble("FailureRateThreshold", true, 0.0, null, null);
			double num = 0.0;
			double num2 = 0.0;
			double num3 = (double)SearchMonitoringHelper.GetPerformanceCounterValue("MSExchangeSearch Transport CTS Flow", "Number Of Processed Documents", "EdgeTransport");
			double num4 = (double)SearchMonitoringHelper.GetPerformanceCounterValue("MSExchangeSearch Transport CTS Flow", "Number Of Failed Documents", "EdgeTransport");
			base.Result.StateAttribute6 = num3;
			base.Result.StateAttribute7 = num4;
			ProbeResult lastProbeResult = SearchMonitoringHelper.GetLastProbeResult(this, base.Broker, cancellationToken);
			if (lastProbeResult != null)
			{
				base.Result.StateAttribute8 = lastProbeResult.StateAttribute6;
				base.Result.StateAttribute9 = lastProbeResult.StateAttribute7;
				if (lastProbeResult.StateAttribute6 <= num3 && lastProbeResult.StateAttribute7 <= num4)
				{
					num = lastProbeResult.StateAttribute6;
					num2 = lastProbeResult.StateAttribute7;
				}
			}
			double num5 = num3 - num;
			double num6 = num4 - num2;
			if (num5 + num6 < (double)@int || num5 + num6 == 0.0)
			{
				base.Result.StateAttribute1 = "MinimumProcessedDocumentsNotMet";
				return;
			}
			double num7 = num6 / (num5 + num6);
			if (num7 > @double)
			{
				throw new SearchProbeFailureException(Strings.SearchTransportAgentFailure(num7.ToString("P"), @double.ToString("P"), (base.Definition.RecurrenceIntervalSeconds / 60).ToString("F0"), num5.ToString(), num6.ToString()));
			}
		}
	}
}
