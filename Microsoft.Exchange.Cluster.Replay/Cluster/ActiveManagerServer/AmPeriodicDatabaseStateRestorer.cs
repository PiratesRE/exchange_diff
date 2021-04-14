using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmPeriodicDatabaseStateRestorer : AmStartupAutoMounter
	{
		internal AmPeriodicDatabaseStateRestorer(object context)
		{
			this.m_reasonCode = AmDbActionReason.PeriodicAction;
			if (context != null)
			{
				this.m_prevDbMap = (Dictionary<Guid, DatabaseInfo>)context;
			}
		}

		private void RaiseStoreNotificationEvent(HashSet<AmServerName> serversWithFailedDatabases)
		{
			if (this.mdbStatusFetcher.ServersStatusWithException != null)
			{
				foreach (Tuple<AmServerName, Exception> tuple in this.mdbStatusFetcher.ServersStatusWithException)
				{
					AmServerName item = tuple.Item1;
					Exception item2 = tuple.Item2;
					if (item2 != null && item != null && !item.IsLocalComputerName && serversWithFailedDatabases.Contains(item))
					{
						new EventNotificationItem(ExchangeComponent.RemoteStore.Name, "ListMDBStatusNotification", string.Empty, item2.ToString(), item.NetbiosName, ResultSeverityLevel.Error)
						{
							StateAttribute2 = ((item2.InnerException == null) ? item2.GetType().FullName : item2.InnerException.GetType().FullName)
						}.Publish(false);
					}
				}
			}
		}

		protected override void RunInternal()
		{
			HashSet<AmServerName> hashSet = new HashSet<AmServerName>();
			this.mdbStatusFetcher = base.StartMdbStatusFetcher();
			Dictionary<Guid, DatabaseInfo> dictionary = null;
			dictionary = this.GenerateDatabaseInfoMapFromPrevious();
			this.mdbStatusFetcher.WaitUntilStatusIsReady();
			foreach (DatabaseInfo databaseInfo in dictionary.Values)
			{
				if (!databaseInfo.Database.AutoDagExcludeFromMonitoring)
				{
					AmServerName item = new AmServerName(databaseInfo.Database.Server.Name);
					if (!hashSet.Contains(item))
					{
						hashSet.Add(item);
					}
				}
				else
				{
					AmTrace.Info("Database '{0}' has the AutoDagExcludeFromMonitoring flag set hence excluding it from remote store monitoring resultset", new object[]
					{
						databaseInfo.Database.Name
					});
				}
			}
			if (hashSet.Count > 0)
			{
				this.RaiseStoreNotificationEvent(hashSet);
			}
			Dictionary<AmServerName, MdbStatus[]> mdbStatusMap = this.mdbStatusFetcher.MdbStatusMap;
			if (mdbStatusMap == null)
			{
				ReplayEventLogConstants.Tuple_PeriodicOperationFailedRetrievingStatuses.LogEvent(null, new object[]
				{
					"AmPeriodicDatabaseStateRestorer"
				});
				return;
			}
			base.MergeDatabaseInfoWithMdbStatus(dictionary, mdbStatusMap, this.mdbStatusFetcher.ServerInfoMap);
			this.DetermineDatabaseOperations(dictionary);
			base.EnqueueGeneratedOperations(dictionary);
		}

		protected Dictionary<Guid, DatabaseInfo> GenerateDatabaseInfoMapFromPrevious()
		{
			Dictionary<Guid, DatabaseInfo> dictionary = new Dictionary<Guid, DatabaseInfo>();
			foreach (DatabaseInfo databaseInfo in this.m_prevDbMap.Values)
			{
				IADDatabase database = databaseInfo.Database;
				AmDbStateInfo stateInfo = this.m_amConfig.DbState.Read(database.Guid);
				DatabaseInfo databaseInfo2 = new DatabaseInfo(database, stateInfo);
				foreach (AmServerName key in databaseInfo.StoreStatus.Keys)
				{
					databaseInfo2.StoreStatus[key] = null;
				}
				dictionary[database.Guid] = databaseInfo2;
			}
			return dictionary;
		}

		protected void DetermineDatabaseOperations(Dictionary<Guid, DatabaseInfo> dbMap)
		{
			foreach (DatabaseInfo databaseInfo in dbMap.Values)
			{
				databaseInfo.Analyze();
				if (this.IsActionsMatchWithPreviousPhase(databaseInfo))
				{
					List<AmDbOperation> list = new List<AmDbOperation>();
					IADDatabase database = databaseInfo.Database;
					if (databaseInfo.IsMountedButAdminRequestedDismount)
					{
						AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Admin, this.m_reasonCode, AmDbActionCategory.Dismount);
						AmDbDismountAdminDismountedOperation item = new AmDbDismountAdminDismountedOperation(database, actionCode);
						list.Add(item);
						this.m_dismountRequests++;
					}
					else
					{
						if (databaseInfo.IsMismounted)
						{
							AmDbActionCode actionCode2 = new AmDbActionCode(AmDbActionInitiator.Automatic, this.m_reasonCode, AmDbActionCategory.ForceDismount);
							AmDbDismountMismountedOperation item2 = new AmDbDismountMismountedOperation(database, actionCode2, databaseInfo.MisMountedServerList);
							list.Add(item2);
							this.m_dismountRequests++;
						}
						if (databaseInfo.IsActiveOnDisabledServer && !base.AllCopiesAreDisfavored(database))
						{
							list.Add(base.BuildMoveForActivationDisabled(database));
							this.m_moveRequests++;
						}
						if (databaseInfo.IsPeriodicMountRequired)
						{
							AmDbActionCode actionCode3 = new AmDbActionCode(AmDbActionInitiator.Automatic, this.m_reasonCode, AmDbActionCategory.Mount);
							AmDbMountOperation item3 = new AmDbMountOperation(database, actionCode3);
							list.Add(item3);
							this.m_mountRequests++;
						}
						if (databaseInfo.IsClusterDatabaseOutOfSync)
						{
							AmDbActionCode actionCode4 = new AmDbActionCode(AmDbActionInitiator.Automatic, this.m_reasonCode, AmDbActionCategory.SyncState);
							AmDbClusterDatabaseSyncOperation item4 = new AmDbClusterDatabaseSyncOperation(database, actionCode4);
							list.Add(item4);
							this.m_clusDbSyncRequests++;
						}
						if (databaseInfo.IsAdPropertiesOutOfSync)
						{
							AmDbActionCode actionCode5 = new AmDbActionCode(AmDbActionInitiator.Automatic, this.m_reasonCode, AmDbActionCategory.SyncAd);
							AmDbAdPropertySyncOperation item5 = new AmDbAdPropertySyncOperation(database, actionCode5);
							list.Add(item5);
							this.m_adSyncRequests++;
						}
					}
					databaseInfo.OperationsQueued = list;
				}
			}
		}

		protected override List<AmServerName> GetServers()
		{
			HashSet<AmServerName> hashSet = new HashSet<AmServerName>();
			foreach (DatabaseInfo databaseInfo in this.m_prevDbMap.Values)
			{
				foreach (AmServerName item in databaseInfo.StoreStatus.Keys)
				{
					hashSet.Add(item);
				}
			}
			return new List<AmServerName>(hashSet);
		}

		protected bool IsActionsMatchWithPreviousPhase(DatabaseInfo dbInfo)
		{
			DatabaseInfo databaseInfo = null;
			this.m_prevDbMap.TryGetValue(dbInfo.Database.Guid, out databaseInfo);
			return databaseInfo != null && dbInfo.IsActionsEqual(databaseInfo);
		}

		protected override void LogStartupInternal()
		{
			AmTrace.Debug("Starting {0}", new object[]
			{
				base.GetType().Name
			});
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2372283709U);
		}

		protected override void LogCompletionInternal()
		{
			AmTrace.Debug("Finished {0}", new object[]
			{
				base.GetType().Name
			});
		}

		private Dictionary<Guid, DatabaseInfo> m_prevDbMap;

		private AmMultiNodeMdbStatusFetcher mdbStatusFetcher;
	}
}
