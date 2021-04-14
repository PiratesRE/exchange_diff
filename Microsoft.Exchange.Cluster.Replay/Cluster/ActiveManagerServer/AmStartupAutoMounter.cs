using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmStartupAutoMounter : AmBatchOperationBase
	{
		protected bool IsSelectOnlyActives { get; set; }

		internal AmStartupAutoMounter()
		{
			this.m_adSession = ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
			this.m_reasonCode = AmDbActionReason.Startup;
		}

		protected override void RunInternal()
		{
			AmTrace.Entering("AmStartupAutoMounter.RunInternal()", new object[0]);
			this.DelayFirstStartup();
			if (AmSystemManager.Instance.DbNodeAttemptTable != null)
			{
				AmSystemManager.Instance.DbNodeAttemptTable.ClearFailedTime();
			}
			this.RunInternalCommon();
			AmTrace.Leaving("AmStartupAutoMounter.RunInternal()", new object[0]);
		}

		protected void RunInternalCommon()
		{
			AmMultiNodeMdbStatusFetcher amMultiNodeMdbStatusFetcher = base.StartMdbStatusFetcher();
			Dictionary<Guid, DatabaseInfo> dbMap = this.GenerateDatabaseInfoMap();
			amMultiNodeMdbStatusFetcher.WaitUntilStatusIsReady();
			Dictionary<AmServerName, MdbStatus[]> mdbStatusMap = amMultiNodeMdbStatusFetcher.MdbStatusMap;
			if (mdbStatusMap == null)
			{
				ReplayEventLogConstants.Tuple_PeriodicOperationFailedRetrievingStatuses.LogEvent(null, new object[]
				{
					"AmStartupAutoMounter"
				});
				return;
			}
			this.MergeDatabaseInfoWithMdbStatus(dbMap, mdbStatusMap, amMultiNodeMdbStatusFetcher.ServerInfoMap);
			this.PopulateWithDatabaseOperations(dbMap);
			this.EnqueueGeneratedOperations(dbMap);
		}

		protected void DelayFirstStartup()
		{
			if (AmStartupAutoMounter.sm_isFirstTimeStartupMount)
			{
				AmStartupAutoMounter.sm_isFirstTimeStartupMount = false;
				AmHelper.SleepUntilShutdown(TimeSpan.FromMilliseconds((double)RegistryParameters.AutoMounterFirstStartupDelayInMsec), true);
			}
			if (AmSystemManager.Instance.StoreStateMarker != null)
			{
				AmSystemManager.Instance.StoreStateMarker.ClearAllStoreStartRequests();
			}
		}

		protected override List<AmServerName> GetServers()
		{
			List<AmServerName> result = new List<AmServerName>();
			if (this.m_amConfig.IsPAM)
			{
				result = this.m_amConfig.DagConfig.MemberServers.ToList<AmServerName>();
			}
			else
			{
				result = new List<AmServerName>
				{
					AmServerName.LocalComputerName
				};
			}
			return result;
		}

		protected Dictionary<Guid, DatabaseInfo> GenerateDatabaseInfoMap()
		{
			Dictionary<Guid, DatabaseInfo> dictionary = new Dictionary<Guid, DatabaseInfo>();
			List<AmServerName> servers = this.GetServers();
			IADConfig adconfig = Dependencies.ADConfig;
			foreach (AmServerName amServerName in servers)
			{
				IADServer server = adconfig.GetServer(amServerName);
				if (server == null)
				{
					AmTrace.Debug("Startup mount could not find server named {0} in AD", new object[]
					{
						amServerName
					});
				}
				else
				{
					IEnumerable<IADDatabase> databasesOnServer = adconfig.GetDatabasesOnServer(server);
					if (databasesOnServer != null)
					{
						using (IEnumerator<IADDatabase> enumerator2 = databasesOnServer.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								IADDatabase iaddatabase = enumerator2.Current;
								DatabaseInfo databaseInfo;
								if (!dictionary.TryGetValue(iaddatabase.Guid, out databaseInfo))
								{
									AmDbStateInfo amDbStateInfo = this.m_amConfig.DbState.Read(iaddatabase.Guid);
									if (amDbStateInfo == null)
									{
										AmTrace.Error("Skipping database {0} since AmDbStateInfo is null", new object[]
										{
											iaddatabase.Name
										});
										continue;
									}
									if (this.IsSelectOnlyActives && !AmServerName.IsNullOrEmpty(amDbStateInfo.ActiveServer) && !amDbStateInfo.ActiveServer.Equals(amServerName))
									{
										AmTrace.Debug("Skipping database {0} since it is not active on {1}.", new object[]
										{
											iaddatabase.Name,
											amServerName
										});
										continue;
									}
									if (iaddatabase.Servers == null || iaddatabase.Servers.Length == 0 || iaddatabase.DatabaseCopies == null || iaddatabase.DatabaseCopies.Length == 0)
									{
										AmTrace.Error("Skipping database {0} since no copies were found.", new object[]
										{
											iaddatabase.Name
										});
										ReplayCrimsonEvents.AmBatchMounterIgnoresInvalidDatabase.LogPeriodic<Guid, string>(iaddatabase.Guid, TimeSpan.FromMinutes(30.0), iaddatabase.Guid, iaddatabase.Name);
										continue;
									}
									databaseInfo = new DatabaseInfo(iaddatabase, amDbStateInfo);
									dictionary[iaddatabase.Guid] = databaseInfo;
								}
								databaseInfo.StoreStatus[amServerName] = null;
							}
							continue;
						}
					}
					AmTrace.Debug("Skipping server {0} for automounting since it does not have any database copies", new object[]
					{
						amServerName
					});
				}
			}
			return dictionary;
		}

		private bool ShouldConsiderMdbStatus(AmMdbStatusServerInfo serverInfo, MdbStatus[] mdbStatuses)
		{
			bool flag = false;
			if (mdbStatuses != null)
			{
				if (RegistryParameters.IsTransientFailoverSuppressionEnabled)
				{
					if (serverInfo.IsNodeUp)
					{
						flag = true;
					}
					else if (serverInfo.IsReplayRunning)
					{
						flag = true;
					}
				}
				else
				{
					flag = serverInfo.IsNodeUp;
				}
			}
			AmTrace.Info("ShouldConsiderMdbStatus: ServerName: {0}, IsConsider: {1}, IsStoreRunning: {2}, MountedDbCount: {3}, IsNodeUp: {4}, IsReplayRunning: {5}, IsTimedout: {6}", new object[]
			{
				serverInfo.ServerName,
				flag,
				serverInfo.IsStoreRunning,
				(mdbStatuses != null) ? mdbStatuses.Length : -1,
				serverInfo.IsNodeUp,
				serverInfo.IsReplayRunning,
				serverInfo.TimeOut
			});
			return flag;
		}

		protected void MergeDatabaseInfoWithMdbStatus(Dictionary<Guid, DatabaseInfo> dbMap, Dictionary<AmServerName, MdbStatus[]> mdbFullStatusMap, Dictionary<AmServerName, AmMdbStatusServerInfo> serverInfoMap)
		{
			if (mdbFullStatusMap == null)
			{
				AmTrace.Warning("MergeDatabaseInfoWithMdbStatus: mdbFullStatusMap was null.", new object[0]);
				return;
			}
			foreach (AmServerName amServerName in mdbFullStatusMap.Keys)
			{
				MdbStatus[] array = mdbFullStatusMap[amServerName];
				AmMdbStatusServerInfo amMdbStatusServerInfo = serverInfoMap[amServerName];
				if (array == null)
				{
					AmListMdbStatusMonitor.Instance.RecordFailure(amServerName);
				}
				else
				{
					AmListMdbStatusMonitor.Instance.RecordSuccess(amServerName);
				}
				if (this.ShouldConsiderMdbStatus(amMdbStatusServerInfo, array))
				{
					if (!amMdbStatusServerInfo.IsNodeUp && array.Length > 0)
					{
						base.AddDelayedFailoverEntryAsync(amServerName, this.m_reasonCode);
					}
					foreach (MdbStatus mdbStatus in array)
					{
						DatabaseInfo databaseInfo = null;
						dbMap.TryGetValue(mdbStatus.MdbGuid, out databaseInfo);
						if (databaseInfo != null)
						{
							databaseInfo.StoreStatus[amServerName] = new MdbStatusFlags?(mdbStatus.Status);
							string text = string.Format("{0}{1}{2}", mdbStatus.MdbGuid, amServerName.NetbiosName, mdbStatus.Status);
							ReplayCrimsonEvents.ObservedMdbStatus.LogPeriodic<string, Guid, string, string>(text, TimeSpan.FromMinutes(480.0), databaseInfo.Database.Name, mdbStatus.MdbGuid, amServerName.NetbiosName, string.Format("{0} (0x{0:X})", mdbStatus.Status));
						}
						else
						{
							AmTrace.Warning("{0} is reported from MdbStatus but not present in the dbmap - so ignoring", new object[]
							{
								mdbStatus.MdbGuid
							});
						}
					}
					foreach (DatabaseInfo databaseInfo2 in dbMap.Values)
					{
						MdbStatusFlags? mdbStatusFlags;
						if (databaseInfo2.StoreStatus.TryGetValue(amServerName, out mdbStatusFlags) && mdbStatusFlags == null)
						{
							databaseInfo2.StoreStatus[amServerName] = new MdbStatusFlags?(MdbStatusFlags.Offline);
						}
					}
				}
			}
		}

		protected AmDbMoveOperation BuildMoveForActivationDisabled(IADDatabase db)
		{
			AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, AmDbActionReason.ActivationDisabled, AmDbActionCategory.Move);
			return new AmDbMoveOperation(db, actionCode)
			{
				Arguments = 
				{
					MountDialOverride = DatabaseMountDialOverride.None,
					MoveComment = "Hosting MailboxServer is marked as DatabaseCopyActivationDisabled"
				}
			};
		}

		protected bool AllCopiesAreDisfavored(IADDatabase db)
		{
			IMonitoringADConfig config = Dependencies.MonitoringADConfigProvider.GetConfig(true);
			bool flag = true;
			IADDatabaseCopy[] databaseCopies = db.DatabaseCopies;
			foreach (IADDatabaseCopy iaddatabaseCopy in databaseCopies)
			{
				AmServerName serverName = new AmServerName(iaddatabaseCopy.HostServerName);
				IADServer iadserver = config.LookupMiniServerByName(serverName);
				if (!iadserver.DatabaseCopyActivationDisabledAndMoveNow)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				AmTrace.Warning("db ({0}) is ActivationDisfavored but no favored server exists. Move is skipped", new object[]
				{
					db.Name
				});
				ReplayCrimsonEvents.AllServersMarkedDisfavored.LogPeriodic<Guid, string>(db.Name, DiagCore.DefaultEventSuppressionInterval, db.Guid, db.Name);
			}
			return flag;
		}

		protected virtual void PopulateWithDatabaseOperations(Dictionary<Guid, DatabaseInfo> dbMap)
		{
			foreach (DatabaseInfo databaseInfo in dbMap.Values)
			{
				databaseInfo.Analyze();
				if (this.m_isDebugOptionEnabled && this.m_amConfig.IsIgnoreServerDebugOptionEnabled(databaseInfo.ActiveServer))
				{
					AmTrace.Warning("Batchmounter operation for database {0} is not performed since debug option {1} is set for server {2} which is the current active for the database", new object[]
					{
						databaseInfo.Database.Name,
						databaseInfo.ActiveServer.NetbiosName,
						AmDebugOptions.IgnoreServerFromAutomaticActions.ToString()
					});
				}
				else
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
						if (databaseInfo.IsActiveOnDisabledServer && !this.AllCopiesAreDisfavored(database))
						{
							list.Add(this.BuildMoveForActivationDisabled(database));
							this.m_moveRequests++;
						}
						if (!databaseInfo.IsMountedOnActive)
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

		protected void EnqueueGeneratedOperations(Dictionary<Guid, DatabaseInfo> dbMap)
		{
			ThreadPoolThreadCountHelper.IncreaseForDatabaseOperations(this.GetCountOfGeneratedOperations(dbMap));
			foreach (DatabaseInfo databaseInfo in dbMap.Values)
			{
				if (databaseInfo.OperationsQueued != null && databaseInfo.OperationsQueued.Count > 0)
				{
					base.EnqueueDatabaseOperationBatch(databaseInfo.Database.Guid, databaseInfo.OperationsQueued);
				}
			}
			foreach (DatabaseInfo databaseInfo2 in dbMap.Values)
			{
				if (databaseInfo2.OperationsQueued != null && databaseInfo2.OperationsQueued.Count > 0)
				{
					base.StartDatabaseOperationBatch(databaseInfo2.Database.Guid, databaseInfo2.OperationsQueued);
				}
			}
		}

		protected int GetCountOfGeneratedOperations(Dictionary<Guid, DatabaseInfo> dbMap)
		{
			int num = 0;
			foreach (DatabaseInfo databaseInfo in dbMap.Values)
			{
				if (databaseInfo.OperationsQueued != null)
				{
					num += databaseInfo.OperationsQueued.Count;
				}
			}
			return num;
		}

		protected bool IsActionRequired(DatabaseInfo dbInfo)
		{
			return dbInfo.IsMismounted || dbInfo.IsPeriodicMountRequired || dbInfo.IsClusterDatabaseOutOfSync || dbInfo.IsActiveOnDisabledServer || dbInfo.IsAdPropertiesOutOfSync;
		}

		protected override void LogStartupInternal()
		{
			AmTrace.Debug("Starting {0}", new object[]
			{
				base.GetType().Name
			});
			ReplayCrimsonEvents.InitiatingStartupAutomount.Log();
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3714460989U);
		}

		protected override void LogCompletionInternal()
		{
			AmTrace.Debug("Finished {0}", new object[]
			{
				base.GetType().Name
			});
			ReplayCrimsonEvents.CompletedStartupAutomount.Log<int, int, int, int>(this.m_mountRequests, this.m_dismountRequests, this.m_clusDbSyncRequests, this.m_moveRequests);
		}

		protected object m_locker = new object();

		protected IADToplogyConfigurationSession m_adSession;

		protected List<AmServerName> m_serversUpList;

		protected AmDbActionReason m_reasonCode;

		private static bool sm_isFirstTimeStartupMount = true;
	}
}
