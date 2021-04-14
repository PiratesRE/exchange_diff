using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MdbResourcehealthMonitorWrapper : ResourceHealthMonitorWrapper, IRPCLatencyProvider, IResourceLoadMonitor
	{
		public MdbResourcehealthMonitorWrapper(MdbResourceHealthMonitor monitor) : base(monitor)
		{
		}

		public void Update(int averageRpcLatency, uint totalDbOperations)
		{
			base.GetWrappedMonitor<MdbResourceHealthMonitor>().Update(averageRpcLatency, totalDbOperations);
		}

		public int LastRPCLatencyValue
		{
			get
			{
				return base.GetWrappedMonitor<MdbResourceHealthMonitor>().LastRPCLatencyValue;
			}
		}

		public int AverageRPCLatencyValue
		{
			get
			{
				return base.GetWrappedMonitor<MdbResourceHealthMonitor>().AverageRPCLatencyValue;
			}
		}
	}
}
