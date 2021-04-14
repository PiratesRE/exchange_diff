using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public interface ITopologyProvider
	{
		TopologyInfo GetLocalServerTopology(bool isForceRefresh = false);
	}
}
