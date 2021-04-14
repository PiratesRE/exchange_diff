using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmPeriodicDatabaseStateAnalyzer : AmStartupAutoMounter
	{
		internal AmPeriodicDatabaseStateAnalyzer()
		{
			this.m_reasonCode = AmDbActionReason.PeriodicAction;
		}

		protected override void RunInternal()
		{
			AmMultiNodeMdbStatusFetcher amMultiNodeMdbStatusFetcher = base.StartMdbStatusFetcher();
			Dictionary<Guid, DatabaseInfo> dbMap = base.GenerateDatabaseInfoMap();
			amMultiNodeMdbStatusFetcher.WaitUntilStatusIsReady();
			Dictionary<AmServerName, MdbStatus[]> mdbStatusMap = amMultiNodeMdbStatusFetcher.MdbStatusMap;
			if (mdbStatusMap == null)
			{
				ReplayEventLogConstants.Tuple_PeriodicOperationFailedRetrievingStatuses.LogEvent(null, new object[]
				{
					"AmPeriodicDatabaseStateAnalyzer"
				});
				return;
			}
			base.MergeDatabaseInfoWithMdbStatus(dbMap, mdbStatusMap, amMultiNodeMdbStatusFetcher.ServerInfoMap);
			Dictionary<Guid, DatabaseInfo> filteredMap = this.FilterDatabasesNeedingAction(dbMap);
			this.DeferDatabaseActionsIfRequired(filteredMap);
			this.InitiateSystemFailoverIfReplayUnreachable(amMultiNodeMdbStatusFetcher, dbMap);
		}

		protected int GetActiveDatabaseCountOnServer(Dictionary<Guid, DatabaseInfo> dbMap, AmServerName server)
		{
			return dbMap.Values.Count((DatabaseInfo dbInfo) => dbInfo.IsActiveOnServerAndReplicated(server));
		}

		protected void InitiateSystemFailoverIfReplayUnreachable(AmMultiNodeMdbStatusFetcher mdbStatusFetcher, Dictionary<Guid, DatabaseInfo> dbMap)
		{
			AmRole role = AmSystemManager.Instance.Config.Role;
			if (role != AmRole.PAM)
			{
				return;
			}
			if (!RegistryParameters.OnReplDownFailoverEnabled)
			{
				ReplayCrimsonEvents.FailoverOnReplDownDisabledInRegistry.LogPeriodic(Environment.MachineName, TimeSpan.FromHours(1.0));
				return;
			}
			AmSystemFailoverOnReplayDownTracker systemFailoverOnReplayDownTracker = AmSystemManager.Instance.SystemFailoverOnReplayDownTracker;
			if (systemFailoverOnReplayDownTracker == null)
			{
				ReplayCrimsonEvents.FailoverOnReplDownFailoverTrackerNotInitialized.LogPeriodic(Environment.MachineName, TimeSpan.FromHours(1.0));
				return;
			}
			foreach (KeyValuePair<AmServerName, AmMdbStatusServerInfo> keyValuePair in mdbStatusFetcher.ServerInfoMap)
			{
				AmServerName key = keyValuePair.Key;
				AmMdbStatusServerInfo value = keyValuePair.Value;
				if (value.IsReplayRunning)
				{
					systemFailoverOnReplayDownTracker.MarkReplayUp(key);
				}
				else if (value.IsStoreRunning)
				{
					systemFailoverOnReplayDownTracker.MarkReplayDown(key, false);
					int activeDatabaseCountOnServer = this.GetActiveDatabaseCountOnServer(dbMap, key);
					if (activeDatabaseCountOnServer > 0)
					{
						systemFailoverOnReplayDownTracker.ScheduleFailover(key);
					}
					else
					{
						ReplayCrimsonEvents.FailoverOnReplDownSkipped.LogPeriodic<AmServerName, string, string>(key, TimeSpan.FromDays(1.0), key, "NoActives", "Periodic");
					}
				}
			}
		}

		protected Dictionary<Guid, DatabaseInfo> FilterDatabasesNeedingAction(Dictionary<Guid, DatabaseInfo> dbMap)
		{
			Dictionary<Guid, DatabaseInfo> dictionary = new Dictionary<Guid, DatabaseInfo>();
			foreach (DatabaseInfo databaseInfo in dbMap.Values)
			{
				databaseInfo.Analyze();
				if (base.IsActionRequired(databaseInfo))
				{
					if (this.m_isDebugOptionEnabled && this.m_amConfig.IsIgnoreServerDebugOptionEnabled(databaseInfo.ActiveServer))
					{
						AmTrace.Warning("Periodic action for database {0} is not performed since debug option {1} is set for server {2} which is the current active for the database", new object[]
						{
							databaseInfo.Database.Name,
							databaseInfo.ActiveServer.NetbiosName,
							AmDebugOptions.IgnoreServerFromAutomaticActions.ToString()
						});
					}
					else
					{
						dictionary.Add(databaseInfo.Database.Guid, databaseInfo);
					}
				}
			}
			return dictionary;
		}

		protected void DeferDatabaseActionsIfRequired(Dictionary<Guid, DatabaseInfo> filteredMap)
		{
			if (filteredMap.Count > 0)
			{
				AmEvtPeriodicDbStateRestore amEvtPeriodicDbStateRestore = new AmEvtPeriodicDbStateRestore(filteredMap);
				if (AmSystemManager.Instance.PeriodicEventManager.EnqueueDeferredSystemEvent(amEvtPeriodicDbStateRestore, RegistryParameters.AmDeferredDatabaseStateRestorerIntervalInMSec))
				{
					AmTrace.Debug("Enqueuing deferred system event for restoring database states if the state is confirmed (evt={0})", new object[]
					{
						amEvtPeriodicDbStateRestore
					});
					return;
				}
				AmTrace.Warning("There is already a timer pending for periodic action verification. Until it is completed a new one won't be posted.", new object[0]);
			}
		}

		protected override void LogStartupInternal()
		{
			AmTrace.Debug("Starting {0}", new object[]
			{
				base.GetType().Name
			});
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3982896445U);
		}

		protected override void LogCompletionInternal()
		{
			AmTrace.Debug("Finished {0}", new object[]
			{
				base.GetType().Name
			});
		}
	}
}
