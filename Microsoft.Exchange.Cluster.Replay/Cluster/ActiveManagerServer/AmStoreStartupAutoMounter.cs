using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmStoreStartupAutoMounter : AmStartupAutoMounter
	{
		internal AmStoreStartupAutoMounter(AmServerName nodeName)
		{
			this.m_nodeName = nodeName;
			this.m_reasonCode = AmDbActionReason.StoreStarted;
			base.IsSelectOnlyActives = true;
		}

		protected override void LogStartupInternal()
		{
			AmTrace.Debug("Starting {0} for {1}", new object[]
			{
				base.GetType().Name,
				this.m_nodeName
			});
			ReplayCrimsonEvents.InitiatingStoreStartAutomount.Log<AmServerName>(this.m_nodeName);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2909154621U);
		}

		protected override void LogCompletionInternal()
		{
			AmTrace.Debug("Finished {0} for {1}", new object[]
			{
				base.GetType().Name,
				this.m_nodeName
			});
			ReplayCrimsonEvents.CompletedStoreStartAutomount.Log<AmServerName, int, int, int, int>(this.m_nodeName, this.m_mountRequests, this.m_dismountRequests, this.m_clusDbSyncRequests, this.m_moveRequests);
		}

		protected override void RunInternal()
		{
			this.ClearFailureTimeForAllDatabasesOnServer(this.m_nodeName);
			base.RunInternalCommon();
		}

		protected void ClearFailureTimeForAllDatabasesOnServer(AmServerName serverName)
		{
			IADConfig adconfig = Dependencies.ADConfig;
			IEnumerable<IADDatabase> databasesOnServer = adconfig.GetDatabasesOnServer(serverName);
			if (databasesOnServer != null)
			{
				foreach (IADDatabase iaddatabase in databasesOnServer)
				{
					AmSystemManager.Instance.DbNodeAttemptTable.ClearFailedTime(iaddatabase.Guid);
				}
			}
		}

		protected override List<AmServerName> GetServers()
		{
			return new List<AmServerName>
			{
				this.m_nodeName
			};
		}

		private AmServerName m_nodeName;
	}
}
