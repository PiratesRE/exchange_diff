using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal interface IRPCLatencyProvider : IResourceLoadMonitor
	{
		void Update(int averageRpcLatency, uint totalDbOperations);

		int LastRPCLatencyValue { get; }

		int AverageRPCLatencyValue { get; }
	}
}
