using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmClusterServiceMonitor : AmServiceMonitor
	{
		internal AmClusterServiceMonitor() : base("Clussvc")
		{
		}

		protected override void OnStop()
		{
			AmTrace.Debug("Cluster service stop detected. Notifying system manager", new object[0]);
			AmEvtClussvcStopped amEvtClussvcStopped = new AmEvtClussvcStopped();
			amEvtClussvcStopped.Notify(true);
			AmSystemManager.Instance.ConfigManager.TriggerRefresh(true);
		}

		protected override void OnStart()
		{
			AmTrace.Debug("Cluster service start detected. Notifying config manager to refresh configuration", new object[0]);
			AmSystemManager.Instance.ConfigManager.TriggerRefresh(true);
		}
	}
}
