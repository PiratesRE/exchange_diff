using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class AmMultiNodeCopyStatusFetcher_ForPoller : AmMultiNodeCopyStatusFetcher
	{
		public AmMultiNodeCopyStatusFetcher_ForPoller(IMonitoringADConfig adConfig, ActiveManager activeManager, ConcurrentDictionary<AmServerName, bool> rpcThreadsInProgress) : base(adConfig.AmServerNames, adConfig.DatabaseMap, RpcGetDatabaseCopyStatusFlags2.None, activeManager, false)
		{
			this.m_rpcThreadsInProgress = rpcThreadsInProgress;
		}

		protected override bool TryStartRpc(AmServerName server)
		{
			bool inProgress = false;
			bool flag = this.m_rpcThreadsInProgress.TryGetValue(server, out inProgress);
			if (flag && inProgress)
			{
				return false;
			}
			this.m_rpcThreadsInProgress.AddOrUpdate(server, true, delegate(AmServerName serverName, bool oldValue)
			{
				inProgress = oldValue;
				return true;
			});
			return !inProgress;
		}

		protected override void RecordRpcCompleted(AmServerName server)
		{
			this.m_rpcThreadsInProgress.TryUpdate(server, false, true);
		}

		private ConcurrentDictionary<AmServerName, bool> m_rpcThreadsInProgress;
	}
}
