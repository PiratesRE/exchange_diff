using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal interface IAmClusterNode : IDisposable
	{
		AmClusterNodeHandle Handle { get; }

		AmServerName Name { get; }

		AmNodeState State { get; }

		IEnumerable<AmClusterNetInterface> EnumerateNetInterfaces();

		long GetHungNodesMask(out int currentGumId);

		AmNodeState GetState(bool isThrowIfUnknown);

		bool IsNetworkVisible(string networkName);
	}
}
