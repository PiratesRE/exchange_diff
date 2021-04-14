using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal class DummyResourceHealthMonitorWrapper : ResourceHealthMonitorWrapper, IRPCLatencyProvider, IDatabaseReplicationProvider, IDatabaseAvailabilityProvider, IResourceLoadMonitor
	{
		public DummyResourceHealthMonitorWrapper(DummyResourceHealthMonitor monitor) : base(monitor)
		{
		}

		void IRPCLatencyProvider.Update(int averageRpcLatency, uint totalDbOperations)
		{
		}

		void IDatabaseReplicationProvider.Update(uint databaseReplicationHealth)
		{
		}

		void IDatabaseAvailabilityProvider.Update(uint databaseAvailabilityHealth)
		{
		}

		int IRPCLatencyProvider.LastRPCLatencyValue
		{
			get
			{
				return 0;
			}
		}

		int IRPCLatencyProvider.AverageRPCLatencyValue
		{
			get
			{
				return 0;
			}
		}
	}
}
