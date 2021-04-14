using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class StoreDatabase : ISchemaVersion, IStateObject
	{
		public StoreDatabase(string mdbName, Guid mdbGuid, Guid dagOrServerGuid, string serverName, string legacyDN, string description, string logPath, string filePath, string fileName, bool hostServerIsDAGMember, bool circularLoggingEnabled, DatabaseOptions databaseOptions, bool isMultiRole, TimeSpan eventHistoryRetentionPeriod, bool isRecoveryDatabase, bool allowFileRestore, ServerEditionType edition, string forestName)
		{
			this.lockName = new LockName<Guid>(mdbGuid, LockManager.LockLevel.Database);
			this.dagOrServerGuid = dagOrServerGuid;
			this.serverName = serverName;
			this.legacyDN = legacyDN;
			this.description = description;
			this.eventHistoryRetentionPeriod = eventHistoryRetentionPeriod;
			this.resourceDigest = new ResourceMonitorDigest();
			this.serverEdition = edition;
			this.allowFileRestore = allowFileRestore;
			this.hostServerIsDAGMember = hostServerIsDAGMember;
			this.forestName = forestName;
			DatabaseFlags databaseFlags = DatabaseFlags.None;
			if (circularLoggingEnabled)
			{
				databaseFlags |= DatabaseFlags.CircularLoggingEnabled;
			}
			if (isMultiRole)
			{
				databaseFlags |= DatabaseFlags.IsMultiRole;
			}
			if (isRecoveryDatabase)
			{
				this.SetStatusFlag(DatabaseStatus.ForRecovery);
			}
			else
			{
				databaseFlags |= DatabaseFlags.BackgroundMaintenanceEnabled;
			}
			this.physicalDatabase = Factory.CreateDatabase(mdbGuid, mdbName, logPath, filePath, fileName, databaseFlags, databaseOptions);
			this.DismountError = ErrorCode.NoError;
		}

		public TaskList TaskList
		{
			get
			{
				return this.taskList;
			}
		}

		public StoreDatabase.ComponentDataStorage ComponentData
		{
			get
			{
				if (this.componentDataStorage == null)
				{
					Interlocked.CompareExchange<StoreDatabase.ComponentDataStorage>(ref this.componentDataStorage, new StoreDatabase.ComponentDataStorage(this), null);
				}
				return this.componentDataStorage;
			}
		}

		public Database PhysicalDatabase
		{
			get
			{
				return this.physicalDatabase;
			}
		}

		public ILockName LockName
		{
			get
			{
				return this.lockName;
			}
		}

		public Guid MdbGuid
		{
			get
			{
				return this.lockName.NameValue;
			}
		}

		public Guid DagOrServerGuid
		{
			get
			{
				return this.dagOrServerGuid;
			}
		}

		public string MdbName
		{
			get
			{
				return this.physicalDatabase.DisplayName;
			}
		}

		public string VServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string LegacyDN
		{
			get
			{
				return this.legacyDN;
			}
		}

		public bool HostServerIsDAGMember
		{
			get
			{
				return this.hostServerIsDAGMember;
			}
		}

		public bool CircularLoggingEnabled
		{
			get
			{
				return (this.physicalDatabase.Flags & DatabaseFlags.CircularLoggingEnabled) != DatabaseFlags.None;
			}
		}

		public bool IsMultiRole
		{
			get
			{
				return (this.physicalDatabase.Flags & DatabaseFlags.IsMultiRole) != DatabaseFlags.None;
			}
		}

		public bool IsOnlineActive
		{
			get
			{
				return StoreDatabase.DatabaseState.OnlineActive == this.state;
			}
		}

		public bool IsOnlinePassive
		{
			get
			{
				StoreDatabase.DatabaseState databaseState = this.state;
				switch (databaseState)
				{
				case StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnly:
				case StoreDatabase.DatabaseState.OnlinePassiveReplayingLogs:
					break;
				default:
					switch (databaseState)
					{
					case StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOnlinePassiveAttachedReadOnly:
					case StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOnlinePassiveReplayingLogs:
						break;
					default:
						return false;
					}
					break;
				}
				return true;
			}
		}

		public bool IsOnlinePassiveAttachedReadOnly
		{
			get
			{
				return StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnly == this.state;
			}
		}

		public bool IsOnlinePassiveReplayingLogs
		{
			get
			{
				return StoreDatabase.DatabaseState.OnlinePassiveReplayingLogs == this.state;
			}
		}

		public bool IsOffline
		{
			get
			{
				return StoreDatabase.DatabaseState.Offline == this.state;
			}
		}

		public bool IsTransitioningToOnlineActive
		{
			get
			{
				switch (this.state)
				{
				case StoreDatabase.DatabaseState.OfflineToOnlineActive:
				case StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOnlineActive:
				case StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOnlineActive:
					return true;
				}
				return false;
			}
		}

		public bool IsTransitioningToOnline
		{
			get
			{
				switch (this.state)
				{
				case StoreDatabase.DatabaseState.OfflineToOnlineActive:
				case StoreDatabase.DatabaseState.OfflineToOnlinePassiveReplayingLogs:
				case StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOnlineActive:
				case StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOnlineActive:
				case StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOnlinePassiveAttachedReadOnly:
				case StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOnlinePassiveReplayingLogs:
					return true;
				default:
					return false;
				}
			}
		}

		public bool IsTransitioningToOffline
		{
			get
			{
				switch (this.state)
				{
				case StoreDatabase.DatabaseState.OnlineActiveToOffline:
				case StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOffline:
				case StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOffline:
					return true;
				default:
					return false;
				}
			}
		}

		public bool IsTransitioningFromOffline
		{
			get
			{
				switch (this.state)
				{
				case StoreDatabase.DatabaseState.OfflineToOnlineActive:
				case StoreDatabase.DatabaseState.OfflineToOnlinePassiveReplayingLogs:
					return true;
				default:
					return false;
				}
			}
		}

		public bool IsTransitioningBetweenOnlinePassiveStates
		{
			get
			{
				switch (this.state)
				{
				case StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOnlinePassiveAttachedReadOnly:
				case StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOnlinePassiveReplayingLogs:
					return true;
				default:
					return false;
				}
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnly == this.state;
			}
		}

		public bool IsBackupInProgress
		{
			get
			{
				return (this.MdbStatus & DatabaseStatus.BackupInProgress) == DatabaseStatus.BackupInProgress;
			}
		}

		public bool IsInInteg
		{
			get
			{
				return (this.MdbStatus & DatabaseStatus.InInteg) == DatabaseStatus.InInteg;
			}
		}

		public bool IsPublic
		{
			get
			{
				return (this.MdbStatus & DatabaseStatus.IsPublic) == DatabaseStatus.IsPublic;
			}
		}

		public bool IsRecovery
		{
			get
			{
				return (this.MdbStatus & DatabaseStatus.ForRecovery) == DatabaseStatus.ForRecovery;
			}
		}

		public bool IsMaintenance
		{
			get
			{
				return (this.MdbStatus & DatabaseStatus.Maintenance) == DatabaseStatus.Maintenance;
			}
		}

		public uint ExternalMdbStatus
		{
			get
			{
				DatabaseStatus databaseStatus = this.MdbStatus;
				if (this.IsOnlineActive)
				{
					databaseStatus |= DatabaseStatus.OnLine;
				}
				else if (this.IsOnlinePassiveAttachedReadOnly)
				{
					databaseStatus |= DatabaseStatus.AttachedReadOnly;
				}
				else
				{
					databaseStatus = databaseStatus;
				}
				return (uint)databaseStatus;
			}
		}

		public bool AllowFileRestore
		{
			get
			{
				return this.allowFileRestore;
			}
		}

		public string FilePath
		{
			get
			{
				return this.physicalDatabase.FilePath;
			}
		}

		public string FileName
		{
			get
			{
				return this.physicalDatabase.FileName;
			}
		}

		public string LogPath
		{
			get
			{
				return this.physicalDatabase.LogPath;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public TimeSpan EventHistoryRetentionPeriod
		{
			get
			{
				return this.eventHistoryRetentionPeriod;
			}
		}

		public DateTime MountTime
		{
			get
			{
				return this.mountTime;
			}
			private set
			{
				this.mountTime = value;
			}
		}

		public long MountId
		{
			get
			{
				return this.mountTime.Ticks;
			}
		}

		public ResourceMonitorDigest ResourceDigest
		{
			get
			{
				return this.resourceDigest;
			}
		}

		public string ForestName
		{
			get
			{
				return this.forestName;
			}
		}

		public DatabaseHeaderInfo DatabaseHeaderInfo
		{
			get
			{
				return this.databaseHeaderInfo;
			}
		}

		internal static StoreDatabase.InitInMemoryDatabaseSchemaHandlerDelegate InitInMemoryDatabaseSchemaHandler { get; set; }

		internal static StoreDatabase.MountingHandlerDelegate MountingHandler { get; set; }

		internal static StoreDatabase.MountedHandlerDelegate MountedHandler { get; set; }

		internal static StoreDatabase.DismountingHandlerDelegate DismountingHandler { get; set; }

		internal StorePerDatabasePerformanceCountersInstance CachedStorePerDatabasePerformanceCountersInstance { get; set; }

		internal ErrorCode DismountError { get; set; }

		internal ServerEditionType ServerEdition
		{
			get
			{
				return this.serverEdition;
			}
		}

		private DatabaseStatus MdbStatus
		{
			get
			{
				return this.mdbStatus;
			}
		}

		internal static void Initialize()
		{
			SchemaUpgradeService.Register(SchemaUpgradeService.SchemaCategory.Database, new SchemaUpgrader[]
			{
				AddLastMaintenanceTimeToMailbox.Instance,
				AddUpgradeHistoryTable.Instance,
				AsyncMessageCleanup.Instance,
				AddMidsetDeletedDelta.Instance,
				AddGroupMailboxType.Instance,
				UnifiedMailbox.Instance,
				RemoveFolderIdsetIn.Instance,
				UserInfoUpgrader.Instance,
				AddDatabaseDsGuidToGlobalsTable.Instance,
				EnableAddingSpecialFolders.Instance
			});
		}

		internal static void Terminate()
		{
			SchemaUpgradeService.ClearRegistrations();
		}

		public static ComponentVersion GetMinimumSchemaVersion()
		{
			return StoreDatabase.minimumSchemaVersionTestHook.Value;
		}

		public static ComponentVersion GetMaximumSchemaVersion()
		{
			return StoreDatabase.maximumSchemaVersionTestHook.Value;
		}

		public static int AllocateComponentDataSlot()
		{
			return StoreDatabase.ComponentDataStorage.AllocateSlot();
		}

		public static bool IsSharedLockHeld(Guid databaseGuid)
		{
			return LockManager.TestLock(StoreDatabase.GetDatabaseLockName(databaseGuid), LockManager.LockType.DatabaseShared);
		}

		public static bool IsExclusiveLockHeld(Guid databaseGuid)
		{
			return LockManager.TestLock(StoreDatabase.GetDatabaseLockName(databaseGuid), LockManager.LockType.DatabaseExclusive);
		}

		[Conditional("DEBUG")]
		public static void AssertSharedLockHeld(Guid databaseGuid)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertExclusiveLockHeld(Guid databaseGuid)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertLocked(Guid databaseGuid)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertNotLocked(Guid databaseGuid)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertNoDatabaseLocked()
		{
		}

		public void GetSharedLock()
		{
			this.GetSharedLock(null);
		}

		public void GetSharedLock(ILockStatistics lockStats)
		{
			LockManager.GetLock(this.LockName, LockManager.LockType.DatabaseShared, lockStats);
		}

		public void ReleaseSharedLock()
		{
			LockManager.ReleaseLock(this.LockName, LockManager.LockType.DatabaseShared);
		}

		public LockManager.NamedLockFrame SharedLock(ILockStatistics lockStats)
		{
			return LockManager.Lock(this.LockName, LockManager.LockType.DatabaseShared, lockStats);
		}

		public bool IsSharedLockHeld()
		{
			return LockManager.TestLock(this.LockName, LockManager.LockType.DatabaseShared);
		}

		public void GetExclusiveLock()
		{
			try
			{
				Interlocked.Increment(ref this.exclusiveLockContentionCounter);
				LockManager.GetLock(this.LockName, LockManager.LockType.DatabaseExclusive);
			}
			finally
			{
				Interlocked.Decrement(ref this.exclusiveLockContentionCounter);
			}
		}

		public void ReleaseExclusiveLock()
		{
			LockManager.ReleaseLock(this.LockName, LockManager.LockType.DatabaseExclusive);
		}

		public bool IsExclusiveLockHeld()
		{
			return LockManager.TestLock(this.LockName, LockManager.LockType.DatabaseExclusive);
		}

		public bool HasExclusiveLockContention()
		{
			return this.exclusiveLockContentionCounter > 0;
		}

		[Conditional("DEBUG")]
		public void AssertSharedLockHeld()
		{
		}

		[Conditional("DEBUG")]
		public void AssertExclusiveLockHeld()
		{
		}

		[Conditional("DEBUG")]
		public void AssertLocked()
		{
		}

		[Conditional("DEBUG")]
		public void AssertNotLocked()
		{
		}

		[Conditional("DEBUG")]
		public void AssertDatabaseIsSafe()
		{
		}

		[Conditional("DEBUG")]
		public void GetLockAndAssert(Func<bool> assertCondition, string assertMessage, bool sharedLock)
		{
			if (sharedLock)
			{
				this.GetSharedLock();
			}
			else
			{
				this.GetExclusiveLock();
			}
			if (sharedLock)
			{
				this.ReleaseSharedLock();
				return;
			}
			this.ReleaseExclusiveLock();
		}

		public void SetBackupInProgress()
		{
			this.SetStatusFlag(DatabaseStatus.BackupInProgress);
		}

		public void ResetBackupInProgress()
		{
			this.ResetStatusFlag(DatabaseStatus.BackupInProgress);
		}

		public void ResetDatabaseEngine()
		{
			if (this.IsOnlineActive)
			{
				this.PhysicalDatabase.ResetDatabaseEngine();
			}
		}

		public bool IsDatabaseEngineTooBusyForDatabaseMaintenanceTask(Guid maintenanceId)
		{
			string text;
			long num;
			long num2;
			bool flag = this.PhysicalDatabase.IsDatabaseEngineBusy(out text, out num, out num2);
			if (StoreDatabase.isDatabaseEngineBusyTestHook.Value != null)
			{
				flag = StoreDatabase.isDatabaseEngineBusyTestHook.Value(flag);
			}
			if (flag)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_DatabaseMaintenancePreemptedByDbEngineBusy, new object[]
				{
					this.MdbGuid,
					this.MdbName,
					maintenanceId,
					MaintenanceHandler.GetDatabaseMaintenanceTaskName(maintenanceId),
					text,
					num,
					num2
				});
			}
			return flag;
		}

		public bool IsDatabaseEngineTooBusyForMailboxMaintenanceTask(MailboxState mailboxState, Guid maintenanceId)
		{
			string text;
			long num;
			long num2;
			bool flag = this.PhysicalDatabase.IsDatabaseEngineBusy(out text, out num, out num2);
			if (StoreDatabase.isDatabaseEngineBusyTestHook.Value != null)
			{
				flag = StoreDatabase.isDatabaseEngineBusyTestHook.Value(flag);
			}
			if (flag)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_MailboxMaintenancePreemptedByDbEngineBusy, new object[]
				{
					this.MdbGuid,
					this.MdbName,
					mailboxState.MailboxGuid,
					mailboxState.MailboxNumber,
					mailboxState.TenantHint,
					maintenanceId,
					MaintenanceHandler.GetMailboxMaintenanceTaskName(maintenanceId),
					text,
					num,
					num2
				});
			}
			return flag;
		}

		public void TraceState(LID lid)
		{
			DiagnosticContext.TraceDword(lid, (uint)this.state);
		}

		void IStateObject.OnBeforeCommit(Context context)
		{
		}

		void IStateObject.OnCommit(Context context)
		{
		}

		void IStateObject.OnAbort(Context context)
		{
			this.currentSchemaVersion = null;
		}

		public void ExtendDatabase(Context context)
		{
			this.PhysicalDatabase.ExtendDatabase(context);
		}

		public void ShrinkDatabase(Context context)
		{
			this.PhysicalDatabase.ShrinkDatabase(context);
		}

		public ComponentVersion CurrentSchemaVersionForDiagnostics
		{
			get
			{
				if (this.currentSchemaVersion == null)
				{
					return StoreDatabase.maximumSchemaVersionTestHook.Value;
				}
				return this.currentSchemaVersion.Value;
			}
		}

		public ComponentVersion GetCurrentSchemaVersion(Context context)
		{
			if (this.currentSchemaVersion == null)
			{
				GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(this);
				Column[] columnsToFetch = new Column[]
				{
					globalsTable.DatabaseVersion
				};
				int @int;
				try
				{
					using (TableOperator tableOperator = Factory.CreateTableOperator(CultureHelper.DefaultCultureInfo, context, globalsTable.Table, globalsTable.Table.PrimaryKeyIndex, columnsToFetch, null, null, 0, 1, KeyRange.AllRows, false, true))
					{
						using (Reader reader = tableOperator.ExecuteReader(false))
						{
							if (!reader.Read())
							{
								throw new DatabaseSchemaBroken(this.MdbGuid.ToString(), "No globals table row");
							}
							@int = reader.GetInt32(globalsTable.DatabaseVersion);
						}
					}
				}
				catch (DatabaseSchemaBroken databaseSchemaBroken)
				{
					context.OnExceptionCatch(databaseSchemaBroken);
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_DatabaseLogicalCorruption, new object[]
					{
						this.MdbGuid
					});
					this.PublishHaFailure(FailureTag.Configuration);
					throw new DatabaseLogicalCorruption((LID)63776U, this.MdbGuid, databaseSchemaBroken);
				}
				this.currentSchemaVersion = new ComponentVersion?(new ComponentVersion(@int));
			}
			return this.currentSchemaVersion.Value;
		}

		public void SetCurrentSchemaVersion(Context context, ComponentVersion version)
		{
			GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(this);
			Column[] columnsToUpdate = new Column[]
			{
				globalsTable.DatabaseVersion
			};
			List<object> list = new List<object>(1);
			list.Add(version.Value);
			using (UpdateOperator updateOperator = Factory.CreateUpdateOperator(CultureHelper.DefaultCultureInfo, context, Factory.CreateTableOperator(CultureHelper.DefaultCultureInfo, context, globalsTable.Table, globalsTable.Table.PrimaryKeyIndex, null, null, null, 0, 1, KeyRange.AllRows, false, true), columnsToUpdate, list, true))
			{
				int num = (int)updateOperator.ExecuteScalar();
			}
			this.currentSchemaVersion = new ComponentVersion?(version);
		}

		public ComponentVersion GetRequestedSchemaVersion(Context context, ComponentVersion currentVersion, ComponentVersion maximumSupportedVersion)
		{
			int num;
			if ((this.MdbStatus & DatabaseStatus.ForRecovery) == DatabaseStatus.ForRecovery)
			{
				num = currentVersion.Value;
			}
			else
			{
				if (this.hostServerIsDAGMember)
				{
					num = currentVersion.Value;
					try
					{
						using (IClusterDB clusterDB = ClusterDB.Open())
						{
							if (clusterDB != null && clusterDB.IsInstalled && clusterDB.IsInitialized)
							{
								ClusterDBHelpers.ReadRequestedDatabaseSchemaVersion(clusterDB, this.MdbGuid, currentVersion.Value, out num);
							}
						}
						goto IL_89;
					}
					catch (ClusterException exception)
					{
						context.OnExceptionCatch(exception);
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(num == currentVersion.Value, "value changed?");
						goto IL_89;
					}
				}
				num = maximumSupportedVersion.Value;
			}
			IL_89:
			num = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "Requested Version", num);
			num = Math.Min(num, maximumSupportedVersion.Value);
			return new ComponentVersion(num);
		}

		public string Identifier
		{
			get
			{
				return this.MdbGuid.ToString();
			}
		}

		int ISchemaVersion.MailboxNumber
		{
			get
			{
				return 0;
			}
		}

		internal static IDisposable SetMinimumSchemaVersionTestHook(ComponentVersion newMinimum)
		{
			return StoreDatabase.minimumSchemaVersionTestHook.SetTestHook(newMinimum);
		}

		internal static IDisposable SetMaximumSchemaVersionTestHook(ComponentVersion newMaximum)
		{
			DisposeGuard disposeGuard = default(DisposeGuard);
			disposeGuard.Add<IDisposable>(StoreDatabase.maximumSchemaVersionTestHook.SetTestHook(newMaximum));
			disposeGuard.Add<IDisposable>(ConfigurationSchema.MaximumRequestableSchemaVersion.SetDefaultValueHook(newMaximum));
			return disposeGuard;
		}

		internal static IDisposable SetDismountRequestedStopTasksTestHook(Action action)
		{
			return StoreDatabase.dismountRequestedStopTasksTestHook.SetTestHook(action);
		}

		internal static IDisposable SetIsDatabaseEngineBusyTestHook(Func<bool, bool> testDelegate)
		{
			return StoreDatabase.isDatabaseEngineBusyTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetPreMoveToNewStateTestHook(Action<StoreDatabase.DatabaseState, StoreDatabase.DatabaseState> testDelegate)
		{
			return StoreDatabase.preMoveToNewStateTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetPostMoveToNewStateTestHook(Action<StoreDatabase.DatabaseState, object> testDelegate)
		{
			return StoreDatabase.postMoveToNewStateTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetWaitForTransitionBetweenOnlinePassiveStatesTestHook(Action testDelegate)
		{
			return StoreDatabase.waitForTransitionBetweenOnlinePassiveStatesTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetSimulateTimeoutWaitingForTransitionBetweenOnlinePassiveStatesTestHook(Func<int, bool> testDelegate)
		{
			return StoreDatabase.simulateTimeoutWaitingForTransitionBetweenOnlinePassiveStatesTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetDatabaseCreationTestHook(Action action)
		{
			return StoreDatabase.databaseCreationTestHook.SetTestHook(action);
		}

		internal static IDisposable SetPassiveAttachedDetachedTestHook(Action<bool, uint> testDelegate)
		{
			return StoreDatabase.passiveAttachedDetachedTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetPermitReplayThreadAttachToPassiveDatabaseTestHook(Action<StoreDatabase> testDelegate)
		{
			return StoreDatabase.permitReplayThreadAttachPassiveDatabaseTestHook.SetTestHook(testDelegate);
		}

		internal ErrorCode MountDatabase(Context context, MountFlags flags, ref bool errorOnTheThreadExecutingTheMount)
		{
			bool flag = (flags & MountFlags.LogReplay) == MountFlags.LogReplay;
			bool allowLoss = (flags & MountFlags.AllowDatabasePatch) == MountFlags.AllowDatabasePatch || (flags & MountFlags.AcceptDataLoss) == MountFlags.AcceptDataLoss;
			StoreDatabase.MountOperation mountOperation = StoreDatabase.MountOperation.None;
			bool flag2 = true;
			bool flag3 = false;
			bool flag4 = false;
			StoreDatabase.DatabaseState databaseState = StoreDatabase.DatabaseState.Offline;
			try
			{
				if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StoreDatabaseTracer.TraceDebug<MountFlags, StoreDatabase.DatabaseState>(0L, "StoreDatabase:MountDatabase Flags:{0} Current State:{1}.", flags, this.state);
				}
				this.GetExclusiveLock();
				if (this.IsTransitioningBetweenOnlinePassiveStates)
				{
					if (flag)
					{
						flag2 = false;
						return ErrorCode.NoError;
					}
					if (!this.WaitForTransitionBetweenOnlinePassiveStates())
					{
						this.TraceState((LID)36604U);
						return ErrorCode.CreateMountInProgress((LID)61180U);
					}
				}
				if (this.IsOffline)
				{
					if (flag)
					{
						mountOperation = StoreDatabase.MountOperation.MountPassive;
						databaseState = StoreDatabase.DatabaseState.OfflineToOnlinePassiveReplayingLogs;
					}
					else
					{
						mountOperation = StoreDatabase.MountOperation.MountActive;
						databaseState = StoreDatabase.DatabaseState.OfflineToOnlineActive;
					}
				}
				else if (this.IsOnlineActive)
				{
					if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
					{
						ExTraceGlobals.FaultInjectionTracer.TraceTest(2730896701U);
					}
					if (flag)
					{
						flag2 = false;
						return ErrorCode.CreateDatabaseStateConflict((LID)33340U);
					}
					flag2 = false;
					return ErrorCode.NoError;
				}
				else if (this.IsOnlinePassiveAttachedReadOnly)
				{
					if (flag)
					{
						flag2 = false;
						return ErrorCode.NoError;
					}
					this.StartForceDetachPassiveDatabase();
					flag4 = true;
					databaseState = StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOnlineActive;
					mountOperation = StoreDatabase.MountOperation.ActivatePassive;
				}
				else if (this.IsOnlinePassiveReplayingLogs)
				{
					if (flag)
					{
						flag2 = false;
						return ErrorCode.NoError;
					}
					databaseState = StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOnlineActive;
					mountOperation = StoreDatabase.MountOperation.ActivatePassive;
				}
				else
				{
					if (this.IsTransitioningToOnline)
					{
						this.TraceState((LID)50172U);
						return ErrorCode.CreateMountInProgress((LID)58296U);
					}
					if (this.IsTransitioningToOffline)
					{
						this.TraceState((LID)48380U);
						return ErrorCode.CreateDismountInProgress((LID)33720U);
					}
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "Unexpected database state.");
				}
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(StoreDatabase.MountOperation.None != mountOperation, "The mount operation should be defined.");
				this.SetNewState(databaseState);
				this.ReleaseExclusiveLock();
				if (StoreDatabase.postMoveToNewStateTestHook.Value != null)
				{
					StoreDatabase.postMoveToNewStateTestHook.Value(databaseState, null);
				}
				flag3 = true;
				if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest(4207291709U);
				}
				databaseState = StoreDatabase.DatabaseState.Offline;
				DiagnosticContext.TraceDword((LID)45120U, (uint)Environment.TickCount);
				switch (mountOperation)
				{
				case StoreDatabase.MountOperation.MountActive:
					this.MountActive(context, allowLoss);
					databaseState = StoreDatabase.DatabaseState.OnlineActive;
					break;
				case StoreDatabase.MountOperation.MountPassive:
					this.MountPassive(context);
					databaseState = StoreDatabase.DatabaseState.OnlinePassiveReplayingLogs;
					break;
				case StoreDatabase.MountOperation.ActivatePassive:
					if (flag4)
					{
						this.FinishForceDetachPassiveDatabase(context);
					}
					this.ActivatePassive(context);
					databaseState = StoreDatabase.DatabaseState.OnlineActive;
					break;
				}
				DiagnosticContext.TraceDword((LID)59776U, (uint)Environment.TickCount);
				FaultInjection.InjectFault(this.databaseTestHook);
				this.GetExclusiveLock();
				this.SetNewState(databaseState);
				if (mountOperation == StoreDatabase.MountOperation.MountPassive)
				{
					this.PermitReplayThreadAttachPassiveDatabase();
				}
				this.ReleaseExclusiveLock();
				if (StoreDatabase.postMoveToNewStateTestHook.Value != null)
				{
					StoreDatabase.postMoveToNewStateTestHook.Value(databaseState, null);
				}
				this.PostMountInitialization(context);
				if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest(2596678973U);
				}
				flag3 = false;
				flag2 = false;
			}
			finally
			{
				errorOnTheThreadExecutingTheMount = flag3;
				if (this.IsExclusiveLockHeld())
				{
					this.ReleaseExclusiveLock();
				}
				if (flag2)
				{
					this.PublishHaFailure(FailureTag.GenericMountFailure);
				}
				else if (!flag)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_WorkerMountCompleted, new object[]
					{
						this.MdbGuid
					});
				}
			}
			return ErrorCode.NoError;
		}

		internal ErrorCode MountDatabase(Context context, MountFlags flags)
		{
			bool flag = false;
			this.GetExclusiveLock();
			StoreDatabase.DatabaseState databaseState = this.state;
			this.ReleaseExclusiveLock();
			ErrorCode result;
			try
			{
				result = this.MountDatabase(context, flags, ref flag);
			}
			finally
			{
				if (flag)
				{
					this.GetExclusiveLock();
					this.SetNewState(databaseState);
					this.ReleaseExclusiveLock();
					if (StoreDatabase.postMoveToNewStateTestHook.Value != null)
					{
						StoreDatabase.postMoveToNewStateTestHook.Value(databaseState, null);
					}
				}
			}
			return result;
		}

		internal ErrorCode DismountDatabase(Context context)
		{
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug<StoreDatabase.DatabaseState>(0L, "Execute DismountDatabase. Current State:{0}", this.state);
			}
			bool flag = false;
			StoreDatabase.DismountOperation dismountOperation = StoreDatabase.DismountOperation.None;
			StoreDatabase.DatabaseState databaseState = StoreDatabase.DatabaseState.OnlineActive;
			try
			{
				this.GetSharedLock();
				if (this.IsOnlineActive)
				{
					this.taskList.StopAllAndPreventFurtherRegistration();
					FaultInjection.InjectFault(StoreDatabase.dismountRequestedStopTasksTestHook);
				}
			}
			finally
			{
				this.ReleaseSharedLock();
			}
			try
			{
				this.GetExclusiveLock();
				if (this.IsTransitioningBetweenOnlinePassiveStates && !this.WaitForTransitionBetweenOnlinePassiveStates())
				{
					this.TraceState((LID)52988U);
					return ErrorCode.CreateMountInProgress((LID)46844U);
				}
				if (this.IsOnlineActive)
				{
					dismountOperation = StoreDatabase.DismountOperation.DismountActive;
					databaseState = StoreDatabase.DatabaseState.OnlineActiveToOffline;
				}
				else if (this.IsOnlinePassiveAttachedReadOnly)
				{
					this.StartForceDetachPassiveDatabase();
					flag = true;
					dismountOperation = StoreDatabase.DismountOperation.DismountPassive;
					databaseState = StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOffline;
				}
				else if (this.IsOnlinePassiveReplayingLogs)
				{
					dismountOperation = StoreDatabase.DismountOperation.DismountPassive;
					databaseState = StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOffline;
				}
				else
				{
					if (this.IsTransitioningToOnline)
					{
						this.TraceState((LID)64764U);
						return ErrorCode.CreateMountInProgress((LID)50104U);
					}
					if (this.IsTransitioningToOffline)
					{
						this.TraceState((LID)40188U);
						return ErrorCode.CreateDismountInProgress((LID)48312U);
					}
					if (this.IsOffline)
					{
						return ErrorCode.NoError;
					}
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "Unexpected database state.");
				}
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(StoreDatabase.DismountOperation.None != dismountOperation, "The dismount operation should be defined.");
				this.SetNewState(databaseState);
				this.ReleaseExclusiveLock();
				if (StoreDatabase.postMoveToNewStateTestHook.Value != null)
				{
					StoreDatabase.postMoveToNewStateTestHook.Value(databaseState, null);
				}
				if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest(3032886589U);
				}
				DiagnosticContext.TraceDword((LID)43072U, (uint)Environment.TickCount);
				using (context.CriticalBlock((LID)30664U, CriticalBlockScope.Database))
				{
					switch (dismountOperation)
					{
					case StoreDatabase.DismountOperation.DismountActive:
						this.DismountActive(context);
						break;
					case StoreDatabase.DismountOperation.DismountPassive:
						if (flag)
						{
							this.FinishForceDetachPassiveDatabase(context);
						}
						this.DismountPassive(context);
						break;
					}
					DiagnosticContext.TraceDword((LID)59456U, (uint)Environment.TickCount);
					FaultInjection.InjectFault(this.databaseTestHook);
					context.EndCriticalBlock();
				}
				this.GetExclusiveLock();
				this.SetNewState(StoreDatabase.DatabaseState.Offline);
			}
			finally
			{
				if (this.IsExclusiveLockHeld())
				{
					this.ReleaseExclusiveLock();
				}
			}
			if (StoreDatabase.postMoveToNewStateTestHook.Value != null)
			{
				StoreDatabase.postMoveToNewStateTestHook.Value(StoreDatabase.DatabaseState.Offline, null);
			}
			return ErrorCode.NoError;
		}

		internal void FinalizeDatabaseDismount(Context context)
		{
			DiagnosticContext.TraceDword((LID)55360U, (uint)Environment.TickCount);
			if (this.taskList != null)
			{
				this.taskList.Dispose();
				this.taskList = null;
			}
			try
			{
				this.InvokeDismountingHandler(context);
			}
			finally
			{
				DiagnosticContext.TraceDword((LID)38976U, (uint)Environment.TickCount);
				this.physicalDatabase.Close();
				DiagnosticContext.TraceDword((LID)42048U, (uint)Environment.TickCount);
			}
		}

		internal void InvokeDismountingHandler(Context context)
		{
			using (context.AssociateWithDatabaseNoLock(this))
			{
				if (this.taskList != null)
				{
					this.taskList.Dispose();
					this.taskList = null;
				}
				StoreDatabase.DismountingHandler(context, this);
				context.Commit();
			}
		}

		internal void StartBackgroundChecksumming(Context context)
		{
			this.PhysicalDatabase.StartBackgroundChecksumming(context);
		}

		internal void VersionStoreCleanup(Context context)
		{
			this.PhysicalDatabase.VersionStoreCleanup(context);
		}

		internal Guid GetDatabaseDsGuid(Context context)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(AddDatabaseDsGuidToGlobalsTable.IsReady(context, this), "Schema not ready.");
			GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(this);
			Column[] columnsToFetch = new Column[]
			{
				globalsTable.DatabaseDsGuid
			};
			Guid guid;
			using (TableOperator tableOperator = Factory.CreateTableOperator(CultureHelper.DefaultCultureInfo, context, globalsTable.Table, globalsTable.Table.PrimaryKeyIndex, columnsToFetch, null, null, 0, 1, KeyRange.AllRows, false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					reader.Read();
					guid = reader.GetGuid(globalsTable.DatabaseDsGuid);
				}
			}
			return guid;
		}

		internal void SetDatabaseDsGuid(Context context, Guid databaseDsGuid)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(AddDatabaseDsGuidToGlobalsTable.IsReady(context, this), "Schema not ready.");
			GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(this);
			Column[] columnsToUpdate = new Column[]
			{
				globalsTable.DatabaseDsGuid
			};
			List<object> list = new List<object>(1);
			list.Add(databaseDsGuid);
			using (UpdateOperator updateOperator = Factory.CreateUpdateOperator(CultureHelper.DefaultCultureInfo, context, Factory.CreateTableOperator(CultureHelper.DefaultCultureInfo, context, globalsTable.Table, globalsTable.Table.PrimaryKeyIndex, null, null, null, 0, 1, KeyRange.AllRows, false, true), columnsToUpdate, list, true))
			{
				int num = (int)updateOperator.ExecuteScalar();
			}
		}

		internal IDisposable SetDatabaseTestHook(Action action)
		{
			return this.databaseTestHook.SetTestHook(action);
		}

		internal void PublishHaFailure(FailureTag failureTag)
		{
			this.PhysicalDatabase.PublishHaFailure(failureTag);
		}

		private static LockName<Guid> GetDatabaseLockName(Guid databaseGuid)
		{
			return new LockName<Guid>(databaseGuid, LockManager.LockLevel.Database);
		}

		private void SetNewState(StoreDatabase.DatabaseState newState)
		{
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug<StoreDatabase.DatabaseState, StoreDatabase.DatabaseState>(0L, "SetNewState:{0}, Current State:{1}.", newState, this.state);
			}
			if (StoreDatabase.preMoveToNewStateTestHook.Value != null)
			{
				StoreDatabase.preMoveToNewStateTestHook.Value(this.state, newState);
			}
			switch (newState)
			{
			case StoreDatabase.DatabaseState.Offline:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.IsTransitioningToOffline || this.IsTransitioningFromOffline, "Unexpected current state transitioning to Offline.");
				break;
			case StoreDatabase.DatabaseState.OnlineActive:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.IsTransitioningToOnlineActive, "Unexpected current state transitioning to OnlineActive.");
				break;
			case StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnly:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.state == StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOnlinePassiveAttachedReadOnly, "Unexpected current state transitioning to OnlinePassiveAttachedReadOnly.");
				break;
			case StoreDatabase.DatabaseState.OnlinePassiveReplayingLogs:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.state == StoreDatabase.DatabaseState.OfflineToOnlinePassiveReplayingLogs || this.state == StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOnlinePassiveReplayingLogs, "Unexpected current state transitioning to OnlinePassiveAttachedReadOnly.");
				break;
			case StoreDatabase.DatabaseState.OfflineToOnlineActive:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.IsOffline, "Unexpected current state before setting to transition state OfflineToOnlineActive.");
				break;
			case StoreDatabase.DatabaseState.OfflineToOnlinePassiveReplayingLogs:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.IsOffline, "Unexpected current state before setting to transition state OfflineToOnlinePassiveReplayingLogs.");
				break;
			case StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOnlineActive:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.state == StoreDatabase.DatabaseState.OnlinePassiveReplayingLogs, "Unexpected current state before setting to transition state OnlinePassiveReplayingLogsToOnlineActive.");
				break;
			case StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOnlineActive:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.state == StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnly, "Unexpected current state before setting to transition state OnlinePassiveAttachedReadOnlyToOnlineActive.");
				break;
			case StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOnlinePassiveAttachedReadOnly:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.state == StoreDatabase.DatabaseState.OnlinePassiveReplayingLogs, "Unexpected current state before setting to transition state OnlinePassiveReplayingLogsToOnlinePassiveAttachedReadOnly.");
				break;
			case StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOnlinePassiveReplayingLogs:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.state == StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnly, "Unexpected current state before setting to transition state OnlinePassiveAttachedReadOnlyToOnlinePassiveReplayingLogs.");
				break;
			case StoreDatabase.DatabaseState.OnlineActiveToOffline:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.state == StoreDatabase.DatabaseState.OnlineActive, "Unexpected current state before setting to transition state OnlineActiveToOffline.");
				break;
			case StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOffline:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.state == StoreDatabase.DatabaseState.OnlinePassiveReplayingLogs, "Unexpected current state before setting to transition state OnlinePassiveReplayingLogsToOffline.");
				break;
			case StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOffline:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.state == StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnly, "Unexpected current state before setting to transition state OnlinePassiveAttachedReadOnlyToOffline.");
				break;
			default:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "Unexpected database state.");
				break;
			}
			this.state = newState;
		}

		private void SetStatusFlag(DatabaseStatus flag)
		{
			this.mdbStatus |= flag;
		}

		private void ResetStatusFlag(DatabaseStatus flag)
		{
			this.mdbStatus &= ~flag;
		}

		private void MountActive(Context context, bool allowLoss)
		{
			StoreDatabase.<>c__DisplayClass6 CS$<>8__locals1 = new StoreDatabase.<>c__DisplayClass6();
			CS$<>8__locals1.context = context;
			CS$<>8__locals1.<>4__this = this;
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "Mount the database as active.");
			}
			bool flag = false;
			try
			{
				this.physicalDatabase.Configure();
				using (CS$<>8__locals1.context.AssociateWithDatabaseNoLock(this))
				{
					if (!this.physicalDatabase.TryOpen(allowLoss))
					{
						ILUtil.DoTryFilterCatch<IExecutionDiagnostics>(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<MountActive>b__0)), new GenericFilterDelegate<IExecutionDiagnostics>(CS$<>8__locals1, (UIntPtr)ldftn(<MountActive>b__1)), new GenericCatchDelegate<IExecutionDiagnostics>(null, (UIntPtr)ldftn(<MountActive>b__2)), CS$<>8__locals1.context.Diagnostics);
					}
					this.FinishMount(CS$<>8__locals1.context, false);
					CS$<>8__locals1.context.Commit();
					CS$<>8__locals1.context.GetConnection().FlushDatabaseLogs(true);
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.physicalDatabase.Close();
				}
			}
		}

		private void FinishMount(Context context, bool readOnly)
		{
			StoreDatabase.InitInMemoryDatabaseSchemaHandler(context, this);
			this.CheckDatabaseVersionAndUpgrade(context, readOnly);
			if (AddDatabaseDsGuidToGlobalsTable.IsReady(context, this))
			{
				Guid databaseDsGuid = this.GetDatabaseDsGuid(context);
				if (!databaseDsGuid.Equals(this.MdbGuid))
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_DatabaseGuidPatchRequired, new object[]
					{
						this.MdbGuid,
						databaseDsGuid,
						this.MdbName
					});
					if (!this.allowFileRestore || readOnly)
					{
						Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_DatabaseFileRestoreNotAllowed, new object[]
						{
							this.MdbName
						});
						throw new StoreException((LID)39200U, ErrorCodeValue.DatabaseRolledBack);
					}
					this.SetDatabaseDsGuid(context, this.MdbGuid);
				}
				this.allowFileRestore = false;
			}
			if (!readOnly)
			{
				this.StartBackgroundChecksumming(context);
			}
			this.MountTime = DateTime.UtcNow;
			this.taskList = new TaskList();
			StoreDatabase.MountingHandler(context, this, readOnly);
			this.databaseHeaderInfo = this.PhysicalDatabase.GetDatabaseHeaderInfo(context.GetConnection());
			this.CheckForRepairedDatabase(context);
		}

		private void CheckForRepairedDatabase(Context context)
		{
			bool databaseRepaired = this.databaseHeaderInfo.DatabaseRepaired;
			context.Diagnostics.DatabaseRepaired = new bool?(databaseRepaired);
			if (databaseRepaired)
			{
				if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "WARNING: Database {0} was previously offline-repaired: lastRepairTime={1}, repairCountSinceLastDefrag=[2}, repairCountBeforeLastDefrag={3}", new object[]
					{
						this.MdbName,
						this.databaseHeaderInfo.LastRepairedTime,
						this.databaseHeaderInfo.RepairCountSinceLastDefrag,
						this.databaseHeaderInfo.RepairCountBeforeLastDefrag
					});
				}
				bool isRecoveryDatabase = (this.MdbStatus & DatabaseStatus.ForRecovery) != DatabaseStatus.OffLine;
				RecurringTask<StoreDatabase> task = new RecurringTask<StoreDatabase>(delegate(TaskExecutionDiagnosticsProxy diagnosticsContext, StoreDatabase storeDatabase, Func<bool> shouldCallbackContinue)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_RunningWithRepairedDatabase, new object[]
					{
						storeDatabase.MdbName,
						storeDatabase.MdbGuid,
						storeDatabase.databaseHeaderInfo.LastRepairedTime,
						storeDatabase.databaseHeaderInfo.RepairCountSinceLastDefrag,
						storeDatabase.databaseHeaderInfo.RepairCountBeforeLastDefrag,
						isRecoveryDatabase
					});
				}, this, StoreDatabase.FrequencyForReportingRepairedDatabase);
				this.TaskList.Add(task, true);
			}
		}

		private void PostMountInitialization(Context context)
		{
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "Execute PostMountInitialization");
			}
			this.GetSharedLock(context.Diagnostics);
			try
			{
				if (this.IsOnlineActive || this.IsOnlinePassiveAttachedReadOnly)
				{
					using (context.AssociateWithDatabaseNoLock(this))
					{
						StoreDatabase.MountedHandler(context, this);
						context.Commit();
					}
				}
			}
			finally
			{
				this.ReleaseSharedLock();
			}
		}

		private void DismountActive(Context context)
		{
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "Dismount the active database.");
			}
			this.FinalizeDatabaseDismount(context);
		}

		private void CreateGlobalsTableRow(IConnectionProvider connectionProvider)
		{
			GlobalsTable globalsTable = new GlobalsTable();
			List<ColumnValue> list = new List<ColumnValue>(10);
			int value;
			if (this.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
			{
				value = StoreDatabase.GetMinimumSchemaVersion().Value;
			}
			else
			{
				value = StoreDatabase.GetMaximumSchemaVersion().Value;
				list.Add(new ColumnValue(globalsTable.DatabaseDsGuid, this.MdbGuid));
				list.Add(new ColumnValue(globalsTable.EventCounterLowerBound, 0L));
				list.Add(new ColumnValue(globalsTable.EventCounterUpperBound, 1L));
			}
			list.Add(new ColumnValue(globalsTable.DatabaseVersion, value));
			list.Add(new ColumnValue(globalsTable.VersionName, "Exchange"));
			list.Add(new ColumnValue(globalsTable.Inid, 0L));
			list.Add(new ColumnValue(globalsTable.LastMaintenanceTask, 0));
			using (DataRow dataRow = Factory.CreateDataRow(CultureHelper.DefaultCultureInfo, connectionProvider, globalsTable.Table, true, list.ToArray()))
			{
				dataRow.Flush(connectionProvider);
			}
		}

		private void MountPassive(Context context)
		{
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "Mount the database as passive");
			}
			this.ResetPassiveDatabaseAttachDetach();
			this.replayThreadMayAttachPassiveDatabaseEventHandle = new ManualResetEvent(false);
			this.physicalDatabase.Configure();
			using (context.AssociateWithDatabaseNoLock(this))
			{
				this.physicalDatabase.StartLogReplay(new Func<bool, bool>(this.PassiveDatabaseAttachDetachHandler));
				context.Commit();
			}
			if (Microsoft.Exchange.Server.Storage.Common.Globals.IsMultiProcess)
			{
				PerformanceCounterFactory.CreateDatabaseInstance(this);
			}
		}

		private void ActivatePassive(Context context)
		{
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "Mount a passive database as active.");
			}
			bool flag = false;
			try
			{
				this.physicalDatabase.FinishLogReplay();
				this.ResetPassiveDatabaseAttachDetach();
				using (context.AssociateWithDatabaseNoLock(this))
				{
					this.FinishMount(context, false);
					context.Commit();
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.physicalDatabase.Close();
				}
			}
		}

		private void DismountPassive(Context context)
		{
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "Dismount the passive database.");
			}
			this.physicalDatabase.CancelLogReplay();
			this.ResetPassiveDatabaseAttachDetach();
			this.FinalizeDatabaseDismount(context);
		}

		private void ResetPassiveDatabaseAttachDetach()
		{
			this.replayThreadMayAttachPassiveDatabase = false;
			if (this.replayThreadMayAttachPassiveDatabaseEventHandle != null)
			{
				this.replayThreadMayAttachPassiveDatabaseEventHandle.Dispose();
				this.replayThreadMayAttachPassiveDatabaseEventHandle = null;
			}
			if (this.foregroundThreadForceDetachingPassiveDatabaseEventHandle != null)
			{
				this.foregroundThreadForceDetachingPassiveDatabaseEventHandle.Dispose();
				this.foregroundThreadForceDetachingPassiveDatabaseEventHandle = null;
			}
		}

		private bool PassiveDatabaseAttachDetachHandler(bool attachDatabase)
		{
			bool result;
			try
			{
				result = (attachDatabase ? this.PassiveDatabaseAttachHandler() : this.PassiveDatabaseDetachHandler());
			}
			catch (Exception ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				this.HandlePassiveDatabaseAttachDetachException(ex);
				throw;
			}
			return result;
		}

		private void HandlePassiveDatabaseAttachDetachException(Exception e)
		{
			if (!this.IsExclusiveLockHeld())
			{
				this.GetExclusiveLock();
			}
			StoreDatabase.DatabaseState databaseState = this.state;
			this.state = StoreDatabase.DatabaseState.OnlinePassiveReplayingLogs;
			this.ReleaseExclusiveLock();
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug<StoreDatabase.DatabaseState, Exception>(0L, "Exception raised while attaching or detaching the passive database: Current state: {0}, Exception: {1}", databaseState, e);
			}
			Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_PassiveDatabaseAttachDetachException, new object[]
			{
				this.MdbName,
				this.MdbGuid,
				databaseState,
				e
			});
		}

		private bool PassiveDatabaseAttachHandler()
		{
			bool flag = false;
			bool result = false;
			this.GetExclusiveLock();
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug<StoreDatabase.DatabaseState>(0L, "Recovery callback invoked to mount the passive database for read-only access (current state: {0}).", this.state);
			}
			if (!this.replayThreadMayAttachPassiveDatabase)
			{
				this.ReleaseExclusiveLock();
				bool assertCondition = this.replayThreadMayAttachPassiveDatabaseEventHandle.WaitOne(TimeSpan.FromMinutes(1.0));
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "Waited an abnormally long time for the foreground mount thread to set the database into the OnlinePassiveReplayingLogs state.");
				this.GetExclusiveLock();
			}
			if (this.IsOnlinePassiveReplayingLogs)
			{
				if (this.replayThreadMayAttachPassiveDatabaseEventHandle != null)
				{
					this.replayThreadMayAttachPassiveDatabaseEventHandle.Dispose();
					this.replayThreadMayAttachPassiveDatabaseEventHandle = null;
				}
				if (this.physicalDatabase.CheckTableExists("Globals"))
				{
					this.SetNewState(StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOnlinePassiveAttachedReadOnly);
					flag = true;
				}
				else if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "Cannot attach to passive database because the database does not contain all initial schema objects yet.");
				}
			}
			this.ReleaseExclusiveLock();
			if (flag)
			{
				uint currentLogReplayGeneration = this.GetCurrentLogReplayGeneration();
				if (StoreDatabase.postMoveToNewStateTestHook.Value != null)
				{
					StoreDatabase.postMoveToNewStateTestHook.Value(StoreDatabase.DatabaseState.OnlinePassiveReplayingLogsToOnlinePassiveAttachedReadOnly, currentLogReplayGeneration);
				}
				using (Context context = Context.CreateForSystem())
				{
					using (context.AssociateWithDatabaseNoLock(this))
					{
						this.FinishMount(context, true);
						context.Commit();
					}
					this.GetExclusiveLock();
					this.SetNewState(StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnly);
					this.ReleaseExclusiveLock();
					if (StoreDatabase.postMoveToNewStateTestHook.Value != null)
					{
						StoreDatabase.postMoveToNewStateTestHook.Value(StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnly, this.GetCurrentLogReplayGeneration());
					}
					this.PostMountInitialization(context);
				}
				result = true;
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_PassiveDatabaseAttachedReadOnly, new object[]
				{
					this.MdbName,
					this.MdbGuid,
					string.Format("0x{0:X}", currentLogReplayGeneration)
				});
				if (StoreDatabase.passiveAttachedDetachedTestHook.Value != null)
				{
					StoreDatabase.passiveAttachedDetachedTestHook.Value(true, this.GetCurrentLogReplayGeneration());
				}
			}
			return result;
		}

		private bool PassiveDatabaseDetachHandler()
		{
			bool flag = false;
			bool result = false;
			this.GetExclusiveLock();
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug<StoreDatabase.DatabaseState>(0L, "Recovery callback invoked to dismount the read-only passive database (current state: {0}).", this.state);
			}
			if (this.foregroundThreadForceDetachingPassiveDatabaseEventHandle != null)
			{
				this.ReleaseExclusiveLock();
				bool assertCondition = this.foregroundThreadForceDetachingPassiveDatabaseEventHandle.WaitOne(TimeSpan.FromMinutes(1.0));
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "Waited an abnormally long time for the foreground thread to force-detach the passive database.");
				if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "Recovery callback cannot dismount the read-only passive database because it has already been force-detached.");
				}
				return true;
			}
			if (this.IsOnlinePassiveAttachedReadOnly)
			{
				this.SetNewState(StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOnlinePassiveReplayingLogs);
				flag = true;
			}
			this.ReleaseExclusiveLock();
			if (flag)
			{
				uint currentLogReplayGeneration = this.GetCurrentLogReplayGeneration();
				if (StoreDatabase.postMoveToNewStateTestHook.Value != null)
				{
					StoreDatabase.postMoveToNewStateTestHook.Value(StoreDatabase.DatabaseState.OnlinePassiveAttachedReadOnlyToOnlinePassiveReplayingLogs, currentLogReplayGeneration);
				}
				using (Context context = Context.CreateForSystem())
				{
					this.InvokeDismountingHandler(context);
				}
				this.GetExclusiveLock();
				this.SetNewState(StoreDatabase.DatabaseState.OnlinePassiveReplayingLogs);
				this.ReleaseExclusiveLock();
				if (StoreDatabase.postMoveToNewStateTestHook.Value != null)
				{
					StoreDatabase.postMoveToNewStateTestHook.Value(StoreDatabase.DatabaseState.OnlinePassiveReplayingLogs, this.GetCurrentLogReplayGeneration());
				}
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_PassiveDatabaseDetached, new object[]
				{
					this.MdbName,
					this.MdbGuid,
					string.Format("0x{0:X}", currentLogReplayGeneration)
				});
				if (StoreDatabase.passiveAttachedDetachedTestHook.Value != null)
				{
					StoreDatabase.passiveAttachedDetachedTestHook.Value(false, this.GetCurrentLogReplayGeneration());
				}
			}
			return result;
		}

		private void PermitReplayThreadAttachPassiveDatabase()
		{
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "Signalling log replay thread that the passive database was transitioned to the PassiveReplayingLogs state.");
			}
			this.replayThreadMayAttachPassiveDatabase = true;
			bool assertCondition = this.replayThreadMayAttachPassiveDatabaseEventHandle.Set();
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "Setting the event must succeed.");
			if (StoreDatabase.permitReplayThreadAttachPassiveDatabaseTestHook.Value != null)
			{
				StoreDatabase.permitReplayThreadAttachPassiveDatabaseTestHook.Value(this);
			}
		}

		private void StartForceDetachPassiveDatabase()
		{
			this.foregroundThreadForceDetachingPassiveDatabaseEventHandle = new ManualResetEvent(false);
		}

		private void FinishForceDetachPassiveDatabase(Context context)
		{
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "Force-detaching the passive database.");
			}
			this.InvokeDismountingHandler(context);
			if (StoreDatabase.passiveAttachedDetachedTestHook.Value != null)
			{
				StoreDatabase.passiveAttachedDetachedTestHook.Value(false, this.GetCurrentLogReplayGeneration());
			}
			bool assertCondition = this.foregroundThreadForceDetachingPassiveDatabaseEventHandle.Set();
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "Setting the signal must succeed.");
		}

		private bool WaitForTransitionBetweenOnlinePassiveStates()
		{
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug(0L, "Starting to wait for the database to complete a transition between online passive states.");
			}
			if (StoreDatabase.waitForTransitionBetweenOnlinePassiveStatesTestHook.Value != null)
			{
				StoreDatabase.waitForTransitionBetweenOnlinePassiveStatesTestHook.Value();
			}
			int num = 0;
			while (this.IsTransitioningBetweenOnlinePassiveStates)
			{
				num++;
				bool flag = StoreDatabase.simulateTimeoutWaitingForTransitionBetweenOnlinePassiveStatesTestHook.Value(num);
				if (num > 2000 || flag)
				{
					if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.StoreDatabaseTracer.TraceDebug<StoreDatabase.DatabaseState>(0L, "Failed waiting because the online passive state transition appeared to be taking an abnormally long time (current state: {0}).", this.state);
					}
					return false;
				}
				this.ReleaseExclusiveLock();
				Thread.Sleep(TimeSpan.FromMilliseconds(10.0));
				this.GetExclusiveLock();
			}
			if (ExTraceGlobals.StoreDatabaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StoreDatabaseTracer.TraceDebug<int>(0L, "Successfully finished waiting for the database to complete a transition between online passive states (retryCount=={0}).", num);
			}
			return true;
		}

		private uint GetCurrentLogReplayGeneration()
		{
			uint result;
			byte[] array;
			uint num;
			byte[] array2;
			byte[] array3;
			uint[] array4;
			this.physicalDatabase.LogReplayStatus.GetReplayStatus(out result, out array, out num, out array2, out array3, out array4);
			return result;
		}

		internal void CheckDatabaseVersionAndUpgrade(Context context, bool readOnly)
		{
			ComponentVersion minimumSchemaVersion = StoreDatabase.GetMinimumSchemaVersion();
			ComponentVersion maximumSchemaVersion = StoreDatabase.GetMaximumSchemaVersion();
			ComponentVersion componentVersion = this.GetCurrentSchemaVersion(context);
			if (componentVersion.Value < minimumSchemaVersion.Value)
			{
				this.PublishHaFailure(FailureTag.Configuration);
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_DatabaseVersionTooOld, new object[]
				{
					this.MdbGuid,
					componentVersion,
					minimumSchemaVersion
				});
				throw new DatabaseBadVersion((LID)55584U, this.MdbGuid, minimumSchemaVersion, componentVersion);
			}
			if (componentVersion.Value > maximumSchemaVersion.Value)
			{
				this.PublishHaFailure(FailureTag.Configuration);
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_DatabaseVersionTooNew, new object[]
				{
					this.MdbGuid,
					componentVersion,
					maximumSchemaVersion
				});
				throw new DatabaseBadVersion((LID)50464U, this.MdbGuid, maximumSchemaVersion, componentVersion);
			}
			ComponentVersion requestedSchemaVersion = this.GetRequestedSchemaVersion(context, componentVersion, maximumSchemaVersion);
			if (requestedSchemaVersion.Value < componentVersion.Value || requestedSchemaVersion.Value > maximumSchemaVersion.Value)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_DatabaseBadRequestedUpdgradeVersion, new object[]
				{
					this.MdbGuid,
					requestedSchemaVersion,
					componentVersion,
					maximumSchemaVersion
				});
				return;
			}
			if (readOnly)
			{
				return;
			}
			SchemaUpgradeService.Upgrade(context, this, SchemaUpgradeService.SchemaCategory.Database, requestedSchemaVersion);
		}

		private const string RequestedVersion = "Requested Version";

		private static readonly TimeSpan FrequencyForReportingRepairedDatabase = TimeSpan.FromHours(1.0);

		private static Hookable<ComponentVersion> maximumSchemaVersionTestHook = Hookable<ComponentVersion>.Create(true, (Factory.GetDatabaseType() == DatabaseType.Jet) ? DefaultSettings.Get.MaximumSupportableDatabaseSchemaVersion : new ComponentVersion(int.MaxValue));

		private static readonly Hookable<Action<bool, uint>> passiveAttachedDetachedTestHook = Hookable<Action<bool, uint>>.Create(true, null);

		private static readonly Hookable<Action<StoreDatabase>> permitReplayThreadAttachPassiveDatabaseTestHook = Hookable<Action<StoreDatabase>>.Create(true, null);

		private static Hookable<ComponentVersion> minimumSchemaVersionTestHook = Hookable<ComponentVersion>.Create(true, new ComponentVersion(0, 121));

		private static Hookable<Action> dismountRequestedStopTasksTestHook = Hookable<Action>.Create(true, null);

		private static readonly Hookable<Func<bool, bool>> isDatabaseEngineBusyTestHook = Hookable<Func<bool, bool>>.Create(true, null);

		private static Hookable<Action<StoreDatabase.DatabaseState, StoreDatabase.DatabaseState>> preMoveToNewStateTestHook = Hookable<Action<StoreDatabase.DatabaseState, StoreDatabase.DatabaseState>>.Create(true, null);

		private static Hookable<Action<StoreDatabase.DatabaseState, object>> postMoveToNewStateTestHook = Hookable<Action<StoreDatabase.DatabaseState, object>>.Create(true, null);

		private static Hookable<Action> waitForTransitionBetweenOnlinePassiveStatesTestHook = Hookable<Action>.Create(true, null);

		private static Hookable<Func<int, bool>> simulateTimeoutWaitingForTransitionBetweenOnlinePassiveStatesTestHook = Hookable<Func<int, bool>>.Create(true, (int retryCount) => false);

		private static Hookable<Action> databaseCreationTestHook = Hookable<Action>.Create(true, null);

		private readonly Database physicalDatabase;

		private readonly LockName<Guid> lockName;

		private readonly Guid dagOrServerGuid;

		private readonly string serverName;

		private readonly string legacyDN;

		private readonly string description;

		private readonly TimeSpan eventHistoryRetentionPeriod;

		private readonly bool hostServerIsDAGMember;

		private ComponentVersion? currentSchemaVersion;

		private TaskList taskList;

		private StoreDatabase.DatabaseState state;

		private DatabaseStatus mdbStatus;

		private StoreDatabase.ComponentDataStorage componentDataStorage;

		private DateTime mountTime;

		private Hookable<Action> databaseTestHook = Hookable<Action>.Create(true, null);

		private int exclusiveLockContentionCounter;

		private ResourceMonitorDigest resourceDigest;

		private ServerEditionType serverEdition;

		private bool allowFileRestore;

		private readonly string forestName;

		private DatabaseHeaderInfo databaseHeaderInfo;

		private EventWaitHandle replayThreadMayAttachPassiveDatabaseEventHandle;

		private bool replayThreadMayAttachPassiveDatabase;

		private EventWaitHandle foregroundThreadForceDetachingPassiveDatabaseEventHandle;

		public delegate void InitInMemoryDatabaseSchemaHandlerDelegate(Context context, StoreDatabase database);

		public delegate void MountingHandlerDelegate(Context context, StoreDatabase database, bool readOnly);

		public delegate void MountedHandlerDelegate(Context context, StoreDatabase database);

		public delegate void DismountingHandlerDelegate(Context context, StoreDatabase database);

		[Flags]
		internal enum MountOperation
		{
			None = 0,
			MountActive = 1,
			MountPassive = 2,
			ActivatePassive = 3
		}

		[Flags]
		internal enum DismountOperation
		{
			None = 0,
			DismountActive = 1,
			DismountPassive = 2
		}

		public enum DatabaseState
		{
			Offline,
			OnlineActive,
			OnlinePassiveAttachedReadOnly,
			OnlinePassiveReplayingLogs,
			OfflineToOnlineActive,
			OfflineToOnlinePassiveReplayingLogs,
			OnlinePassiveReplayingLogsToOnlineActive,
			OnlinePassiveAttachedReadOnlyToOnlineActive,
			OnlinePassiveReplayingLogsToOnlinePassiveAttachedReadOnly,
			OnlinePassiveAttachedReadOnlyToOnlinePassiveReplayingLogs,
			OnlineActiveToOffline,
			OnlinePassiveReplayingLogsToOffline,
			OnlinePassiveAttachedReadOnlyToOffline
		}

		public class ComponentDataStorage : ComponentDataStorageBase
		{
			public ComponentDataStorage(StoreDatabase database)
			{
				this.database = database;
			}

			public static bool SkipDatabaseStateValidation
			{
				get
				{
					return StoreDatabase.ComponentDataStorage.skipDatabaseStateValidation;
				}
				set
				{
					StoreDatabase.ComponentDataStorage.skipDatabaseStateValidation = value;
				}
			}

			public new object this[int slotNumber]
			{
				get
				{
					return base[slotNumber];
				}
				set
				{
					base[slotNumber] = value;
				}
			}

			internal static int AllocateSlot()
			{
				return Interlocked.Increment(ref StoreDatabase.ComponentDataStorage.nextAvailableSlot) - 1;
			}

			internal override int SlotCount
			{
				get
				{
					return StoreDatabase.ComponentDataStorage.nextAvailableSlot;
				}
			}

			private static int nextAvailableSlot;

			private static bool skipDatabaseStateValidation;

			private StoreDatabase database;
		}
	}
}
