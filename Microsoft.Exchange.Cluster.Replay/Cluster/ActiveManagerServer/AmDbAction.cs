using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal abstract class AmDbAction
	{
		internal AmDbAction(AmConfig cfg, IADDatabase db, AmDbActionCode actionCode, string uniqueOperationId)
		{
			this.m_database = db;
			this.m_dbTrace = new AmDbTrace(db);
			this.Config = cfg;
			this.DatabaseName = db.Name;
			this.DatabaseGuid = db.Guid;
			this.ActionCode = actionCode;
			this.UniqueOperationId = uniqueOperationId;
			this.CurrentAttemptNumber = 1;
		}

		internal string DatabaseName { get; private set; }

		internal Guid DatabaseGuid { get; private set; }

		internal string UniqueOperationId { get; private set; }

		internal TimeSpan? LockTimeout
		{
			get
			{
				return this.m_lockTimeout;
			}
			set
			{
				this.m_lockTimeout = value;
			}
		}

		internal AmReportStatusDelegate StatusCallback { get; set; }

		internal AmDbTrace DbTrace
		{
			get
			{
				return this.m_dbTrace;
			}
		}

		protected AmConfig Config { get; set; }

		protected IADDatabase Database
		{
			get
			{
				return this.m_database;
			}
		}

		protected AmDbStateInfo State
		{
			get
			{
				if (this.m_state == null)
				{
					lock (this)
					{
						if (this.m_state == null)
						{
							this.m_state = this.Config.DbState.Read(this.Database.Guid);
						}
					}
				}
				return this.m_state;
			}
			set
			{
				this.m_state = value;
			}
		}

		protected AmDbActionCode ActionCode { get; set; }

		protected int CurrentAttemptNumber { get; set; }

		internal static void AttemptCopyLastLogsDirect(AmServerName serverName, Guid dbGuid, AmAcllArgs acllArgs, ref AmAcllReturnStatus acllStatus)
		{
			acllStatus = null;
			Dependencies.AmRpcClientWrapper.AttemptCopyLastLogsDirect(serverName.Fqdn, dbGuid, acllArgs.MountDialOverride, acllArgs.NumRetries, acllArgs.E00TimeoutMs, acllArgs.NetworkIOTimeoutMs, acllArgs.NetworkConnectTimeoutMs, acllArgs.SourceServer.Fqdn, (int)acllArgs.ActionCode, (int)acllArgs.SkipValidationChecks, acllArgs.MountPending, acllArgs.UniqueOperationId, acllArgs.SubactionAttemptNumber, ref acllStatus);
		}

		internal static void MountDatabaseDirect(AmServerName serverName, AmServerName lastMountedServerName, Guid dbGuid, MountFlags storeFlags, AmMountFlags amFlags, AmDbActionCode actionCode)
		{
			AmFaultInject.GenerateMapiExceptionIfRequired(dbGuid, serverName);
			AmMountArg mountArg = new AmMountArg((int)storeFlags, (int)amFlags, lastMountedServerName.Fqdn, (int)actionCode);
			Dependencies.AmRpcClientWrapper.MountDatabaseDirectEx(serverName.Fqdn, dbGuid, mountArg);
		}

		internal static void DismountDatabaseDirect(AmServerName serverName, Guid dbGuid, UnmountFlags flags, AmDbActionCode actionCode)
		{
			AmFaultInject.GenerateMapiExceptionIfRequired(dbGuid, serverName);
			AmDismountArg dismountArg = new AmDismountArg((int)flags, (int)actionCode);
			Dependencies.AmRpcClientWrapper.DismountDatabaseDirect(serverName.Fqdn, dbGuid, dismountArg);
		}

		internal static void DismountIfMismounted(IADDatabase db, AmDbActionCode actionCode, List<AmServerName> mismountedNodes)
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (config.Role == AmRole.Unknown)
			{
				throw new AmInvalidConfiguration(config.LastError);
			}
			AmDbStateInfo amDbStateInfo = config.DbState.Read(db.Guid);
			if (amDbStateInfo.IsEntryExist)
			{
				using (List<AmServerName>.Enumerator enumerator = mismountedNodes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AmServerName amServerName = enumerator.Current;
						if (!AmServerName.IsEqual(amServerName, amDbStateInfo.ActiveServer))
						{
							ReplayCrimsonEvents.DismountingMismountedDatabase.Log<string, Guid, AmServerName>(db.Name, db.Guid, amServerName);
							AmStoreHelper.RemoteDismount(amServerName, db.Guid, UnmountFlags.SkipCacheFlush, false);
						}
						else
						{
							AmTrace.Warning("Ignoring force dismount for {0} since it is the current active {1}", new object[]
							{
								db.Name,
								amServerName
							});
						}
					}
					return;
				}
			}
			AmTrace.Warning("DismountIfMismounted skipped since the database {0} was never mounted", new object[]
			{
				db.Name
			});
		}

		internal static void SyncClusterDatabaseState(IADDatabase db, AmDbActionCode actionCode)
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (config.IsUnknown)
			{
				AmTrace.Error("SyncClusterDatabaseState: Invalid configuration (db={0})", new object[]
				{
					db
				});
				throw new AmInvalidConfiguration(config.LastError);
			}
			AmDbStateInfo amDbStateInfo = config.DbState.Read(db.Guid);
			AmServerName amServerName;
			if (amDbStateInfo.IsActiveServerValid)
			{
				amServerName = amDbStateInfo.ActiveServer;
			}
			else
			{
				amServerName = new AmServerName(db.Server.Name);
			}
			MountStatus mountStatus = MountStatus.Dismounted;
			if ((config.IsStandalone || (config.IsPamOrSam && config.DagConfig.IsNodePubliclyUp(amServerName))) && AmStoreHelper.IsMounted(amServerName, db.Guid))
			{
				mountStatus = MountStatus.Mounted;
				AmSystemManager.Instance.DbNodeAttemptTable.ClearFailedTime(db.Guid);
			}
			MountStatus mountStatus2 = amDbStateInfo.MountStatus;
			if (mountStatus != mountStatus2 || !AmServerName.IsEqual(amDbStateInfo.LastMountedServer, amDbStateInfo.ActiveServer))
			{
				if (mountStatus != mountStatus2)
				{
					AmTrace.Debug("Mounted state reported by STORE is different from persistent state. (Database:{0}, PersistentState: {1}, StoreIsReporting: {2})", new object[]
					{
						db.Name,
						mountStatus2,
						mountStatus
					});
				}
				else
				{
					AmTrace.Debug("State is in transit. (Database:{0}, LastMountedServer: {1},ActiveServer: {2})", new object[]
					{
						db.Name,
						amDbStateInfo.LastMountedServer,
						amDbStateInfo.ActiveServer
					});
				}
				AmDbAction.WriteStateSyncMountStatus(config, amDbStateInfo, db.Guid, amServerName, mountStatus);
				ReplayCrimsonEvents.DatabaseMountStatusSynchronized.Log<string, Guid, MountStatus, MountStatus, AmServerName>(db.Name, db.Guid, mountStatus2, mountStatus, amServerName);
				return;
			}
			AmTrace.Debug("Ignored persistent state sync for {0} since nothing is out of sync", new object[]
			{
				db.Name
			});
		}

		internal static void SyncDatabaseOwningServerAndLegacyDn(IADDatabase db, AmDbActionCode actionCode)
		{
			AmConfig config = AmSystemManager.Instance.Config;
			AmDbStateInfo amDbStateInfo = config.DbState.Read(db.Guid);
			AmServerName amServerName = new AmServerName(db.Server.Name);
			AmServerName amServerName2;
			if (amDbStateInfo.IsActiveServerValid)
			{
				amServerName2 = amDbStateInfo.ActiveServer;
			}
			else
			{
				amServerName2 = amServerName;
			}
			AmTrace.Debug("Synchronizing AD properties of database {0} (initialOwningServer:{1}, newActiveServer:{2})", new object[]
			{
				db.Name,
				amServerName,
				amServerName2
			});
			bool flag = SharedDependencies.WritableADHelper.SetDatabaseLegacyDnAndOwningServer(db.Guid, amDbStateInfo.LastMountedServer, amServerName2, false);
			if (flag)
			{
				ReplayCrimsonEvents.DatabaseAdPropertiesSynchronized.Log<string, Guid, AmServerName, AmServerName>(db.Name, db.Guid, amServerName, amServerName2);
				return;
			}
			AmTrace.Debug("Ignored ad sync request database {0}", new object[]
			{
				db.Name
			});
		}

		internal static bool WriteStateSyncMountStatus(AmConfig amConfig, AmDbStateInfo stateInfo, Guid databaseGuid, AmServerName activeServer, MountStatus mountStatus)
		{
			bool flag = false;
			if (mountStatus == MountStatus.Mounted)
			{
				stateInfo.UpdateActiveServerAndIncrementFailoverSequenceNumber(activeServer);
				stateInfo.LastMountedServer = activeServer;
				stateInfo.IsAdminDismounted = false;
				stateInfo.MountStatus = mountStatus;
				stateInfo.IsAutomaticActionsAllowed = true;
				stateInfo.LastMountedTime = DateTime.UtcNow;
				flag = true;
			}
			else if (stateInfo.IsEntryExist)
			{
				stateInfo.MountStatus = mountStatus;
				if (stateInfo.IsMountSucceededAtleastOnce)
				{
					stateInfo.UpdateActiveServerAndIncrementFailoverSequenceNumber(activeServer);
					stateInfo.LastMountedServer = activeServer;
				}
				flag = true;
			}
			if (flag && AmDbAction.WriteState(amConfig, stateInfo, false))
			{
				AmDatabaseStateTracker databaseStateTracker = AmSystemManager.Instance.DatabaseStateTracker;
				if (databaseStateTracker != null)
				{
					databaseStateTracker.UpdateActive(databaseGuid, activeServer);
				}
			}
			return flag;
		}

		internal static bool WriteState(AmConfig cfg, AmDbStateInfo stateInfo, bool isBestEffort)
		{
			bool result = false;
			bool flag = true;
			try
			{
				AmConfig config = AmSystemManager.Instance.Config;
				if (cfg != null && cfg.Role != config.Role)
				{
					flag = false;
					if (!isBestEffort)
					{
						throw new AmRoleChangedWhileOperationIsInProgressException(cfg.Role.ToString(), config.Role.ToString());
					}
				}
				if (flag)
				{
					cfg.DbState.Write(stateInfo);
					result = true;
				}
			}
			catch (ClusterApiException ex)
			{
				AmTrace.Error("Error while trying to write state {0} to cluster database. (error={1})", new object[]
				{
					stateInfo,
					ex.Message
				});
				if (!isBestEffort)
				{
					throw;
				}
			}
			return result;
		}

		internal void Mount(MountFlags storeFlags, AmMountFlags amMountFlags, DatabaseMountDialOverride mountDialoverride, ref AmDbOperationDetailedStatus mountStatus)
		{
			mountStatus = new AmDbOperationDetailedStatus(this.Database);
			Exception ex = null;
			bool flag = true;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			AmDbOperationDetailedStatus tempStatus = mountStatus;
			try
			{
				using (AmDatabaseOperationLock.Lock(this.DatabaseGuid, AmDbLockReason.Mount, null))
				{
					ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
					{
						ReplayCrimsonEvents.ToplevelMountInitiated.LogGeneric(this.PrepareStartupArgs(new object[]
						{
							storeFlags,
							mountDialoverride
						}));
						this.EnsureAutomaticActionIsAllowed();
						this.ClearFailureAttemptIfAdminAction(this.DatabaseGuid);
						if (!this.State.IsEntryExist)
						{
							this.DbTrace.Info("Mounting database for the first time!", new object[0]);
						}
						this.MountInternal(storeFlags, amMountFlags, mountDialoverride, ref tempStatus);
					});
					mountStatus = tempStatus;
					this.WriteStateClearIfInProgressStatus(true);
					flag = false;
				}
			}
			catch (AmDbLockConflictException ex2)
			{
				ex = ex2;
			}
			finally
			{
				stopwatch.Stop();
				if (flag || ex != null)
				{
					string text = (ex != null) ? ex.Message : ReplayStrings.UnknownError;
					ReplayCrimsonEvents.ToplevelMountFailed.LogGeneric(this.PrepareCompletionArgs(new object[]
					{
						stopwatch.Elapsed,
						text
					}));
					ReplayEventLogConstants.Tuple_AmDatabaseMountFailed.LogEvent(null, new object[]
					{
						this.DatabaseName,
						this.State.ActiveServer,
						text
					});
				}
				else
				{
					ReplayCrimsonEvents.ToplevelMountSuccess.LogGeneric(this.PrepareCompletionArgs(new object[]
					{
						stopwatch.Elapsed
					}));
					ReplayEventLogConstants.Tuple_AmDatabaseMounted.LogEvent(null, new object[]
					{
						this.DatabaseName,
						this.State.ActiveServer
					});
				}
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		internal void Dismount(UnmountFlags flags)
		{
			Exception ex = null;
			bool flag = true;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				using (AmDatabaseOperationLock.Lock(this.DatabaseGuid, AmDbLockReason.Dismount, this.m_lockTimeout))
				{
					ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
					{
						ReplayCrimsonEvents.ToplevelDismountInitiated.LogGeneric(this.PrepareStartupArgs(new object[]
						{
							flags
						}));
						if (!this.State.IsEntryExist)
						{
							throw new AmDatabaseNeverMountedException();
						}
						this.ClearFailureAttemptIfAdminAction(this.DatabaseGuid);
						this.DismountInternal(flags);
					});
					this.WriteStateClearIfInProgressStatus(true);
					flag = false;
				}
			}
			catch (AmDbLockConflictException ex2)
			{
				ex = ex2;
			}
			finally
			{
				stopwatch.Stop();
				AmSystemManager.Instance.TransientFailoverSuppressor.AdminRequestedForRemoval(this.State.ActiveServer, "Dismount-Database");
				if (flag || ex != null)
				{
					string text = (ex != null) ? ex.Message : ReplayStrings.UnknownError;
					ReplayCrimsonEvents.ToplevelDismountFailed.LogGeneric(this.PrepareCompletionArgs(new object[]
					{
						stopwatch.Elapsed,
						text
					}));
					ReplayEventLogConstants.Tuple_AmDatabaseDismountFailed.LogEvent(null, new object[]
					{
						this.DatabaseName,
						this.State.ActiveServer,
						text
					});
				}
				else
				{
					ReplayCrimsonEvents.ToplevelDismountSuccess.LogGeneric(this.PrepareCompletionArgs(new object[]
					{
						stopwatch.Elapsed
					}));
					ReplayEventLogConstants.Tuple_AmDatabaseDismounted.LogEvent(null, new object[]
					{
						this.DatabaseName,
						this.State.ActiveServer
					});
				}
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		internal void Move(MountFlags mountFlags, UnmountFlags dismountFlags, DatabaseMountDialOverride mountDialoverride, AmServerName fromServer, AmServerName targetServer, bool tryOtherHealthyServers, AmBcsSkipFlags skipValidationChecks, string moveComment, string componentName, ref AmDbOperationDetailedStatus moveStatus)
		{
			moveStatus = new AmDbOperationDetailedStatus(this.Database);
			Exception ex = null;
			bool flag = true;
			AmServerName initialSourceServer = null;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			string initialActive = string.Empty;
			AmDbOperationDetailedStatus tempStatus = moveStatus;
			try
			{
				using (AmDatabaseOperationLock.Lock(this.DatabaseGuid, AmDbLockReason.Move, null))
				{
					ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
					{
						initialActive = this.GetSafeActiveServer();
						ReplayCrimsonEvents.ToplevelMoveInitiated.LogGeneric(this.PrepareStartupArgs(new object[]
						{
							mountFlags,
							dismountFlags,
							mountDialoverride,
							AmServerName.IsNullOrEmpty(fromServer) ? initialActive : fromServer.Fqdn,
							targetServer,
							tryOtherHealthyServers,
							skipValidationChecks,
							moveComment,
							componentName
						}));
						initialSourceServer = this.State.ActiveServer;
						this.EnsureAutomaticActionIsAllowed();
						this.ClearFailureAttemptIfAdminAction(this.DatabaseGuid);
						this.MoveInternal(mountFlags, dismountFlags, mountDialoverride, fromServer, targetServer, tryOtherHealthyServers, skipValidationChecks, componentName, ref tempStatus);
					});
					this.WriteStateClearIfInProgressStatus(true);
					flag = false;
				}
			}
			catch (AmDbLockConflictException ex2)
			{
				ex = ex2;
			}
			finally
			{
				stopwatch.Stop();
				moveStatus = tempStatus;
				if (flag || ex != null)
				{
					string text = (ex != null) ? ex.Message : ReplayStrings.UnknownError;
					ReplayCrimsonEvents.ToplevelMoveFailed.LogGeneric(this.PrepareCompletionArgs(new object[]
					{
						initialActive,
						stopwatch.Elapsed,
						text,
						moveComment
					}));
					if (AmServerName.IsNullOrEmpty(targetServer))
					{
						ReplayEventLogConstants.Tuple_AmDatabaseMoveUnspecifiedServerFailed.LogEvent(null, new object[]
						{
							this.DatabaseName,
							initialSourceServer,
							text,
							moveComment
						});
					}
					else
					{
						ReplayEventLogConstants.Tuple_AmDatabaseMoveFailed.LogEvent(null, new object[]
						{
							this.DatabaseName,
							initialActive,
							targetServer,
							text,
							moveComment
						});
					}
				}
				else
				{
					ReplayCrimsonEvents.ToplevelMoveSuccess.LogGeneric(this.PrepareCompletionArgs(new object[]
					{
						initialActive,
						stopwatch.Elapsed,
						moveComment
					}));
					ReplayEventLogConstants.Tuple_AmDatabaseMoved.LogEvent(null, new object[]
					{
						this.DatabaseName,
						initialActive,
						moveStatus.FinalDbState.ActiveServer,
						moveComment
					});
				}
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		internal void Remount(MountFlags mountFlags, DatabaseMountDialOverride mountDialoverride, AmServerName fromServer)
		{
			Exception ex = null;
			bool flag = true;
			AmServerName amServerName = null;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				using (AmDatabaseOperationLock.Lock(this.DatabaseGuid, AmDbLockReason.Remount, null))
				{
					ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
					{
						ReplayCrimsonEvents.ToplevelRemountInitiated.LogGeneric(this.PrepareStartupArgs(new object[]
						{
							mountFlags,
							mountDialoverride,
							fromServer
						}));
						if (!this.State.IsEntryExist)
						{
							this.DbTrace.Error("Database was never mounted. Remount is applicable only if it was mounted at least once", new object[0]);
							throw new AmDatabaseNeverMountedException();
						}
						if (this.State.IsAdminDismounted)
						{
							this.DbTrace.Error("Skipping remount action since the database was admin dismounted", new object[0]);
							throw new AmDbRemountSkippedSinceDatabaseWasAdminDismounted(this.DatabaseName);
						}
						if (!AmServerName.IsEqual(this.State.ActiveServer, fromServer))
						{
							this.DbTrace.Error("Skipping remount action since database master had changed", new object[0]);
							throw new AmDbRemountSkippedSinceMasterChanged(this.DatabaseName, this.State.ActiveServer.Fqdn, fromServer.NetbiosName);
						}
						this.EnsureAutomaticActionIsAllowed();
						this.RemountInternal(mountFlags, mountDialoverride, fromServer);
					});
					this.WriteStateClearIfInProgressStatus(true);
					flag = false;
				}
			}
			catch (AmDbLockConflictException ex2)
			{
				ex = ex2;
			}
			finally
			{
				stopwatch.Stop();
				if (flag || ex != null)
				{
					string text = (ex != null) ? ex.Message : ReplayStrings.UnknownError;
					ReplayCrimsonEvents.ToplevelRemountFailed.LogGeneric(this.PrepareCompletionArgs(new object[]
					{
						stopwatch.Elapsed,
						text
					}));
					ReplayEventLogConstants.Tuple_AmDatabaseMountFailed.LogEvent(null, new object[]
					{
						this.DatabaseName,
						amServerName,
						text
					});
				}
				else
				{
					ReplayCrimsonEvents.ToplevelRemountSuccess.LogGeneric(this.PrepareCompletionArgs(new object[]
					{
						stopwatch.Elapsed
					}));
					ReplayEventLogConstants.Tuple_AmDatabaseMounted.LogEvent(null, new object[]
					{
						this.DatabaseName,
						amServerName
					});
				}
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		private TimeSpan DetermineDismountTimeout(AmDbActionCode action, bool isNodeUp)
		{
			this.DbTrace.Debug("DetermineDismountTimeout: ActionCode={0}, isNodeUp={1}", new object[]
			{
				action.ToString(),
				isNodeUp
			});
			TimeSpan timeSpan = this.DismountTimeoutShort;
			if (action.IsDismountOperation)
			{
				timeSpan = this.DismountTimeoutInfinite;
			}
			else if (action.Category == AmDbActionCategory.Remount)
			{
				timeSpan = this.DismountTimeoutShort;
			}
			else if (action.IsAutomaticShutdownSwitchover)
			{
				timeSpan = this.DismountTimeoutMedium;
			}
			else if (action.IsAutomaticOperation)
			{
				if (!isNodeUp)
				{
					timeSpan = this.DismountTimeoutAsync;
				}
				else
				{
					timeSpan = this.DismountTimeoutShort;
				}
			}
			else if (action.IsAdminMoveOperation)
			{
				if (!isNodeUp)
				{
					timeSpan = this.DismountTimeoutAsync;
				}
				else
				{
					timeSpan = this.DismountTimeoutMedium;
				}
			}
			this.DbTrace.Debug("DetermineDismountTimeout: Returning timeout of {0}", new object[]
			{
				timeSpan
			});
			return timeSpan;
		}

		protected void DismountCommon(UnmountFlags flags)
		{
			AmServerName serverToDismount = this.State.ActiveServer;
			Exception dismountException = null;
			bool isSuccess = false;
			Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				this.ReportStatus(AmDbActionStatus.StoreDismountInitiated);
				isSuccess = this.AttemptDismount(this.State.ActiveServer, flags, false, out dismountException);
				if (this.ActionCode.IsAdminDismountOperation)
				{
					MountStatus storeDatabaseMountStatus = AmStoreHelper.GetStoreDatabaseMountStatus(serverToDismount, this.DatabaseGuid);
					if (storeDatabaseMountStatus == MountStatus.Dismounted)
					{
						this.WriteStateAdminDismounted();
						dismountException = null;
						return;
					}
					if (storeDatabaseMountStatus == MountStatus.Mounted)
					{
						if (dismountException == null)
						{
							dismountException = new AmDismountSucceededButStillMountedException(serverToDismount.Fqdn, this.Database.Name);
						}
						this.WriteStateDismountFinished(true, MountStatus.Mounted, true);
						return;
					}
					if (dismountException == null)
					{
						dismountException = new AmFailedToDetermineDatabaseMountStatusException(serverToDismount.Fqdn, this.Database.Name);
					}
					this.WriteStateDismountFinished(true, MountStatus.Dismounted, true);
				}
			});
			if (dismountException != null)
			{
				ex = dismountException;
			}
			if (ex != null)
			{
				this.ReportStatus(AmDbActionStatus.StoreDismountFailed);
				AmHelper.ThrowDbActionWrapperExceptionIfNecessary(ex);
				return;
			}
			this.ReportStatus(AmDbActionStatus.StoreDismountSuccessful);
		}

		internal bool AttemptDismount(AmServerName serverName, UnmountFlags flags, bool isIgnoreKnownExceptions, out Exception exception)
		{
			bool result = false;
			exception = null;
			bool isNodeup = true;
			if (this.Config.IsPamOrSam && !this.Config.DagConfig.IsNodePubliclyUp(serverName))
			{
				isNodeup = false;
			}
			TimeSpan bestEffortDismountTimeout = this.DetermineDismountTimeout(this.ActionCode, isNodeup);
			this.DbTrace.Debug("Attempting dismount (server={0}, flags={1}, actionCode={2}, dismountTimeout={3}ms, ignoreException={4})", new object[]
			{
				serverName,
				flags,
				this.ActionCode,
				bestEffortDismountTimeout.TotalMilliseconds,
				isIgnoreKnownExceptions
			});
			AmDbAction.DismountMode modeOfDismount = AmDbAction.DismountMode.None;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool flag = true;
			MountStatus mountStatus = this.State.MountStatus;
			try
			{
				ReplayCrimsonEvents.StoreDismountInitiated.LogGeneric(this.PrepareSubactionArgs(new object[]
				{
					serverName,
					flags,
					bestEffortDismountTimeout
				}));
				if (AmServerName.IsEqual(this.State.ActiveServer, serverName))
				{
					this.WriteStateDismounting(false);
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3152424253U);
				exception = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
				{
					InvokeWithTimeout.Invoke(delegate()
					{
						if (isNodeup && AmHelper.IsReplayRunning(serverName))
						{
							this.DbTrace.Debug("Attempting Dismount through active manager", new object[0]);
							modeOfDismount = AmDbAction.DismountMode.ThroughReplayService;
							AmDbAction.DismountDatabaseDirect(serverName, this.DatabaseGuid, flags, this.ActionCode);
							return;
						}
						this.DbTrace.Debug("Attempting dismount by directly RPCing to store", new object[0]);
						modeOfDismount = AmDbAction.DismountMode.DirectlyToStore;
						AmStoreHelper.RemoteDismount(serverName, this.DatabaseGuid, flags, true);
					}, bestEffortDismountTimeout);
					this.DbTrace.Debug("Database is possibly dismounted at server {0}", new object[]
					{
						serverName
					});
				});
				if (exception != null)
				{
					this.DbTrace.Debug("Dismount failed with error: {0}", new object[]
					{
						exception
					});
				}
				flag = false;
			}
			finally
			{
				stopwatch.Stop();
				string text = null;
				if (flag)
				{
					text = ReplayStrings.UnknownError;
				}
				else if (exception != null)
				{
					text = exception.Message;
				}
				if (string.IsNullOrEmpty(text))
				{
					result = true;
					ReplayCrimsonEvents.StoreDismountSuccess.LogGeneric(this.PrepareSubactionArgs(new object[]
					{
						serverName,
						modeOfDismount,
						stopwatch.Elapsed
					}));
					if (AmServerName.IsEqual(this.State.ActiveServer, serverName))
					{
						this.WriteStateDismountFinished(true, MountStatus.Dismounted, false);
					}
				}
				else
				{
					ReplayCrimsonEvents.StoreDismountFailed.LogGeneric(this.PrepareSubactionArgs(new object[]
					{
						serverName,
						modeOfDismount,
						stopwatch.Elapsed,
						text
					}));
					if (AmServerName.IsEqual(this.State.ActiveServer, serverName))
					{
						this.WriteStateDismountFinished(true, mountStatus, false);
					}
				}
			}
			return result;
		}

		internal bool WriteState()
		{
			return this.WriteState(false);
		}

		internal bool WriteState(bool isBestEffort)
		{
			return AmDbAction.WriteState(this.Config, this.State, isBestEffort);
		}

		internal bool WriteStateMountStart(AmServerName serverToMount)
		{
			this.State.UpdateActiveServerAndIncrementFailoverSequenceNumber(serverToMount);
			if (this.State.IsAdminDismounted)
			{
				if (!this.ActionCode.IsAdminMountOperation)
				{
					throw new AmDbActionRejectedAdminDismountedException(this.ActionCode.ToString());
				}
				this.State.IsAdminDismounted = false;
			}
			if (this.ActionCode.IsAdminOperation)
			{
				this.State.IsAutomaticActionsAllowed = false;
			}
			this.State.MountStatus = MountStatus.Mounting;
			return this.WriteState();
		}

		internal bool WriteStateMountSkipped(AmServerName serverToMount)
		{
			this.State.UpdateActiveServerAndIncrementFailoverSequenceNumber(serverToMount);
			this.State.MountStatus = MountStatus.Dismounted;
			return this.WriteState();
		}

		internal bool WriteStateMountSuccess()
		{
			this.State.LastMountedServer = this.State.ActiveServer;
			this.State.IsAutomaticActionsAllowed = true;
			this.State.MountStatus = MountStatus.Mounted;
			this.State.LastMountedTime = DateTime.UtcNow;
			return this.WriteState();
		}

		internal bool WriteStateMountFailed(bool isBestEffort)
		{
			if (this.ActionCode.IsAdminOperation)
			{
				this.State.IsAutomaticActionsAllowed = false;
			}
			this.State.MountStatus = MountStatus.Dismounted;
			if (this.State.IsMountSucceededAtleastOnce)
			{
				this.State.LastMountedServer = this.State.ActiveServer;
			}
			return this.WriteState(isBestEffort);
		}

		internal void WriteStateDismounting(bool isForce)
		{
			if (isForce || this.State.MountStatus != MountStatus.Dismounting)
			{
				this.State.MountStatus = MountStatus.Dismounting;
				this.WriteState();
			}
		}

		internal void WriteStateAdminDismounted()
		{
			this.State.IsAdminDismounted = true;
			this.State.IsAutomaticActionsAllowed = false;
			this.State.MountStatus = MountStatus.Dismounted;
			this.WriteState();
		}

		internal bool WriteStateDismountFinished(bool isBestEffort, MountStatus mountStatus, bool isIgnoreIfPrevStateIsSame)
		{
			bool result = true;
			if (!isIgnoreIfPrevStateIsSame || this.State.MountStatus != mountStatus)
			{
				this.State.MountStatus = mountStatus;
				result = this.WriteState(isBestEffort);
			}
			return result;
		}

		internal bool WriteStateClearIfInProgressStatus(bool isBestEffort)
		{
			if (this.State.MountStatus == MountStatus.Dismounting || this.State.MountStatus == MountStatus.Mounting)
			{
				this.State.MountStatus = MountStatus.Dismounted;
				return this.WriteState(isBestEffort);
			}
			return false;
		}

		protected object[] PrepareStartupArgs(params object[] args)
		{
			List<object> list = new List<object>();
			list.Add(this.UniqueOperationId);
			list.Add(this.DatabaseName);
			list.Add(this.DatabaseGuid);
			list.Add(this.GetSafeActiveServer());
			list.Add(this.ActionCode.Category);
			list.Add(this.ActionCode.Initiator);
			list.Add(this.ActionCode.Reason);
			list.Add(this.Config.Role);
			list.Add(this.Config.IsPamOrSam ? this.Config.DagConfig.CurrentPAM : AmServerName.LocalComputerName);
			if (args != null)
			{
				foreach (object item in args)
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		protected object[] PrepareSubactionArgs(params object[] args)
		{
			List<object> list = new List<object>();
			list.Add(this.UniqueOperationId);
			list.Add(this.DatabaseName);
			list.Add(this.DatabaseGuid);
			list.Add(this.CurrentAttemptNumber);
			if (args != null)
			{
				foreach (object item in args)
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		protected object[] PrepareCompletionArgs(params object[] args)
		{
			List<object> list = new List<object>();
			list.Add(this.UniqueOperationId);
			list.Add(this.DatabaseName);
			list.Add(this.DatabaseGuid);
			list.Add(this.GetSafeActiveServer());
			if (args != null)
			{
				foreach (object item in args)
				{
					list.Add(item);
				}
			}
			list.Add(this.GetSafeFailoverSequenceNumber());
			return list.ToArray();
		}

		protected void ReportStatus(AmDbActionStatus status)
		{
			if (this.StatusCallback != null)
			{
				this.StatusCallback(this.Database, status);
			}
		}

		protected abstract void MountInternal(MountFlags storeFlags, AmMountFlags amMountFlags, DatabaseMountDialOverride mountDialoverride, ref AmDbOperationDetailedStatus mountStatus);

		protected abstract void DismountInternal(UnmountFlags flags);

		protected abstract void MoveInternal(MountFlags mountFlags, UnmountFlags dismountFlags, DatabaseMountDialOverride mountDialoverride, AmServerName fromServer, AmServerName targetServer, bool tryOtherHealthyServers, AmBcsSkipFlags skipValidationChecks, string componentName, ref AmDbOperationDetailedStatus moveStatus);

		protected abstract void RemountInternal(MountFlags mountFlags, DatabaseMountDialOverride mountDialoverride, AmServerName fromServer);

		private string GetSafeActiveServer()
		{
			string result = null;
			try
			{
				result = this.State.ActiveServer.Fqdn;
			}
			catch (ClusterException)
			{
				result = "<unknown>";
			}
			return result;
		}

		private long GetSafeFailoverSequenceNumber()
		{
			long result = -1L;
			try
			{
				result = this.State.FailoverSequenceNumber;
			}
			catch (ClusterException)
			{
				result = -1L;
			}
			return result;
		}

		private void ClearFailureAttemptIfAdminAction(Guid dbGuid)
		{
			if (this.ActionCode.IsAdminOperation)
			{
				AmSystemManager.Instance.DbNodeAttemptTable.ClearFailedTime(dbGuid);
			}
		}

		private void EnsureAutomaticActionIsAllowed()
		{
			if (this.ActionCode.IsAutomaticOperation)
			{
				if (!this.Database.MountAtStartup)
				{
					this.DbTrace.Error("Rejecting action {0} since MountAtStartup is set to false.", new object[]
					{
						this.ActionCode
					});
					throw new AmDbActionRejectedMountAtStartupNotEnabledException(this.ActionCode.ToString());
				}
				if (!this.State.IsEntryExist)
				{
					this.DbTrace.Error("Rejecting action {0} since mount was never attempted for database.", new object[]
					{
						this.ActionCode
					});
					throw new AmDatabaseNeverMountedException();
				}
				if (this.State.IsAdminDismounted)
				{
					if (!this.ActionCode.IsMoveOperation)
					{
						this.DbTrace.Error("Rejecting action {0} since database is admin dismounted.", new object[]
						{
							this.ActionCode
						});
						throw new AmDbActionRejectedAdminDismountedException(this.ActionCode.ToString());
					}
				}
				else if (!this.State.IsMountSucceededAtleastOnce)
				{
					this.DbTrace.Error("Rejecting action {0} since mount has never finished at least once successfully.", new object[]
					{
						this.ActionCode
					});
					throw new AmDatabaseNeverMountedException();
				}
			}
		}

		private readonly TimeSpan DismountTimeoutAsync = TimeSpan.Zero;

		private readonly TimeSpan DismountTimeoutShort = TimeSpan.FromSeconds((double)RegistryParameters.PamToSamDismountRpcTimeoutShortInSec);

		private readonly TimeSpan DismountTimeoutMedium = TimeSpan.FromSeconds((double)RegistryParameters.PamToSamDismountRpcTimeoutMediumInSec);

		private readonly TimeSpan DismountTimeoutInfinite = InvokeWithTimeout.InfiniteTimeSpan;

		private readonly IADDatabase m_database;

		private AmDbStateInfo m_state;

		private TimeSpan? m_lockTimeout = null;

		private AmDbTrace m_dbTrace;

		internal delegate object[] PrepareSubactionArgsDelegate(params object[] args);

		internal enum DismountMode
		{
			None,
			ThroughReplayService,
			DirectlyToStore
		}
	}
}
