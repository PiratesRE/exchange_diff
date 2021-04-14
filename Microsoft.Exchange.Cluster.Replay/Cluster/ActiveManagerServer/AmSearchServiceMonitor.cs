using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmSearchServiceMonitor : AmServiceMonitor
	{
		internal AmSearchServiceMonitor() : base(ComponentInstance.Globals.Search.ServiceName)
		{
		}

		public bool IsServiceRunning { get; private set; }

		protected override void OnStop()
		{
			AmTrace.Debug("AmSearchServiceMonitor: service stop detected.", new object[0]);
			this.IsServiceRunning = false;
		}

		protected override void OnStart()
		{
			AmTrace.Debug("AmSearchServiceMonitor: service start detected.", new object[0]);
			this.IsServiceRunning = true;
		}
	}
}
