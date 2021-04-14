using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchGracefulDegradationStatusProbe : SearchProbeBase
	{
		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			DateTime? dateTime = null;
			try
			{
				dateTime = SearchMonitoringHelper.GetRecentGracefulDegradationExecutionTime();
			}
			catch (TimeoutException innerException)
			{
				throw new SearchProbeFailureException(Strings.SearchGetDiagnosticInfoTimeout((int)SearchMonitoringHelper.GetDiagnosticInfoCallTimeout.TotalSeconds), innerException);
			}
			if (dateTime != null && dateTime < DateTime.UtcNow - TimeSpan.FromMinutes(60.0))
			{
				throw new SearchProbeFailureException(Strings.SearchGracefulDegradationStatus(dateTime.ToString()));
			}
		}
	}
}
