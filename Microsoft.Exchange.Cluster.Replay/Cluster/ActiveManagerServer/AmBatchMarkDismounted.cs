using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmBatchMarkDismounted : AmBatchOperationBase
	{
		internal AmBatchMarkDismounted(AmServerName nodeName, AmDbActionReason reasonCode)
		{
			this.m_nodeName = nodeName;
			this.m_reasonCode = reasonCode;
		}

		protected override void LogStartupInternal()
		{
			ReplayCrimsonEvents.InitiatingMarkDismounted.Log<AmServerName>(this.m_nodeName);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2439392573U);
		}

		protected override void LogCompletionInternal()
		{
			ReplayCrimsonEvents.MarkedDatabasesStatesAsDismounted.Log<AmServerName, int>(this.m_nodeName, this.m_clusDbSyncRequests);
		}

		protected override void RunInternal()
		{
			IADToplogyConfigurationSession iadtoplogyConfigurationSession = ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
			IADServer iadserver = iadtoplogyConfigurationSession.FindServerByName(this.m_nodeName.NetbiosName);
			if (iadserver == null)
			{
				throw new ServerNotFoundException(this.m_nodeName.NetbiosName);
			}
			IADDatabase[] array = iadtoplogyConfigurationSession.GetAllDatabases(iadserver).ToArray<IADDatabase>();
			if (array.Length <= 0)
			{
				AmTrace.Info("Server '{0}' does not have any databases that needs to be marked as dismounted", new object[]
				{
					this.m_nodeName
				});
				return;
			}
			AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, this.m_reasonCode, AmDbActionCategory.SyncState);
			foreach (IADDatabase db in array)
			{
				AmDbClusterDatabaseSyncOperation operation = new AmDbClusterDatabaseSyncOperation(db, actionCode);
				this.m_clusDbSyncRequests++;
				base.EnqueueDatabaseOperation(operation);
			}
			base.StartDatabaseOperations();
		}

		protected override List<AmServerName> GetServers()
		{
			return new List<AmServerName>
			{
				this.m_nodeName
			};
		}

		private AmServerName m_nodeName;

		private AmDbActionReason m_reasonCode;
	}
}
