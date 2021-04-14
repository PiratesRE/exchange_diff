using System;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	public static class DagTaskHelperPublic
	{
		public static bool IsLocalNodeClustered()
		{
			return DagHelper.IsLocalNodeClustered();
		}

		public static bool MovePrimaryActiveManagerRole(string CurrentPrimaryName)
		{
			AmServerName serverName = new AmServerName(CurrentPrimaryName);
			bool result;
			using (IAmCluster amCluster = ClusterFactory.Instance.OpenByName(serverName))
			{
				using (IAmClusterGroup amClusterGroup = amCluster.FindCoreClusterGroup())
				{
					string text;
					result = amClusterGroup.MoveGroupToReplayEnabledNode((string targetNode) => AmHelper.IsReplayRunning(targetNode), "Network Name", new TimeSpan(0, 3, 0), out text);
				}
			}
			return result;
		}
	}
}
