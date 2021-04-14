using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Isam.Esent.Interop.Vista;
using Microsoft.Isam.Esent.Interop.Windows8;
using Microsoft.Isam.Esent.Interop.Windows81;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetDatabase : Database
	{
		internal JetDatabase(Guid dbGuid, string displayName, string logPath, string filePath, string fileName, DatabaseFlags databaseFlags, DatabaseOptions databaseOptions) : base(displayName, logPath, filePath, fileName, databaseFlags, databaseOptions)
		{
			this.databaseFile = Path.Combine(base.FilePath, base.FileName);
			this.DbGuid = dbGuid;
			this.InternalInitializeJetDatabase();
		}

		public static int MaxSessions
		{
			get
			{
				return JetDatabase.maxSessions;
			}
			internal set
			{
				JetDatabase.maxSessions = value;
			}
		}

		public static int MaxOpenTables
		{
			get
			{
				return JetDatabase.maxOpenTables;
			}
			internal set
			{
				JetDatabase.maxOpenTables = value;
			}
		}

		public static int MaxCursors
		{
			get
			{
				return JetDatabase.maxCursors;
			}
			internal set
			{
				JetDatabase.maxCursors = value;
			}
		}

		public static int MaxTemporaryTables
		{
			get
			{
				return JetDatabase.maxTemporaryTables;
			}
			internal set
			{
				JetDatabase.maxTemporaryTables = value;
			}
		}

		public static int MaxVersionBuckets
		{
			get
			{
				return JetDatabase.maxVersionBuckets;
			}
			internal set
			{
				JetDatabase.maxVersionBuckets = value;
			}
		}

		public static int PreferredVersionBuckets
		{
			get
			{
				return JetDatabase.preferredVersionBuckets;
			}
			internal set
			{
				JetDatabase.preferredVersionBuckets = value;
			}
		}

		public static int MaxBackgroundCleanupTasks
		{
			get
			{
				return JetDatabase.maxBackgroundCleanupTasks;
			}
			internal set
			{
				JetDatabase.maxBackgroundCleanupTasks = value;
			}
		}

		public static int LogBuffers
		{
			get
			{
				return JetDatabase.logBuffers;
			}
			internal set
			{
				JetDatabase.logBuffers = value;
			}
		}

		public static int LogFileSize
		{
			get
			{
				return JetDatabase.logFileSize;
			}
			internal set
			{
				JetDatabase.logFileSize = value;
			}
		}

		public static int DatabaseExtensionSize
		{
			get
			{
				return JetDatabase.databaseExtensionSize;
			}
			internal set
			{
				JetDatabase.databaseExtensionSize = value;
			}
		}

		public static int StartFlushPct
		{
			get
			{
				return JetDatabase.startFlushPct;
			}
			internal set
			{
				JetDatabase.startFlushPct = value;
			}
		}

		public static int StopFlushPct
		{
			get
			{
				return JetDatabase.stopFlushPct;
			}
			internal set
			{
				JetDatabase.stopFlushPct = value;
			}
		}

		public static int MaxInstances
		{
			get
			{
				return JetDatabase.maxInstances;
			}
			internal set
			{
				JetDatabase.maxInstances = value;
			}
		}

		public static int EventLoggingLevel
		{
			get
			{
				return JetDatabase.eventLoggingLevel;
			}
			internal set
			{
				JetDatabase.eventLoggingLevel = value;
			}
		}

		public override int PageSize
		{
			get
			{
				return 32768;
			}
		}

		public override DatabaseType DatabaseType
		{
			get
			{
				return DatabaseType.Jet;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.DbGuid;
			}
		}

		internal JET_INSTANCE JetInstance
		{
			get
			{
				return this.jetInstance;
			}
		}

		internal Instance EseInteropInstance
		{
			get
			{
				return this.jetInstance;
			}
		}

		internal string DatabaseFile
		{
			get
			{
				return this.databaseFile;
			}
		}

		public override void Configure()
		{
			if (!JetDatabase.initializedJet)
			{
				using (LockManager.Lock(JetDatabase.configurationLockObject))
				{
					if (!JetDatabase.initializedJet)
					{
						this.InternalConfigureEseStaging();
						JetDatabase.SetTableClassNames();
						SystemParameters.DatabasePageSize = this.PageSize;
						SystemParameters.EnableAdvanced = true;
						SystemParameters.EventLoggingLevel = JetDatabase.EventLoggingLevel;
						SystemParameters.MaxInstances = JetDatabase.MaxInstances;
						SystemParameters.MinDataForXpress = 32;
						SystemParameters.OutstandingIOMax = 64;
						JetDatabase.SetEseCachePageReplacementParameters();
						this.InternalConfigureHungIO();
						string text = "Information Store - " + base.DisplayName;
						SystemParameters.ProcessFriendlyName = text.Substring(0, Math.Min(64, text.Length));
						this.InternalSetEmitCallback();
						JetDatabase.SetJetTraceTags();
						ExTraceConfiguration.Instance.OnConfigurationChange += JetDatabase.SetJetTraceTags;
						ETWTrace.OnTraceStateChange += JetDatabase.SetJetTraceTags;
						JetDatabase.initializedJet = true;
					}
				}
			}
		}

		public override bool TryOpen(bool lossyMount)
		{
			this.lossyMount = lossyMount;
			if (!File.Exists(this.DatabaseFile))
			{
				return false;
			}
			try
			{
				DiagnosticContext.TraceDword((LID)57728U, (uint)Environment.TickCount);
				this.InitializeJetInstance(JetDatabase.InitType.Normal);
				DiagnosticContext.TraceDword((LID)53632U, (uint)Environment.TickCount);
				this.AttachDatabase();
				DiagnosticContext.TraceDword((LID)37248U, (uint)Environment.TickCount);
			}
			catch (EsentErrorException e)
			{
				this.Close();
				throw this.ProcessJetError(JetDatabase.InitType.Normal, (LID)34760U, "JetDatabase.TryOpen", e);
			}
			return true;
		}

		public override bool TryCreate(bool force)
		{
			string text = this.DatabaseFile;
			if (File.Exists(text))
			{
				if (!force)
				{
					return false;
				}
				base.DeleteFileOrDirectory(text);
			}
			base.CreateDirectory(base.FilePath);
			base.CreateDirectory(base.LogPath);
			this.DeleteLogfiles();
			try
			{
				this.InitializeJetInstance(JetDatabase.InitType.Normal);
				using (Session session = new Session(this.jetInstance))
				{
					bool flag = true;
					try
					{
						if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.TraceDebug(0L, "TryCreate: calling JetCreateDatabase.");
						}
						JET_DBID jet_DBID;
						Api.JetCreateDatabase(session, text, null, out jet_DBID, CreateDatabaseGrbit.None);
						flag = false;
					}
					finally
					{
						if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.TraceDebug<string>(0L, "TryCreate: JetCreateDatabase {0}.", flag ? "thrown exception" : "returned succesfully");
						}
					}
				}
			}
			catch (EsentErrorException e)
			{
				this.Close();
				throw this.ProcessJetError(JetDatabase.InitType.Normal, (LID)51144U, "JetDatabase.TryCreate", e);
			}
			return true;
		}

		public override void Close()
		{
			this.DismountBegins();
			DiagnosticContext.TraceDword((LID)46144U, (uint)Environment.TickCount);
			if (this.jetInstance != null)
			{
				this.jetInstance.Dispose();
				this.jetInstance = null;
			}
			DiagnosticContext.TraceDword((LID)34880U, (uint)Environment.TickCount);
			this.DismountComplete();
			this.InternalDisposeLogReplay();
		}

		public override void Delete(bool deleteFiles)
		{
			this.Close();
			if (deleteFiles)
			{
				File.Delete(this.DatabaseFile);
				this.DeleteLogfiles();
			}
		}

		public override void ResetDatabaseEngine()
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!this.jetInstance.IsClosed, "Do not call ResetDatabaseEngine when the database is closed already");
			this.Close();
			this.InitializeJetInstance(JetDatabase.InitType.Normal);
			this.AttachDatabase();
		}

		public override void ExtendDatabase(IConnectionProvider connectionProvider)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!this.jetInstance.IsClosed, "Do not call ExtendDatabase when the database is closed already");
			JetConnection jetConnection = (JetConnection)connectionProvider.GetConnection();
			JET_SESID jetSession = jetConnection.JetSession;
			JET_DBID jetDatabase = jetConnection.JetDatabase;
			int num = 0;
			Api.JetGetDatabaseInfo(jetSession, jetDatabase, out num, JET_DbInfo.SpaceOwned);
			num += this.jetInstance.Parameters.DbExtensionSize;
			int num2 = 0;
			Windows8Api.JetResizeDatabase(jetSession, jetDatabase, num, out num2, ResizeDatabaseGrbit.None);
		}

		public override void CreatePhysicalSchemaObjects(IConnectionProvider connectionProvider)
		{
			JetConnection jetConnection = (JetConnection)connectionProvider.GetConnection();
			GeneratedCreateDatabase.CreateDatabase(jetConnection.JetSession, jetConnection.JetDatabase);
		}

		public override void PopulateTableMetadataFromDatabase()
		{
		}

		public override void GetDatabaseSize(IConnectionProvider connectionProvider, out uint totalPages, out uint availablePages, out uint pageSize)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			int num = 0;
			int num2 = 0;
			pageSize = (uint)this.PageSize;
			try
			{
				Api.JetGetDatabaseInfo(jetConnection.JetSession, jetConnection.JetDatabase, out num, JET_DbInfo.Filesize);
				Api.JetGetDatabaseInfo(jetConnection.JetSession, jetConnection.JetDatabase, out num2, (JET_DbInfo)1073741836);
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw this.ProcessJetError(JetDatabase.InitType.Normal, (LID)63056U, "JetDatabase.GetDatabaseSize", ex);
			}
			totalPages = (uint)num;
			availablePages = (uint)num2;
		}

		public override void ForceNewLog(IConnectionProvider connectionProvider)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			Api.JetCommitTransaction(jetConnection.JetSession, (CommitTransactionGrbit)16);
		}

		public override IEnumerable<string> GetTableNames(IConnectionProvider connectionProvider)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			return Api.GetTableNames(jetConnection.JetSession, jetConnection.JetDatabase);
		}

		public override ILogReplayStatus LogReplayStatus
		{
			get
			{
				return this.logReplayStatus;
			}
		}

		public override void StartLogReplay(Func<bool, bool> passiveDatabaseAttachDetachHandler)
		{
			this.logReplayStatus = new JetLogReplayStatus(passiveDatabaseAttachDetachHandler);
			this.logReplayThread = new Thread(delegate()
			{
				WatsonOnUnhandledException.Guard(NullExecutionDiagnostics.Instance, new TryDelegate(this, (UIntPtr)ldftn(<StartLogReplay>b__1)));
			});
			this.logReplayThread.Start();
			this.logReplayStatus.WaitLogReplayInitiatedEvent();
		}

		public override void FinishLogReplay()
		{
			if (this.logReplayThread != null)
			{
				this.PrepareToTransitionToActive();
				this.logReplayStatus.TransitionToActive();
				this.logReplayThread.Join();
				this.logReplayThread = null;
			}
			if (!this.transitionToActiveWasSuccessful)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_FailedFinishLogReplayForTransitionToActive, new object[]
				{
					base.DisplayName,
					this.passiveMountException
				});
				throw new FatalDatabaseException("FinishLogReplay", this.passiveMountException);
			}
		}

		public override bool CheckTableExists(string tableName)
		{
			bool result = false;
			try
			{
				JET_SESID sesid;
				Api.JetBeginSession(this.jetInstance, out sesid, null, null);
				try
				{
					JET_DBID dbid;
					Api.JetOpenDatabase(sesid, this.databaseFile, null, out dbid, OpenDatabaseGrbit.ReadOnly);
					try
					{
						JET_OBJECTINFO jet_OBJECTINFO;
						Api.JetGetObjectInfo(sesid, dbid, JET_objtyp.Table, tableName, out jet_OBJECTINFO);
						if (JetDatabase.checkTableExistsTestHook.Value != null)
						{
							JetDatabase.checkTableExistsTestHook.Value(tableName);
						}
						result = true;
					}
					catch (EsentObjectNotFoundException exception)
					{
						NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
					}
				}
				finally
				{
					Api.JetEndSession(sesid, EndSessionGrbit.None);
				}
			}
			catch (EsentErrorException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				throw this.ProcessJetError(JetDatabase.InitType.Normal, (LID)65356U, "JetDatabase.CheckTableExists", ex);
			}
			return result;
		}

		public override void GetSerializedDatabaseInformation(IConnectionProvider connectionProvider, out byte[] databaseInfo)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			JET_DBINFOMISC dbInfoMisc = this.GetDbInfoMisc(jetConnection);
			databaseInfo = InteropShim.SerializeDatabaseInfo(dbInfoMisc);
		}

		public override DatabaseHeaderInfo GetDatabaseHeaderInfo(IConnectionProvider connectionProvider)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			JET_DBINFOMISC dbInfoMisc = this.GetDbInfoMisc(jetConnection);
			DateTime valueOrDefault = dbInfoMisc.logtimeRepair.ToDateTime().GetValueOrDefault();
			if (JetDatabase.getDatabaseHeaderInfoTestHook.Value == null)
			{
				return new DatabaseHeaderInfo(null, valueOrDefault, dbInfoMisc.ulRepairCount, dbInfoMisc.ulRepairCountOld);
			}
			return JetDatabase.getDatabaseHeaderInfoTestHook.Value(null, valueOrDefault, dbInfoMisc.ulRepairCount, dbInfoMisc.ulRepairCountOld);
		}

		public override void GetLastBackupInformation(IConnectionProvider connectionProvider, out DateTime fullBackupTime, out DateTime incrementalBackupTime, out DateTime differentialBackupTime, out DateTime copyBackupTime, out int snapFull, out int snapIncremental, out int snapDifferential, out int snapCopy)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			try
			{
				JET_DBINFOMISC jet_DBINFOMISC;
				Api.JetGetDatabaseInfo(jetConnection.JetSession, jetConnection.JetDatabase, out jet_DBINFOMISC, JET_DbInfo.Misc);
				JetDatabase.GetBackupInfo(jet_DBINFOMISC.bkinfoFullPrev.bklogtimeMark, out fullBackupTime, out snapFull);
				JetDatabase.GetBackupInfo(jet_DBINFOMISC.bkinfoIncPrev.bklogtimeMark, out incrementalBackupTime, out snapIncremental);
				JetDatabase.GetBackupInfo(jet_DBINFOMISC.bkinfoDiffPrev.bklogtimeMark, out differentialBackupTime, out snapDifferential);
				JetDatabase.GetBackupInfo(jet_DBINFOMISC.bkinfoCopyPrev.bklogtimeMark, out copyBackupTime, out snapCopy);
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw this.ProcessJetError(JetDatabase.InitType.Normal, (LID)54864U, "JetDatabase.GetLastBackupInformation", ex);
			}
		}

		public override void SnapshotPrepare(uint flags)
		{
			if (this.snapshotState != Database.SnapshotState.Null)
			{
				throw new NonFatalDatabaseException("Snapshot session already prepared.");
			}
			if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceDebug(0L, "Prepare snapshot session.");
			}
			try
			{
				Api.JetOSSnapshotPrepare(out this.jetOsSnapId, (SnapshotPrepareGrbit)flags);
			}
			catch (EsentErrorException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceError<EsentErrorException>(0L, "Failed to prepare snapshot session. {0}", ex);
				}
				throw new NonFatalDatabaseException("Could not prepare snapshot session.", ex);
			}
			if (this.jetOsSnapId == JET_OSSNAPID.Nil)
			{
				throw new NonFatalDatabaseException("Could not get a snapshot session.");
			}
			this.snapshotState = Database.SnapshotState.Prepared;
			if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceDebug(0L, "Add snapshot instance to the snapshot session.");
			}
			try
			{
				VistaApi.JetOSSnapshotPrepareInstance(this.jetOsSnapId, this.jetInstance, SnapshotPrepareInstanceGrbit.None);
			}
			catch (EsentErrorException ex2)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex2);
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceError<EsentErrorException>(0L, "Failed to add snapshot instance. {0}", ex2);
				}
				throw new NonFatalDatabaseException("Could not add snapshot instance to the snapshot session.", ex2);
			}
		}

		public override void SnapshotFreeze(uint flags)
		{
			if (this.snapshotState == Database.SnapshotState.Prepared)
			{
				if (this.jetOsSnapId == JET_OSSNAPID.Nil)
				{
					throw new NonFatalDatabaseException("There is no snapshot session to freeze.");
				}
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceDebug(0L, "Freeze the snapshot.");
				}
				try
				{
					JET_INSTANCE_INFO[] array = null;
					int num;
					Api.JetOSSnapshotFreeze(this.jetOsSnapId, out num, out array, (SnapshotFreezeGrbit)flags);
					this.snapshotState = Database.SnapshotState.Frozen;
					return;
				}
				catch (EsentErrorException ex)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceError<EsentErrorException>(0L, "Failed to freeze the snapshot. {0}", ex);
					}
					throw new NonFatalDatabaseException("Could not freeze the snapshot.", ex);
				}
			}
			if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceDebug(0L, "Snapshot alreay frozen. Ignore freeze operation.");
			}
		}

		public override void SnapshotThaw(uint flags)
		{
			if (this.snapshotState == Database.SnapshotState.Frozen)
			{
				if (this.jetOsSnapId == JET_OSSNAPID.Nil)
				{
					throw new NonFatalDatabaseException("There is no snapshot session to thaw.");
				}
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceDebug(0L, "Thaw the snapshot.");
				}
				try
				{
					Api.JetOSSnapshotThaw(this.jetOsSnapId, (SnapshotThawGrbit)flags);
					this.snapshotState = Database.SnapshotState.Thawed;
					return;
				}
				catch (EsentErrorException ex)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceError<EsentErrorException>(0L, "Failed to thaw the snapshot. {0}", ex);
					}
					throw new NonFatalDatabaseException("Could not thaw the snapshot.", ex);
				}
			}
			if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceDebug(0L, "Snapshot not frozen. Ignore freeze operation.");
			}
		}

		public override void SnapshotTruncateLogInstance(uint flags)
		{
			if (this.snapshotState == Database.SnapshotState.Thawed)
			{
				if (this.jetOsSnapId == JET_OSSNAPID.Nil)
				{
					throw new NonFatalDatabaseException("There is no snapshot session to truncate logs.");
				}
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceDebug(0L, "Truncate logs for snapshot session.");
				}
				try
				{
					VistaApi.JetOSSnapshotTruncateLogInstance(this.jetOsSnapId, this.jetInstance, (SnapshotTruncateLogGrbit)flags);
					return;
				}
				catch (EsentErrorException ex)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceError<EsentErrorException>(0L, "Failed to truncate logs for snapshot session. {0}", ex);
					}
					throw new NonFatalDatabaseException("Could not truncate logs for snapshot session.", ex);
				}
			}
			if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceDebug(0L, "Snapshot not thawed. Ignore truncate logs operation.");
			}
		}

		public override void SnapshotStop(uint flags)
		{
			if (this.jetOsSnapId != JET_OSSNAPID.Nil)
			{
				if (this.snapshotState == Database.SnapshotState.Frozen)
				{
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceDebug(0L, "Thaw the frozen snapshot.");
					}
					try
					{
						Api.JetOSSnapshotThaw(this.jetOsSnapId, SnapshotThawGrbit.None);
						this.snapshotState = Database.SnapshotState.Thawed;
					}
					catch (EsentErrorException ex)
					{
						NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
						if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.ErrorTrace))
						{
							Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceError<EsentErrorException>(0L, "Thaw operation failed. {0}", ex);
						}
					}
				}
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceDebug(0L, "End the snapshot.");
				}
				try
				{
					VistaApi.JetOSSnapshotEnd(this.jetOsSnapId, (SnapshotEndGrbit)flags);
					this.snapshotState = Database.SnapshotState.Null;
					this.jetOsSnapId = JET_OSSNAPID.Nil;
					return;
				}
				catch (EsentErrorException ex2)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(ex2);
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceError<EsentErrorException>(0L, "Snapshot end operation failed. {0}", ex2);
					}
					return;
				}
			}
			if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.SnapshotOperationTracer.TraceDebug(0L, "No snapshot session. Ignore stop snapshot operation.");
			}
		}

		public override void CancelLogReplay()
		{
			if (this.logReplayThread != null)
			{
				this.logReplayStatus.Cancel();
				this.logReplayThread.Join();
				this.logReplayThread = null;
			}
		}

		public override bool IsDatabaseEngineBusy(out string highResourceUsageType, out long currentResourceUsage, out long maxResourceUsage)
		{
			long num = 0L;
			long num2 = 0L;
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!this.jetInstance.IsClosed, "It should not be possible to call IsDatabaseEngineBusy when the database is closed already.");
			UnpublishedApi.JetGetResourceParam(this.jetInstance, JET_resoper.CurrentUse, JET_resid.VERBUCKET, out num);
			UnpublishedApi.JetGetResourceParam(this.jetInstance, JET_resoper.MaxUse, JET_resid.VERBUCKET, out num2);
			if (JetDatabase.getCurrentResourceUsageTestHook.Value != null)
			{
				num = JetDatabase.getCurrentResourceUsageTestHook.Value(JET_resid.VERBUCKET, num, num2);
			}
			bool flag = num > num2 / 2L;
			if (flag)
			{
				highResourceUsageType = JET_resid.VERBUCKET.ToString();
				currentResourceUsage = num;
				maxResourceUsage = num2;
			}
			else
			{
				highResourceUsageType = null;
				currentResourceUsage = 0L;
				maxResourceUsage = 0L;
			}
			return flag;
		}

		public override void VersionStoreCleanup(IConnectionProvider connectionProvider)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			Api.JetIdle(jetConnection.JetSession, IdleGrbit.Compact);
		}

		public override void StartBackgroundChecksumming(IConnectionProvider connectionProvider)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!this.jetInstance.IsClosed, "Do not call StartBackgroundChecksumming when the database is closed already");
			JetConnection jetConnection = (JetConnection)connectionProvider.GetConnection();
			if (base.DatabaseOptions.BackgroundDatabaseMaintenance && (base.Flags & DatabaseFlags.BackgroundMaintenanceEnabled) != DatabaseFlags.None)
			{
				JET_SESID jetSession = jetConnection.JetSession;
				JET_DBID jetDatabase = jetConnection.JetDatabase;
				int num = 0;
				UnpublishedApi.JetDatabaseScan(jetSession, jetDatabase, ref num, 0, null, DatabaseScanGrbit.BatchStartContinuous);
				FaultInjection.InjectFault(JetDatabase.backgroundMaintenanceTestHook);
			}
		}

		internal static IDisposable SetBackgroundMaintenanceTestHook(Action action)
		{
			return JetDatabase.backgroundMaintenanceTestHook.SetTestHook(action);
		}

		internal static IDisposable SetGetCurrentResourceUsageTestHook(Func<JET_resid, long, long, long> testDelegate)
		{
			return JetDatabase.getCurrentResourceUsageTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetCheckTableExistsTestHook(Action<string> testDelegate)
		{
			return JetDatabase.checkTableExistsTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetGetDatabaseHeaderInfoTestHook(Func<byte[], DateTime, int, int, DatabaseHeaderInfo> testDelegate)
		{
			return JetDatabase.getDatabaseHeaderInfoTestHook.SetTestHook(testDelegate);
		}

		private static void SetJetTraceTags()
		{
			bool flag = false;
			foreach (KeyValuePair<JET_tracetag, Microsoft.Exchange.Diagnostics.Trace> keyValuePair in JetDatabase.jetTracers)
			{
				bool flag2 = keyValuePair.Value.IsTraceEnabled(TraceType.DebugTrace);
				UnpublishedApi.JetTracing(JET_traceop.SetTag, keyValuePair.Key, flag2);
				flag = (flag || flag2);
			}
			UnpublishedApi.JetTracing(JET_traceop.SetGlobal, JET_tracetag.Null, flag);
		}

		private static void EmitJetTrace(JET_tracetag tag, string prefix, string trace, IntPtr context)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(JetDatabase.jetTracers.ContainsKey(tag), "No tracer for JET_tracetag");
			JetDatabase.jetTracers[tag].TraceDebug<string>(0L, "{0}", prefix + trace);
		}

		private static void GetBackupInfo(JET_BKLOGTIME bklogtimeMark, out DateTime lastBackupTime, out int snapshot)
		{
			DateTime? dateTime = bklogtimeMark.ToDateTime();
			if (dateTime == null)
			{
				lastBackupTime = DateTime.MinValue;
				snapshot = -1;
				return;
			}
			lastBackupTime = dateTime.Value;
			snapshot = (bklogtimeMark.fOSSnapshot ? 1 : 0);
		}

		protected virtual void DismountBegins()
		{
		}

		protected virtual void DismountComplete()
		{
		}

		protected virtual void PrepareToTransitionToActive()
		{
			this.SetCacheParameters(JetDatabase.InitType.Normal);
		}

		protected virtual void PrepareToMountAsActive()
		{
		}

		protected virtual void PrepareToMountAsPassive()
		{
		}

		protected virtual void JetInitComplete()
		{
		}

		private static void SetTableClassNames()
		{
			foreach (JetTableClassInfo jetTableClassInfo in JetTableClassInfo.Classes.Values)
			{
				Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, jetTableClassInfo.JetParam, 0, jetTableClassInfo.Name);
			}
		}

		private static void SetEseCachePageReplacementParameters()
		{
			Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, JET_param.LrukCorrInterval, (int)ConfigurationSchema.EseLrukCorrInterval.Value.TotalMilliseconds * 1000, null);
			Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, JET_param.LrukTimeout, (int)ConfigurationSchema.EseLrukTimeout.Value.TotalSeconds, null);
		}

		private void DeleteLogfiles()
		{
			foreach (string searchPattern in JetDatabase.LogFilePatterns)
			{
				foreach (string pathName in Directory.GetFiles(base.LogPath, searchPattern))
				{
					base.DeleteFileOrDirectory(pathName);
				}
			}
		}

		private void AttachDatabase()
		{
			using (Session session = new Session(this.jetInstance))
			{
				bool flag = true;
				try
				{
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.TraceDebug(0L, "AttachDatabase: calling JetCreateDatabase.");
					}
					DiagnosticContext.TraceDword((LID)61824U, (uint)Environment.TickCount);
					Api.JetAttachDatabase(session, this.DatabaseFile, AttachDatabaseGrbit.None);
					DiagnosticContext.TraceDword((LID)45440U, (uint)Environment.TickCount);
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.TraceDebug(0L, "AttachDatabase: calling JetOpenDatabase.");
					}
					JET_DBID jet_DBID;
					Api.JetOpenDatabase(session, this.DatabaseFile, null, out jet_DBID, OpenDatabaseGrbit.None);
					flag = false;
				}
				finally
				{
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.TraceDebug<string>(0L, "Attach the database {0}.", flag ? "thrown exception" : "returned succesfully");
					}
				}
			}
		}

		private Exception ProcessJetError(JetDatabase.InitType initType, LID lid, string operation, EsentErrorException e)
		{
			bool flag = initType == JetDatabase.InitType.ForLogReplay;
			Exception result = new FatalDatabaseException(operation, e);
			bool flag2 = false;
			DiagnosticContext.TraceStoreError(lid, (uint)e.Error);
			string text = new StackTrace(1, false).ToString();
			if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInteractionSummaryTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				string message = string.Format("{0}:Exception. Message:[{1}] Stack:[{2}]", operation, e.Message, text);
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInteractionSummaryTracer.TraceError(0L, message);
			}
			Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_JetExceptionDetected, new object[]
			{
				e.Message,
				text,
				DiagnosticsNativeMethods.GetCurrentProcessId().ToString(),
				base.DisplayName,
				e.ToString()
			});
			JET_err error = e.Error;
			if (error <= JET_err.OutOfDatabaseSpace)
			{
				if (error <= JET_err.DiskFull)
				{
					if (error <= JET_err.LogCorrupted)
					{
						if (error == JET_err.RollbackError)
						{
							goto IL_1F7;
						}
						if (error != JET_err.LogCorrupted)
						{
							goto IL_22D;
						}
						goto IL_1FE;
					}
					else if (error != JET_err.FileNotFound && error != JET_err.DiskFull)
					{
						goto IL_22D;
					}
				}
				else if (error <= JET_err.FileAccessDenied)
				{
					switch (error)
					{
					case JET_err.InstanceUnavailableDueToFatalLogDiskFull:
						break;
					case JET_err.DatabaseUnavailable:
						goto IL_22D;
					case JET_err.InstanceUnavailable:
						goto IL_1F7;
					default:
						if (error != JET_err.FileAccessDenied)
						{
							goto IL_22D;
						}
						break;
					}
				}
				else
				{
					switch (error)
					{
					case JET_err.DiskIO:
						break;
					case JET_err.DiskReadVerificationFailure:
						goto IL_1FE;
					case JET_err.OutOfFileHandles:
					case JET_err.PageNotInitialized:
						goto IL_22D;
					case JET_err.ReadVerifyFailure:
						goto IL_2A2;
					default:
						if (error != JET_err.OutOfDatabaseSpace)
						{
							goto IL_22D;
						}
						break;
					}
				}
			}
			else
			{
				if (error <= JET_err.RequiredLogFilesMissing)
				{
					if (error <= JET_err.CommittedLogFileCorrupt)
					{
						switch (error)
						{
						case JET_err.RestoreOfNonBackupDatabase:
							Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "RestoreOfNonBackupDatabase: Contact ESEDev");
							goto IL_2A2;
						case JET_err.CheckpointDepthTooDeep:
							goto IL_1F7;
						default:
							if (error != JET_err.CommittedLogFileCorrupt)
							{
								goto IL_22D;
							}
							break;
						}
					}
					else if (error != JET_err.CommittedLogFilesMissing && error != JET_err.RequiredLogFilesMissing)
					{
						goto IL_22D;
					}
					result = new NonFatalDatabaseException((ErrorCodeValue)e.Error, e.Message, e);
					goto IL_2A2;
				}
				if (error <= JET_err.LogSequenceEnd)
				{
					if (error != JET_err.LogDiskFull)
					{
						if (error != JET_err.LogSequenceEnd)
						{
							goto IL_22D;
						}
						flag2 = true;
						goto IL_2A2;
					}
				}
				else
				{
					if (error == JET_err.LogWriteFail)
					{
						flag2 = true;
						goto IL_2A2;
					}
					if (error != JET_err.LogFileCorrupt)
					{
						goto IL_22D;
					}
					goto IL_1FE;
				}
			}
			flag2 = true;
			goto IL_2A2;
			IL_1F7:
			flag2 = true;
			goto IL_2A2;
			IL_1FE:
			flag2 = true;
			goto IL_2A2;
			IL_22D:
			if (e is EsentMemoryException)
			{
				result = new OutOfMemoryException(e.Message, e);
				flag2 = true;
			}
			else if (e is EsentFragmentationException)
			{
				flag2 = true;
			}
			else if (e is EsentCorruptionException)
			{
				flag2 = true;
			}
			else if (e is EsentDataException)
			{
				if (flag)
				{
					flag2 = true;
				}
			}
			else if (ConfigurationSchema.RetailAssertOnUnexpectedJetErrors.Value)
			{
				this.PublishHaFailure(FailureTag.Unrecoverable);
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, string.Format("Unexpected JetError {0}: contact HA/StoreDev", e.Error));
			}
			IL_2A2:
			if (flag2)
			{
				this.PublishHaFailure(FailureTag.UnexpectedDismount);
			}
			return result;
		}

		private void SetDatabaseOptionsForActiveCopy()
		{
			this.jetInstance.Parameters.CheckpointDepthMax = JetDatabase.CheckpointDepthOnActive;
			this.jetInstance.Parameters.DbScanThrottle = (int)DefaultSettings.Get.DbScanThrottleOnActive.TotalMilliseconds;
			if (base.DatabaseOptions != null)
			{
				if (base.DatabaseOptions.LogCheckpointDepth != null)
				{
					this.jetInstance.Parameters.CheckpointDepthMax = base.DatabaseOptions.LogCheckpointDepth.Value;
				}
				if (base.DatabaseOptions.CachePriority != null)
				{
					this.jetInstance.Parameters.CachePriority = base.DatabaseOptions.CachePriority.Value;
				}
				if (base.DatabaseOptions.BackgroundDatabaseMaintenanceDelay != null)
				{
					this.jetInstance.Parameters.DbScanThrottle = base.DatabaseOptions.BackgroundDatabaseMaintenanceDelay.Value;
				}
				if (base.DatabaseOptions.MaximumPreReadPages != null)
				{
					this.jetInstance.Parameters.PrereadIOMax = base.DatabaseOptions.MaximumPreReadPages.Value;
				}
			}
		}

		private void SetDatabaseOptionsForPassiveCopy()
		{
			this.jetInstance.Parameters.PrereadIOMax = 1;
			this.jetInstance.Parameters.CheckpointDepthMax = JetDatabase.CheckpointDepthOnPassive;
			this.jetInstance.Parameters.DbScanThrottle = (int)DefaultSettings.Get.DbScanThrottleOnPassive.TotalMilliseconds;
			if (base.DatabaseOptions != null)
			{
				if (base.DatabaseOptions.ReplayCheckpointDepth != null)
				{
					this.jetInstance.Parameters.CheckpointDepthMax = base.DatabaseOptions.ReplayCheckpointDepth.Value;
				}
				if (base.DatabaseOptions.ReplayCachePriority != null)
				{
					this.jetInstance.Parameters.CachePriority = base.DatabaseOptions.ReplayCachePriority.Value;
				}
				if (base.DatabaseOptions.ReplayBackgroundDatabaseMaintenanceDelay != null)
				{
					this.jetInstance.Parameters.DbScanThrottle = base.DatabaseOptions.ReplayBackgroundDatabaseMaintenanceDelay.Value;
				}
				if (base.DatabaseOptions.ReplayBackgroundDatabaseMaintenance != null)
				{
					if (ConfigurationSchema.EnableDbDivergenceDetection.Value)
					{
						this.jetInstance.Parameters.EnableDbScanInRecovery = (base.DatabaseOptions.ReplayBackgroundDatabaseMaintenance.Value ? 1 : 0);
					}
					else
					{
						this.jetInstance.Parameters.EnableDbScanInRecovery = (base.DatabaseOptions.ReplayBackgroundDatabaseMaintenance.Value ? 2 : 0);
					}
				}
				if (base.DatabaseOptions.MaximumReplayPreReadPages != null)
				{
					this.jetInstance.Parameters.PrereadIOMax = base.DatabaseOptions.MaximumReplayPreReadPages.Value;
				}
			}
		}

		private void SetCacheParameters(JetDatabase.InitType initType)
		{
			this.computedJetCacheSizeMin = this.AdjustCacheForMountType(initType, base.GetMinCachePages());
			this.computedJetCacheSizeMax = this.AdjustCacheForMountType(initType, base.GetMaxCachePages());
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.computedJetCacheSizeMax >= this.computedJetCacheSizeMin, string.Format("The max cache size {0} must at least equal to the min cache size {1}.", this.computedJetCacheSizeMax, this.computedJetCacheSizeMin));
			SystemParameters.CacheSizeMax = this.computedJetCacheSizeMax;
			SystemParameters.CacheSizeMin = this.computedJetCacheSizeMin;
			if (this.computedJetCacheSizeMax > 0)
			{
				SystemParameters.StartFlushThreshold = JetDatabase.StartFlushPct * this.computedJetCacheSizeMax / 100;
				SystemParameters.StopFlushThreshold = JetDatabase.StopFlushPct * this.computedJetCacheSizeMax / 100;
			}
		}

		private void InitializeJetInstance(JetDatabase.InitType initType)
		{
			bool flag = true;
			try
			{
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.TraceDebug(0L, "Initialize the jet instance.");
				}
				this.SetCacheParameters(initType);
				this.jetInstance = new Instance(this.DbGuid.ToString(), base.DisplayName);
				DatabaseOptions databaseOptions = base.DatabaseOptions ?? new DatabaseOptions();
				this.jetInstance.Parameters.MaxSessions = databaseOptions.MaximumSessions.GetValueOrDefault(JetDatabase.MaxSessions);
				this.jetInstance.Parameters.MaxOpenTables = databaseOptions.MaximumOpenTables.GetValueOrDefault(JetDatabase.MaxOpenTables);
				this.jetInstance.Parameters.MaxCursors = databaseOptions.MaximumCursors.GetValueOrDefault(JetDatabase.MaxCursors);
				this.jetInstance.Parameters.MaxTemporaryTables = databaseOptions.MaximumTemporaryTables.GetValueOrDefault(JetDatabase.MaxTemporaryTables);
				this.jetInstance.Parameters.MaxVerPages = databaseOptions.MaximumVersionStorePages.GetValueOrDefault(JetDatabase.MaxVersionBuckets);
				this.jetInstance.Parameters.PreferredVerPages = databaseOptions.PreferredVersionStorePages.GetValueOrDefault(JetDatabase.PreferredVersionBuckets);
				this.jetInstance.Parameters.LogBuffers = databaseOptions.LogBuffers.GetValueOrDefault(JetDatabase.LogBuffers);
				this.jetInstance.Parameters.DbExtensionSize = databaseOptions.DatabaseExtensionSize.GetValueOrDefault(JetDatabase.DatabaseExtensionSize);
				this.jetInstance.Parameters.EnableOnlineDefrag = databaseOptions.EnableOnlineDefragmentation.GetValueOrDefault(true);
				this.InternalInitializeJetParameters();
				this.jetInstance.Parameters.CircularLog = ((base.Flags & DatabaseFlags.CircularLoggingEnabled) != DatabaseFlags.None);
				this.jetInstance.Parameters.LogFileDirectory = base.LogPath;
				this.jetInstance.Parameters.SystemDirectory = base.LogPath;
				this.jetInstance.Parameters.TempDirectory = base.LogPath;
				this.jetInstance.Parameters.VersionStoreTaskQueueMax = JetDatabase.MaxBackgroundCleanupTasks;
				this.jetInstance.Parameters.LogFileSize = JetDatabase.LogFileSize;
				this.jetInstance.Parameters.CreatePathIfNotExist = true;
				this.jetInstance.Parameters.OneDatabasePerSession = true;
				this.jetInstance.Parameters.CleanupMismatchedLogFiles = false;
				this.jetInstance.Parameters.MaxTransactionSize = 80;
				this.jetInstance.Parameters.CachedClosedTables = JetDatabase.CachedClosedTablesValue;
				this.jetInstance.Parameters.WaypointLatency = 1;
				if (base.DatabaseOptions != null)
				{
					if (base.DatabaseOptions.BackgroundDatabaseMaintenanceSerialization != null)
					{
						this.jetInstance.Parameters.EnableDBScanSerialization = base.DatabaseOptions.BackgroundDatabaseMaintenanceSerialization.Value;
					}
					if (base.DatabaseOptions.MimimumBackgroundDatabaseMaintenanceInterval != null)
					{
						this.jetInstance.Parameters.DbScanIntervalMinSec = base.DatabaseOptions.MimimumBackgroundDatabaseMaintenanceInterval.Value;
					}
					if (base.DatabaseOptions.MaximumBackgroundDatabaseMaintenanceInterval != null)
					{
						this.jetInstance.Parameters.DbScanIntervalMaxSec = base.DatabaseOptions.MaximumBackgroundDatabaseMaintenanceInterval.Value;
					}
					else
					{
						this.jetInstance.Parameters.DbScanIntervalMaxSec = 1209600;
					}
					if (base.DatabaseOptions.CachedClosedTables != null)
					{
						this.jetInstance.Parameters.CachedClosedTables = base.DatabaseOptions.CachedClosedTables.Value;
					}
					if (base.DatabaseOptions.LogFilePrefix != null)
					{
						this.jetInstance.Parameters.BaseName = base.DatabaseOptions.LogFilePrefix;
					}
					if (JetDatabase.InitType.ForLogReplay == initType)
					{
						this.SetDatabaseOptionsForPassiveCopy();
						this.InternalSetReplayCallbackInstance();
					}
					else
					{
						this.SetDatabaseOptionsForActiveCopy();
					}
				}
				this.jetInstance.Parameters.Recovery = RegistryReader.Instance.GetValue<bool>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "Recovery", true);
				JET_RSTMAP[] array = new JET_RSTMAP[]
				{
					new JET_RSTMAP
					{
						szNewDatabaseName = this.DatabaseFile
					}
				};
				JET_RSTINFO jet_RSTINFO = new JET_RSTINFO
				{
					rgrstmap = array,
					crstmap = array.Length
				};
				InitGrbit initGrbit = (InitGrbit)4096;
				if (JetDatabase.InitType.ForLogReplay == initType)
				{
					jet_RSTINFO.pfnStatus = this.InternalGetReplayStatusCallback();
					this.PrepareToMountAsPassive();
					initGrbit |= (InitGrbit)2112;
				}
				else
				{
					jet_RSTINFO.pfnStatus = new JET_PFNSTATUS(this.ActiveMountStatusCallback);
					this.PrepareToMountAsActive();
				}
				if (this.lossyMount)
				{
					initGrbit |= (InitGrbit)128;
				}
				this.jetInstance.Init(jet_RSTINFO, initGrbit);
				this.JetInitComplete();
				flag = false;
			}
			finally
			{
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInitializationTracer.TraceDebug<string>(0L, "jet instance initialization {0}.", flag ? "thrown exception" : "returned succesfully");
				}
			}
		}

		private int AdjustCacheForMountType(JetDatabase.InitType initType, int cachePages)
		{
			if (Microsoft.Exchange.Server.Storage.Common.Globals.IsMultiProcess && base.DatabaseOptions != null)
			{
				int num = Math.Max(base.DatabaseOptions.TotalDatabasesOnServer.GetValueOrDefault(1), 4);
				int num2 = Math.Min(base.DatabaseOptions.MaxActiveDatabases.GetValueOrDefault(num), num);
				if (initType == JetDatabase.InitType.Normal)
				{
					int num3 = (int)((double)num2 * 0.8 + (double)num * 0.2);
					if (num3 > 0 && cachePages >= num3)
					{
						return cachePages / num3;
					}
				}
				else
				{
					int num4 = (int)(((double)num2 * 0.8 + (double)num * 0.2) * 5.0);
					if (num4 > 0 && cachePages >= num4)
					{
						return cachePages / num4;
					}
				}
			}
			return cachePages;
		}

		private JET_DBINFOMISC GetDbInfoMisc(JetConnection jetConnection)
		{
			JET_DBINFOMISC result;
			try
			{
				Api.JetGetDatabaseInfo(jetConnection.JetSession, jetConnection.JetDatabase, out result, JET_DbInfo.Misc);
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw this.ProcessJetError(JetDatabase.InitType.Normal, (LID)38480U, "JetDatabase.GetDbInfoMisc", ex);
			}
			return result;
		}

		private void InternalSetReplayCallbackInstance()
		{
			this.logReplayStatus.InitializeWithInstance(this.jetInstance);
		}

		internal JET_OSSNAPID JetOsSnapId
		{
			get
			{
				return this.jetOsSnapId;
			}
		}

		private void ReplayThreadProc()
		{
			if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.TraceDebug(0L, "Replay thread started");
			}
			try
			{
				this.InitializeJetInstance(JetDatabase.InitType.ForLogReplay);
				this.SetCacheParameters(JetDatabase.InitType.Normal);
				this.SetDatabaseOptionsForActiveCopy();
				this.AttachDatabase();
				this.transitionToActiveWasSuccessful = true;
			}
			catch (EsentRecoveredWithoutUndoDatabasesConsistentException)
			{
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.TraceDebug(0L, "Replay caught JET_err.RecoveredWithoutUndoDatabasesConsistent");
				}
			}
			catch (EsentRecoveredWithoutUndoException)
			{
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.TraceDebug(0L, "Replay caught JET_err.RecoveredWithoutUndo");
				}
			}
			catch (EsentErrorException ex)
			{
				this.HandleExpectedExceptionTypesDuringReplayThreadProc(ex);
			}
			catch (StoreException ex2)
			{
				this.HandleExpectedExceptionTypesDuringReplayThreadProc(ex2);
			}
			catch (FatalDatabaseException ex3)
			{
				this.HandleExpectedExceptionTypesDuringReplayThreadProc(ex3);
			}
			catch (NonFatalDatabaseException ex4)
			{
				this.HandleExpectedExceptionTypesDuringReplayThreadProc(ex4);
			}
			finally
			{
				this.logReplayStatus.SetLogReplayInitiatedEvent();
			}
		}

		private void HandleExpectedExceptionTypesDuringReplayThreadProc(Exception ex)
		{
			this.passiveMountException = ex;
			Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.DbInteractionSummaryTracer.TraceError<Exception>(0L, "ReplayThreadProc got exception {0}", ex);
			Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.TraceError<Exception>(0L, "Replay caught exception {0}", ex);
			Exception e = ex;
			if (ex is EsentErrorException)
			{
				this.ProcessJetError(JetDatabase.InitType.ForLogReplay, (LID)48072U, "JetDatabase.ReplayThreadProc", (EsentErrorException)ex);
			}
			else if (ex.InnerException != null)
			{
				e = ex.InnerException;
			}
			this.logReplayStatus.RecordPassiveReplayFailure(e);
		}

		private JET_err ActiveMountStatusCallback(JET_SESID sesid, JET_SNP snp, JET_SNT snt, object data)
		{
			if (snp != JET_SNP.Restore)
			{
				switch (snp)
				{
				case (JET_SNP)18:
				{
					JET_RECOVERYCONTROL jet_RECOVERYCONTROL = (JET_RECOVERYCONTROL)data;
					return this.RecoveryControlForActiveDatabase(jet_RECOVERYCONTROL, jet_RECOVERYCONTROL.errDefault);
				}
				case (JET_SNP)19:
					break;
				default:
					return JET_err.Success;
				}
			}
			else if (snt == JET_SNT.Progress)
			{
				JET_SNPROG jet_SNPROG = (JET_SNPROG)data;
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.TraceDebug<int, int>(0L, "ActiveMount Progress callback {0}/{1}", jet_SNPROG.cunitDone, jet_SNPROG.cunitTotal);
			}
			return JET_err.Success;
		}

		private JET_err RecoveryControlForActiveDatabase(JET_RECOVERYCONTROL status, JET_err errDefault)
		{
			if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.TraceDebug<JET_RECOVERYCONTROL>(0L, "RecoveryControl callback {0}", status);
			}
			JET_err jet_err = errDefault;
			switch (status.sntUnion)
			{
			case (JET_SNT)1004:
				jet_err = this.HandleMissingLog((JET_SNMISSINGLOG)status, errDefault);
				break;
			}
			if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.TraceDebug<JET_err>(0L, "RecoveryControl callback returns {0}", jet_err);
			}
			return jet_err;
		}

		private JET_err HandleMissingLog(JET_SNMISSINGLOG missingLogStatus, JET_err errDefault)
		{
			Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA.ExTraceGlobals.LogReplayStatusTracer.TraceDebug(0L, "Missing log {0} (generation {1}), IsCurrentLog={2}, NextAction={3}, ErrDefault={4}", new object[]
			{
				missingLogStatus.wszLogFile,
				missingLogStatus.lGenMissing,
				missingLogStatus.fCurrentLog,
				missingLogStatus.eNextAction,
				missingLogStatus.errDefault
			});
			if (missingLogStatus.fCurrentLog && this.lossyMount)
			{
				return JET_err.Success;
			}
			return errDefault;
		}

		private void InternalInitializeJetDatabase()
		{
			this.jetOsSnapId = JET_OSSNAPID.Nil;
			this.snapshotState = Database.SnapshotState.Null;
		}

		private void InternalConfigureHungIO()
		{
			SystemParameters.HungIOThreshold = 20000;
			SystemParameters.HungIOActions = 17;
			SystemParameters.ExceptionAction = JET_ExceptionAction.None;
		}

		private void InternalConfigureEseStaging()
		{
			string str = string.Format("SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\{1}-{2}", "Ese", "Mdb", this.DbGuid.ToString());
			string paramString = "reg:HKEY_LOCAL_MACHINE\\" + str;
			try
			{
				Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, (JET_param)189, 0, paramString);
			}
			catch (EsentFileNotFoundException exception)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
			}
			catch (EsentErrorException exception2)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(exception2);
			}
			if (ConfigurationSchema.EseStageFlighting.Value != 0)
			{
				Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, (JET_param)190, ConfigurationSchema.EseStageFlighting.Value, null);
			}
		}

		private void InternalDisposeLogReplay()
		{
			if (this.logReplayStatus != null)
			{
				this.logReplayStatus.Dispose();
				this.logReplayStatus = null;
			}
		}

		private void InternalInitializeJetParameters()
		{
			this.jetInstance.Parameters.EnableIndexCheckingEx = JET_INDEXCHECKING.DeferToOpenTable;
			this.jetInstance.Parameters.CheckpointTooDeep = 10485760 / JetDatabase.LogFileSize;
			this.jetInstance.Parameters.EnableExternalAutoHealing = true;
			this.jetInstance.Parameters.AggressiveLogRollover = true;
			this.jetInstance.Parameters.EnableHaPublish = true;
			this.jetInstance.Parameters.EnableShrinkDatabase = ShrinkDatabaseGrbit.Off;
			this.jetInstance.Parameters.ZeroDatabaseUnusedSpace = ConfigurationSchema.EnableDatabaseUnusedSpaceScrubbing.Value;
		}

		private JET_PFNSTATUS InternalGetReplayStatusCallback()
		{
			return new JET_PFNSTATUS(this.logReplayStatus.InitStatusCallback);
		}

		private void InternalSetEmitCallback()
		{
			UnpublishedApi.JetTracing(JET_traceop.SetEmitCallback, JET_tracetag.Null, new JET_PFNTRACEEMIT(JetDatabase.EmitJetTrace));
		}

		private const InitGrbit LogReplayGrbit = (InitGrbit)2112;

		private const JET_DbInfo UseCachedResult = (JET_DbInfo)1073741824;

		private static readonly int CheckpointDepthOnPassive = DefaultSettings.Get.CheckpointDepthOnPassive;

		private static readonly int CheckpointDepthOnActive = DefaultSettings.Get.CheckpointDepthOnActive;

		private static readonly int CachedClosedTablesValue = DefaultSettings.Get.CachedClosedTables;

		private static readonly string[] LogFilePatterns = new string[]
		{
			"*.chk",
			"*.jrs",
			"*.log"
		};

		private static int maxSessions = 1000;

		private static int maxOpenTables = 100000;

		private static int maxCursors = 100000;

		private static int maxTemporaryTables = 5000;

		private static int maxVersionBuckets = 16384;

		private static int preferredVersionBuckets = JetDatabase.maxVersionBuckets * 8 / 10;

		private static int maxBackgroundCleanupTasks = 4;

		private static int databaseExtensionSize = 4096;

		private static readonly Dictionary<JET_tracetag, Microsoft.Exchange.Diagnostics.Trace> jetTracers = new Dictionary<JET_tracetag, Microsoft.Exchange.Diagnostics.Trace>
		{
			{
				JET_tracetag.Information,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetInformationTracer
			},
			{
				JET_tracetag.Errors,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetErrorsTracer
			},
			{
				JET_tracetag.Asserts,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetAssertsTracer
			},
			{
				JET_tracetag.API,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetAPITracer
			},
			{
				JET_tracetag.InitTerm,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetInitTermTracer
			},
			{
				JET_tracetag.BufferManager,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetBufferManagerTracer
			},
			{
				JET_tracetag.BufferManagerBufferCacheState,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetBufMgrCacheStateTracer
			},
			{
				JET_tracetag.BufferManagerBufferDirtyState,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetBufMgrDirtyStateTracer
			},
			{
				JET_tracetag.BufferManagerHashedLatches,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetBufferManagerHashedLatchesTracer
			},
			{
				JET_tracetag.IO,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetIOTracer
			},
			{
				JET_tracetag.Memory,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetMemoryTracer
			},
			{
				JET_tracetag.VersionStore,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetVersionStoreTracer
			},
			{
				JET_tracetag.VersionStoreOOM,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetVersionStoreOOMTracer
			},
			{
				JET_tracetag.VersionCleanup,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetVersionCleanupTracer
			},
			{
				JET_tracetag.Catalog,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetCatalogTracer
			},
			{
				JET_tracetag.DDLRead,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetDDLReadTracer
			},
			{
				JET_tracetag.DDLWrite,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetDDLWriteTracer
			},
			{
				JET_tracetag.DMLRead,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetDMLReadTracer
			},
			{
				JET_tracetag.DMLWrite,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetDMLWriteTracer
			},
			{
				JET_tracetag.DMLConflicts,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetDMLConflictsTracer
			},
			{
				JET_tracetag.Instances,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetInstancesTracer
			},
			{
				JET_tracetag.Databases,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetDatabasesTracer
			},
			{
				JET_tracetag.Sessions,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetSessionsTracer
			},
			{
				JET_tracetag.Cursors,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetCursorsTracer
			},
			{
				JET_tracetag.CursorNavigation,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetCursorNavigationTracer
			},
			{
				JET_tracetag.CursorPageRefs,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetCursorPageRefsTracer
			},
			{
				JET_tracetag.Btree,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetBtreeTracer
			},
			{
				JET_tracetag.Space,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetSpaceTracer
			},
			{
				JET_tracetag.FCBs,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetFCBsTracer
			},
			{
				JET_tracetag.Transactions,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetTransactionsTracer
			},
			{
				JET_tracetag.Logging,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetLoggingTracer
			},
			{
				JET_tracetag.Recovery,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetRecoveryTracer
			},
			{
				JET_tracetag.Backup,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetBackupTracer
			},
			{
				JET_tracetag.Restore,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetRestoreTracer
			},
			{
				JET_tracetag.OLD,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetOLDTracer
			},
			{
				JET_tracetag.Eventlog,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetEventlogTracer
			},
			{
				JET_tracetag.BufferManagerMaintTasks,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetBufferManagerMaintTasksTracer
			},
			{
				JET_tracetag.SpaceManagement,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetSpaceManagementTracer
			},
			{
				JET_tracetag.SpaceInternal,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetSpaceInternalTracer
			},
			{
				JET_tracetag.IOQueue,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetIOQueueTracer
			},
			{
				JET_tracetag.DiskVolumeManagement,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetDiskVolumeManagementTracer
			},
			{
				JET_tracetag.Callbacks,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetCallbacksTracer
			},
			{
				JET_tracetag.IOProblems,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetIOProblemsTracer
			},
			{
				JET_tracetag.Upgrade,
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess.ExTraceGlobals.JetUpgradeTracer
			}
		};

		private static readonly Hookable<Func<JET_resid, long, long, long>> getCurrentResourceUsageTestHook = Hookable<Func<JET_resid, long, long, long>>.Create(true, null);

		private static readonly Hookable<Action<string>> checkTableExistsTestHook = Hookable<Action<string>>.Create(true, null);

		private static readonly Hookable<Func<byte[], DateTime, int, int, DatabaseHeaderInfo>> getDatabaseHeaderInfoTestHook = Hookable<Func<byte[], DateTime, int, int, DatabaseHeaderInfo>>.Create(true, null);

		protected readonly Guid DbGuid;

		private static readonly object configurationLockObject = new object();

		private static int logFileSize = 1024;

		private static int logBuffers = JetDatabase.logFileSize * 2;

		private static int startFlushPct = 1;

		private static int stopFlushPct = 2;

		private static int eventLoggingLevel = 1;

		private static int maxInstances = 2;

		private static bool initializedJet;

		private static Hookable<Action> backgroundMaintenanceTestHook = Hookable<Action>.Create(true, null);

		private readonly string databaseFile;

		private bool lossyMount;

		private Instance jetInstance;

		private int computedJetCacheSizeMin;

		private int computedJetCacheSizeMax;

		private JetLogReplayStatus logReplayStatus;

		private bool transitionToActiveWasSuccessful;

		private Exception passiveMountException;

		private JET_OSSNAPID jetOsSnapId;

		private Database.SnapshotState snapshotState;

		private Thread logReplayThread;

		internal enum SnapshotBackup
		{
			None = -1,
			NotSnapshot,
			Snapshot
		}

		private enum InitType
		{
			Normal,
			ForLogReplay
		}

		private enum DbScanMode
		{
			DbDivergenceBasedScan = 1,
			LegacyFreeBasedScan
		}
	}
}
