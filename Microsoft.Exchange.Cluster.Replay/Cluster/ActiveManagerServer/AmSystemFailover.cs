using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmSystemFailover : AmSystemMoveBase
	{
		internal AmSystemFailover(AmServerName nodeName, AmDbActionReason reasonCode, bool isForce, bool skipIfReplayRunning = false) : base(nodeName)
		{
			this.m_reasonCode = reasonCode;
			this.m_isForce = isForce;
			this.skipIfReplayRunning = skipIfReplayRunning;
		}

		protected override void LogStartupInternal()
		{
			AmTrace.Debug("Starting {0} for {1}", new object[]
			{
				base.GetType().Name,
				this.m_nodeName
			});
			ReplayCrimsonEvents.InitiatingServerFailover.Log<AmServerName>(this.m_nodeName);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2976263485U);
		}

		protected override void LogCompletionInternal()
		{
			AmTrace.Debug("Finished {0} for {1}", new object[]
			{
				base.GetType().Name,
				this.m_nodeName
			});
			ReplayCrimsonEvents.CompletedFailover.Log<AmServerName, int>(this.m_nodeName, this.m_moveRequests);
		}

		protected IADDatabase[] GetDatabasesToBeMoved(IADDatabase[] dbList, MdbStatus[] mdbStatuses)
		{
			List<IADDatabase> list = new List<IADDatabase>();
			foreach (IADDatabase iaddatabase in dbList)
			{
				bool flag = true;
				if (mdbStatuses != null)
				{
					foreach (MdbStatus mdbStatus in mdbStatuses)
					{
						if (mdbStatus.MdbGuid == iaddatabase.Guid && mdbStatus.Status == MdbStatusFlags.Online)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					list.Add(iaddatabase);
				}
			}
			return list.ToArray();
		}

		protected override void RunInternal()
		{
			IADDatabase[] dbList = null;
			if (!this.m_isForce)
			{
				AmMultiNodeMdbStatusFetcher amMultiNodeMdbStatusFetcher = base.StartMdbStatusFetcher();
				IADDatabase[] databases = base.GetDatabases();
				amMultiNodeMdbStatusFetcher.WaitUntilStatusIsReady();
				Dictionary<AmServerName, MdbStatus[]> mdbStatusMap = amMultiNodeMdbStatusFetcher.MdbStatusMap;
				AmMdbStatusServerInfo amMdbStatusServerInfo = amMultiNodeMdbStatusFetcher.ServerInfoMap[this.m_nodeName];
				MdbStatus[] mdbStatuses = mdbStatusMap[this.m_nodeName];
				if (amMdbStatusServerInfo.IsReplayRunning)
				{
					if (!this.skipIfReplayRunning)
					{
						if (RegistryParameters.TransientFailoverSuppressionDelayInSec > 0)
						{
							dbList = this.GetDatabasesToBeMoved(databases, mdbStatuses);
							base.AddDelayedFailoverEntryAsync(this.m_nodeName, this.m_reasonCode);
						}
					}
					else
					{
						ReplayCrimsonEvents.FailoverOnReplDownSkipped.Log<AmServerName, string, string>(this.m_nodeName, "ReplRunning", "MoveAll");
					}
				}
				else
				{
					dbList = databases;
				}
			}
			else
			{
				dbList = base.GetDatabases();
			}
			AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, this.m_reasonCode, AmDbActionCategory.Move);
			base.MoveDatabases(actionCode, dbList);
		}

		protected override void PrepareMoveArguments(ref AmDbMoveArguments moveArgs)
		{
			moveArgs.MountDialOverride = DatabaseMountDialOverride.None;
		}

		private readonly bool m_isForce;

		private AmDbActionReason m_reasonCode;

		private readonly bool skipIfReplayRunning;
	}
}
