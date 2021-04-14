using System;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class GlobalTunables
	{
		public string LocalMachineName { get; set; }

		public TimeSpan ThrottleGroupCacheRefreshFrequency { get; set; }

		public TimeSpan ThrottleGroupCacheRefreshStartDelay { get; set; }

		public int ThrottlingV2SupportedServerVersion { get; set; }

		public bool IsRunningMock { get; set; }
	}
}
