using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal interface IAmClusterGroup : IDisposable
	{
		string Name { get; }

		AmGroupState State { get; }

		AmServerName OwnerNode { get; }

		bool IsCoreGroup();

		IEnumerable<AmClusterResource> EnumerateResourcesOfType(string resourceType);

		IEnumerable<AmClusterResource> EnumerateResources();

		IAmClusterResource CreateResource(string resName, string resType);

		IAmClusterResource FindResourceByTypeName(string desiredTypeName);

		void MoveGroup(TimeSpan timeout, AmServerName destinationNode);

		bool MoveGroupToReplayEnabledNode(IsReplayRunning isReplayRunning, string resourceType, TimeSpan timeout, out string finalDestinationNode);
	}
}
