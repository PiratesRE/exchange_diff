using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Worker.Throttling
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SyncRPCResourceMonitor : SyncResourceMonitor
	{
		public SyncRPCResourceMonitor(SyncLogSession syncLogSession, ResourceKey resourceKey, SyncResourceMonitorType syncResourceMonitorType) : base(syncLogSession, resourceKey, syncResourceMonitorType)
		{
			this.rpcResourceHealthMonitor = (IRPCLatencyProvider)base.ResourceHealthMonitor;
		}

		public virtual float GetRawRpcLatency()
		{
			return (float)this.rpcResourceHealthMonitor.LastRPCLatencyValue;
		}

		public virtual float GetRawRpcLatencyAverage()
		{
			return (float)this.rpcResourceHealthMonitor.AverageRPCLatencyValue;
		}

		private IRPCLatencyProvider rpcResourceHealthMonitor;
	}
}
