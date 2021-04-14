using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchFeedingControllerFailureMonitor : OverallXFailuresMonitor
	{
		protected override void OnAlert()
		{
			base.Result.StateAttribute2 = SearchMonitoringHelper.GetAllLocalDatabaseCopyStatusString();
		}
	}
}
