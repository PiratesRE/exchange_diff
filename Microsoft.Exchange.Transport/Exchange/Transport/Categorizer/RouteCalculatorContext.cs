using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RouteCalculatorContext
	{
		public RouteCalculatorContext(RoutingContextCore contextCore, RoutingTopologyBase topologyConfig, RoutingServerInfoMap serverMap)
		{
			this.Core = contextCore;
			this.TopologyConfig = topologyConfig;
			this.ServerMap = serverMap;
		}

		public DateTime Timestamp
		{
			get
			{
				return this.TopologyConfig.WhenCreated;
			}
		}

		public readonly RoutingContextCore Core;

		public readonly RoutingTopologyBase TopologyConfig;

		public readonly RoutingServerInfoMap ServerMap;
	}
}
