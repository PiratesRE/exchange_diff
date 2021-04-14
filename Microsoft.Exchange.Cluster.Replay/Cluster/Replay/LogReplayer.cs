using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Isam.Esebcli;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogReplayer : ShipControl
	{
		public LogReplayer(IPerfmonCounters perfmonCounters, string fromPrefix, long fromNumber, string fromSuffix, IFileChecker fileChecker, IInspectLog inspectLogFile, ILogTruncater logTruncater, ISetBroken setBroken, ISetViable setViable, IReplayConfiguration configuration, ISetPassiveSeeding setPassiveSeeding, IReplicaProgress replicaProgress) : base(configuration.DestinationLogPath, fromPrefix, fromNumber, fromSuffix, setBroken, replicaProgress)
		{
			this.m_perfmonCounters = perfmonCounters;
			this.m_configuration = configuration;
			this.m_initialFromNumber = fromNumber;
			this.m_fileChecker = fileChecker;
			this.m_checkFilesForReplayHasRun = false;
			this.m_logTruncater = logTruncater;
			this.m_replayThreadWakeupEvent = new AutoResetEvent(false);
			this.m_replayLag = this.Configuration.ReplayLagTime;
			this.m_replayDir = this.Configuration.DestinationLogPath;
			this.m_waitForNextReplayLog = TimeSpan.FromMilliseconds((double)RegistryParameters.LogReplayerPauseDurationInMSecs);
			this.m_waitForNextStoreRpc = TimeSpan.FromMilliseconds((double)RegistryParameters.LogReplayerIdleStoreRpcIntervalInMSecs);
			this.m_setViable = setViable;
			this.m_stopCalled = false;
			this.m_setPassiveSeeding = setPassiveSeeding;
			this.m_queueHighPlayDownSuppression = new TransientErrorInfo();
			this.SuspendThreshold = RegistryParameters.LogReplayerSuspendThreshold;
			this.ResumeThreshold = RegistryParameters.LogReplayerResumeThreshold;
			this.UseSuspendLock = true;
			this.IsDismountControlledExternally = false;
			this.IsReplayLagDisabled = this.m_configuration.ReplayState.ReplayLagDisabled;
			this.m_logRepairWasPending = (this.State.LogRepairMode != LogRepairMode.Off);
			if (this.SuspendThreshold > this.ResumeThreshold)
			{
				this.IsCopyQueueBasedSuspendEnabled = true;
			}
			else
			{
				this.IsCopyQueueBasedSuspendEnabled = false;
			}
			if (RegistryParameters.LogReplayerDelayInMsec > 0)
			{
				this.m_testDelayEvent = new ManualResetEvent(false);
			}
			this.m_perfmonCounters.ReplayLagPercentage = (long)this.GetActualReplayLagPercentage();
			IMonitoringADConfigProvider monitoringADConfigProvider = Dependencies.MonitoringADConfigProvider;
			ICopyStatusClientLookup monitoringCopyStatusClientLookup = Dependencies.MonitoringCopyStatusClientLookup;
			this.m_dbScanControl = new LogReplayScanControl(configuration.Database, this.m_replayLag > TimeSpan.FromSeconds(0.0), monitoringADConfigProvider, monitoringCopyStatusClientLookup, this.m_perfmonCounters);
			LogReplayer.Tracer.TraceDebug<bool, int, int>((long)this.GetHashCode(), "Replayer suspendEnabled is {0} because SuspendThreshold={1}, ResumeThreshold={2}", this.IsCopyQueueBasedSuspendEnabled, this.SuspendThreshold, this.ResumeThreshold);
			ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "LogReplayer initialized - fromDir = {0}, fromNumber = {1}, fromPrefix = {2}, replayLag = {3}", new object[]
			{
				configuration.DestinationLogPath,
				fromNumber,
				fromPrefix,
				configuration.ReplayLagTime.ToString()
			});
			ExTraceGlobals.PFDTracer.TracePfd((long)this.GetHashCode(), "PFD CRS {0} LogReplayer initialized - fromDir = {1}, fromNumber = {2}, fromPrefix = {3}, replayLag = {4}", new object[]
			{
				32413,
				configuration.DestinationLogPath,
				fromNumber,
				fromPrefix,
				configuration.ReplayLagTime.ToString()
			});
			this.Resume();
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.LogReplayerTracer;
			}
		}

		internal static int GetActualReplayLagPercentage(IReplayConfiguration config)
		{
			DateTime utcNow = DateTime.UtcNow;
			int result = 0;
			TimeSpan timeSpan = TimeSpan.Zero;
			TimeSpan t = config.ReplayLagTime;
			if (t != EnhancedTimeSpan.Zero)
			{
				DateTime latestReplayTime = config.ReplayState.LatestReplayTime;
				if (latestReplayTime > ReplayState.ZeroFileTime && utcNow > latestReplayTime)
				{
					timeSpan = utcNow.Subtract(latestReplayTime);
				}
				result = Math.Min(100, (int)Math.Round(timeSpan.TotalMilliseconds / t.TotalMilliseconds * 100.0));
			}
			return result;
		}

		internal static bool DismountReplayDatabase(Guid dbGuid, string identity, string dbName, string localNodeName)
		{
			bool result = false;
			UnmountFlags unmountFlags = UnmountFlags.SkipCacheFlush;
			using (IStoreMountDismount storeMountDismountInstance = Dependencies.GetStoreMountDismountInstance(localNodeName))
			{
				try
				{
					ExTraceGlobals.LogReplayerTracer.TraceDebug<Guid, int>(0L, "UnmountDatabase called for {0} (flags={1}).", dbGuid, (int)unmountFlags);
					storeMountDismountInstance.UnmountDatabase(Guid.Empty, dbGuid, (int)unmountFlags);
					result = true;
				}
				catch (MapiExceptionNotFound arg)
				{
					ExTraceGlobals.LogReplayerTracer.TraceError<MapiExceptionNotFound>(0L, "UnmountDatabase: Ignoring MapiExceptionNotFound: {0}", arg);
					result = true;
				}
				catch (MapiRetryableException ex)
				{
					ExTraceGlobals.LogReplayerTracer.TraceError<MapiRetryableException>(0L, "UnmountDatabase Exception: {0}", ex);
					ReplayEventLogConstants.Tuple_LogReplayMapiException.LogEvent(identity, new object[]
					{
						dbName,
						ex.Message
					});
				}
				catch (MapiPermanentException ex2)
				{
					ExTraceGlobals.LogReplayerTracer.TraceError<MapiPermanentException>(0L, "LogReplay MapiPermanentException:{0}", ex2);
					ReplayEventLogConstants.Tuple_LogReplayMapiException.LogEvent(identity, new object[]
					{
						dbName,
						ex2.Message
					});
				}
			}
			return result;
		}

		public IReplayConfiguration Configuration
		{
			get
			{
				return this.m_configuration;
			}
		}

		public ILogTruncater LogTruncater
		{
			get
			{
				return this.m_logTruncater;
			}
		}

		private ReplayState State
		{
			get
			{
				return this.m_configuration.ReplayState;
			}
		}

		internal string LocalNodeName { get; set; }

		private IStoreRpc StoreReplayController
		{
			get
			{
				lock (this.m_storeLocker)
				{
					if (this.m_storeReplayController != null)
					{
						return this.m_storeReplayController;
					}
					this.m_storeReplayController = Dependencies.GetNewStoreControllerInstance(this.LocalNodeName);
					if (this.m_setPassiveSeeding.PassiveSeedingSourceContext != PassiveSeedingSourceContextEnum.Database)
					{
						LogReplayer.Tracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Mount starts", this.DatabaseName);
						this.m_storeReplayController.MountDatabase(Guid.Empty, this.Configuration.IdentityGuid, 8);
						LogReplayer.Tracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Mount complete", this.DatabaseName);
						this.IsDismountPending = true;
						int workerProcessId;
						int num;
						int num2;
						int num3;
						this.m_storeReplayController.GetDatabaseProcessInfo(this.Configuration.IdentityGuid, out workerProcessId, out num, out num2, out num3);
						this.State.WorkerProcessId = workerProcessId;
					}
					else
					{
						ExTraceGlobals.LogReplayerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Didn't MountDatabase for {0} because it is being a seeding source.", this.Configuration.IdentityGuid);
					}
				}
				return this.m_storeReplayController;
			}
		}

		internal bool IsDismountPending { get; set; }

		internal long MaxLogToReplay
		{
			get
			{
				return this.m_highestGenerationToReplay;
			}
		}

		private int SuspendThreshold { get; set; }

		private int ResumeThreshold { get; set; }

		private bool IsCopyQueueBasedSuspendEnabled { get; set; }

		internal bool UseSuspendLock { get; set; }

		internal bool IsDismountControlledExternally { get; set; }

		private bool IsReplayLagDisabled { get; set; }

		private string DatabaseName
		{
			get
			{
				return this.m_configuration.DatabaseName;
			}
		}

		public override Result ShipAction(long logFileNumber)
		{
			ExTraceGlobals.LogReplayerTracer.TraceDebug<long>((long)this.GetHashCode(), "ShipAction called, logFileNumber = {0}", logFileNumber);
			ExTraceGlobals.PFDTracer.TracePfd<int, long>((long)this.GetHashCode(), "PFD CRS {0} ShipAction called, logFileNumber = {1}", 28317, logFileNumber);
			lock (this)
			{
				this.SetHighestGenerationAvailable(logFileNumber);
			}
			return Result.Success;
		}

		public override void LogError(string inputFile, Exception ex)
		{
			ExTraceGlobals.LogReplayerTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Log Replay error Input file is: {0}" + Environment.NewLine + "Exception received was: {1}", inputFile, ex);
		}

		internal void WaitForIdle()
		{
			ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "WaitForIdle()");
			Thread thread = this.m_thread;
			if (thread != null)
			{
				thread.Join();
			}
		}

		internal void Suspend()
		{
			this.State.ReplaySuspended = true;
		}

		internal void Resume()
		{
			this.State.ReplaySuspended = false;
		}

		protected override void PrepareToStopInternal()
		{
			ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "PrepareToStop()");
			this.m_stopCalled = true;
			this.WakeupReplayer();
			if (this.m_testDelayEvent != null)
			{
				this.m_testDelayEvent.Set();
			}
		}

		protected override void StopInternal()
		{
			if (!this.m_fStopped)
			{
				Thread thread = this.m_thread;
				if (thread != null)
				{
					thread.Join();
					this.m_thread = null;
				}
				this.m_fStopped = true;
				lock (this.m_replayThreadWakeupEventLocker)
				{
					if (this.m_replayThreadWakeupEvent != null)
					{
						this.m_replayThreadWakeupEvent.Close();
						this.m_replayThreadWakeupEvent = null;
					}
				}
				this.CloseReplayController();
				base.EndOfStopTime = DateTime.UtcNow;
				TimerComponent.LogStopEvent("LogReplayer", this.Configuration.DatabaseName, base.PrepareToStopTime, base.StartOfStopTime, base.EndOfStopTime);
			}
			ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "LogReplayer Stop ()");
			ExTraceGlobals.PFDTracer.TracePfd<int>((long)this.GetHashCode(), "PFD CRS {0} LogReplayer Stopped ()", 24221);
		}

		protected override void TestDelaySleep()
		{
			int logReplayerDelayInMsec = RegistryParameters.LogReplayerDelayInMsec;
			if (logReplayerDelayInMsec > 0 && this.m_testDelayEvent != null)
			{
				this.m_testDelayEvent.WaitOne(logReplayerDelayInMsec);
			}
		}

		protected override Result InitializeStartContext()
		{
			ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "InitializeStartContext()");
			ExTraceGlobals.PFDTracer.TracePfd<int, LogReplayer>((long)this.GetHashCode(), "PFD CRS {0} InitializeStartContext() for {1} ", 22685, this);
			return Result.Success;
		}

		private static FailureTag ChooseFailureTag(MapiPermanentException ex)
		{
			if (ex is MapiExceptionNotFound || ex is MapiExceptionMdbOffline)
			{
				return FailureTag.NoOp;
			}
			if (!(ex is MapiExceptionCallFailed))
			{
				return FailureTag.NoOp;
			}
			int lowLevelError = ex.LowLevelError;
			EsentErrorException ex2 = EsentExceptionHelper.JetErrToException((JET_err)lowLevelError);
			if (ex2 is EsentDiskReadVerificationFailureException)
			{
				return FailureTag.RecoveryRedoLogCorruption;
			}
			return ReplicaInstance.TargetIsamErrorExceptionToFailureTag(ex2);
		}

		private bool TryGetSuspendLock()
		{
			LockOwner lockOwner;
			if (!this.State.SuspendLock.TryEnter(LockOwner.Component, false, out lockOwner))
			{
				ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "unable to get SuspendLock, restarting the instance");
				if (lockOwner != LockOwner.AttemptCopyLastLogs)
				{
					ExTraceGlobals.LogReplayerTracer.TraceDebug<LockOwner>((long)this.GetHashCode(), "unable to get SuspendLock, current owner: {0}, stopping the instance", lockOwner);
					this.m_setBroken.RestartInstanceSoon(true);
				}
				else
				{
					this.m_setBroken.RestartInstanceSoon(false);
				}
			}
			else
			{
				this.m_haveSuspendLock = true;
			}
			return this.m_haveSuspendLock;
		}

		private void ReleaseSuspendLock()
		{
			if (this.m_haveSuspendLock)
			{
				this.State.SuspendLock.Leave(LockOwner.Component);
				this.m_haveSuspendLock = false;
			}
		}

		private bool CheckFilesForReplay()
		{
			if (this.m_checkFilesForReplayHasRun)
			{
				return true;
			}
			try
			{
				if (!this.m_fileChecker.RecalculateRequiredGenerations())
				{
					return false;
				}
				if (0L == this.m_fileChecker.FileState.HighestGenerationPresent)
				{
					ExTraceGlobals.LogReplayerTracer.TraceError((long)this.GetHashCode(), "No logfiles. Replay cannot run");
					return false;
				}
				if (!this.m_fileChecker.FileState.RequiredLogfilesArePresent())
				{
					ExTraceGlobals.LogReplayerTracer.TraceError((long)this.GetHashCode(), "Required logfiles are not present. Replay requires generations {0} through {1}. Found generations {2} through {3}. Replay cannot be run until all required logfiles are present.", new object[]
					{
						this.m_fileChecker.FileState.LowestGenerationRequired,
						this.m_fileChecker.FileState.HighestGenerationRequired,
						this.m_fileChecker.FileState.LowestGenerationPresent,
						this.m_fileChecker.FileState.HighestGenerationPresent
					});
					return false;
				}
				if (!this.m_fileChecker.CheckRequiredLogfilesForPassiveOrInconsistentDatabase(true))
				{
					return false;
				}
				this.m_setViable.SetViable();
				this.m_fileChecker.CheckCheckpoint();
			}
			catch (FileCheckLogfileMissingException arg)
			{
				ExTraceGlobals.LogReplayerTracer.TraceError<FileCheckLogfileMissingException>((long)this.GetHashCode(), "FileCheckException thrown in CheckFilesForReplay. Exception is: {0}. We will wait for the log file to arrive", arg);
				return false;
			}
			catch (FileCheckException ex)
			{
				ExTraceGlobals.LogReplayerTracer.TraceError<FileCheckException>((long)this.GetHashCode(), "FileCheckException thrown in CheckFilesForReplay. Exception is: {0}", ex);
				this.m_setBroken.SetBroken(ReplicaInstance.TargetFileCheckerExceptionToFailureTag(ex), ReplayEventLogConstants.Tuple_FileCheckError, ex, new string[]
				{
					ex.ToString()
				});
				return false;
			}
			this.m_highestGenerationToReplay = this.m_fileChecker.FileState.LowestGenerationPresent;
			this.m_checkFilesForReplayHasRun = true;
			return true;
		}

		private bool PostReplayDatabaseCheck()
		{
			string destinationEdbPath = this.m_configuration.DestinationEdbPath;
			if (!File.Exists(destinationEdbPath))
			{
				ExTraceGlobals.LogReplayerTracer.TraceError<string>((long)this.GetHashCode(), "PostReplayChecks failed. Database {0} doesn't exist.", destinationEdbPath);
				this.m_setBroken.SetBroken(FailureTag.Reseed, ReplayEventLogConstants.Tuple_DatabaseNotPresentAfterReplay, new string[]
				{
					destinationEdbPath
				});
				return false;
			}
			return true;
		}

		private int GetActualReplayLagPercentage()
		{
			return LogReplayer.GetActualReplayLagPercentage(this.Configuration);
		}

		private void ReplayCompleted(long lastLogReplayed, ref JET_DBINFOMISC dbinfo)
		{
			ExTraceGlobals.LogReplayerTracer.TraceDebug<long, int, int>((long)this.GetHashCode(), "ReplayCompleted. LastLogReplayed={0}, dbinfo.Version={1}, dbinfo.genMaxRequired={2}", lastLogReplayed, dbinfo.ulVersion, dbinfo.genMaxRequired);
			long replayGenerationNumber = this.State.ReplayGenerationNumber;
			if (lastLogReplayed > replayGenerationNumber)
			{
				if (this.m_logRepairWasPending)
				{
					if (!LogRepair.ExitLogRepair(this.Configuration.IdentityGuid))
					{
						this.m_setBroken.RestartInstanceNow(ReplayConfigChangeHints.LogReplayerHitLogCorruption);
					}
					this.m_logRepairWasPending = false;
				}
				this.m_replicaProgress.ReportLogsReplayed(lastLogReplayed - replayGenerationNumber);
			}
			this.State.ReplayGenerationNumber = lastLogReplayed;
			string filenameFromGenerationNumber = base.GetFilenameFromGenerationNumber(lastLogReplayed);
			string logfile = Path.Combine(this.m_replayDir, filenameFromGenerationNumber);
			DateTime filetimeOfLog = base.GetFiletimeOfLog(logfile);
			this.m_configuration.ReplayState.LatestReplayTime = filetimeOfLog;
			if (this.m_configuration.ReplayState.ReplayGenerationNumber < this.m_highestGenerationToReplay)
			{
				this.m_configuration.ReplayState.CurrentReplayTime = filetimeOfLog;
			}
			if (this.PostReplayDatabaseCheck() && dbinfo.ulVersion != 0)
			{
				this.m_fileChecker.RecalculateRequiredGenerations(ref dbinfo);
				if (this.m_fileChecker.FileState.LatestFullBackupTime != null)
				{
					this.State.LatestFullBackupTime = this.m_fileChecker.FileState.LatestFullBackupTime.Value.ToUniversalTime();
				}
				if (this.m_fileChecker.FileState.LatestIncrementalBackupTime != null)
				{
					this.State.LatestIncrementalBackupTime = this.m_fileChecker.FileState.LatestIncrementalBackupTime.Value.ToUniversalTime();
				}
				if (this.m_fileChecker.FileState.LatestDifferentialBackupTime != null)
				{
					this.State.LatestDifferentialBackupTime = this.m_fileChecker.FileState.LatestDifferentialBackupTime.Value.ToUniversalTime();
				}
				if (this.m_fileChecker.FileState.LatestCopyBackupTime != null)
				{
					this.State.LatestCopyBackupTime = this.m_fileChecker.FileState.LatestCopyBackupTime.Value.ToUniversalTime();
				}
				if (this.m_fileChecker.FileState.SnapshotBackup != null)
				{
					this.State.SnapshotBackup = this.m_fileChecker.FileState.SnapshotBackup.Value;
				}
				if (this.m_fileChecker.FileState.SnapshotLatestFullBackup != null)
				{
					this.State.SnapshotLatestFullBackup = this.m_fileChecker.FileState.SnapshotLatestFullBackup.Value;
				}
				if (this.m_fileChecker.FileState.SnapshotLatestIncrementalBackup != null)
				{
					this.State.SnapshotLatestIncrementalBackup = this.m_fileChecker.FileState.SnapshotLatestIncrementalBackup.Value;
				}
				if (this.m_fileChecker.FileState.SnapshotLatestDifferentialBackup != null)
				{
					this.State.SnapshotLatestDifferentialBackup = this.m_fileChecker.FileState.SnapshotLatestDifferentialBackup.Value;
				}
				if (this.m_fileChecker.FileState.SnapshotLatestCopyBackup != null)
				{
					this.State.SnapshotLatestCopyBackup = this.m_fileChecker.FileState.SnapshotLatestCopyBackup.Value;
				}
				if (this.LogTruncater != null)
				{
					long genRequired = Math.Min((long)dbinfo.genMinRequired, lastLogReplayed);
					this.LogTruncater.RecordReplayGeneration(genRequired);
				}
			}
		}

		private LogReplayRpcFlags ChooseReplayFlags()
		{
			LogReplayRpcFlags result = LogReplayRpcFlags.SetDbScan | LogReplayRpcFlags.EnableDbScan;
			bool isFastLagPlaydownDesired = false;
			long replayQ;
			if (this.m_replayLag > TimeSpan.FromSeconds(0.0))
			{
				if (this.m_fPlayingDownLag || this.IsReplayLagDisabled)
				{
					isFastLagPlaydownDesired = true;
					replayQ = this.CurrentReplayQueue();
				}
				else
				{
					replayQ = Math.Max(0L, this.m_highestGenerationToReplay - this.State.ReplayGenerationNumber);
				}
			}
			else
			{
				replayQ = this.CurrentReplayQueue();
			}
			if (!this.m_dbScanControl.ShouldBeEnabled(isFastLagPlaydownDesired, replayQ))
			{
				result = LogReplayRpcFlags.SetDbScan;
			}
			return result;
		}

		private void LogReplayRpc(Guid mdbGuid, long replayGen)
		{
			ExTraceGlobals.LogReplayerTracer.TraceDebug<Guid, long>((long)this.GetHashCode(), "LogReplayRpc entered for mdbGuid={0}, generation={1}", mdbGuid, replayGen);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3905301821U);
			try
			{
				LogReplayRpcFlags logReplayRpcFlags = this.ChooseReplayFlags();
				ExTraceGlobals.LogReplayerTracer.TraceDebug<string, LogReplayRpcFlags>((long)this.GetHashCode(), "LogReplayRpc({0}) flags=0x{1:X}", this.DatabaseName, logReplayRpcFlags);
				uint num;
				JET_DBINFOMISC jet_DBINFOMISC;
				IPagePatchReply pagePatchReply;
				uint[] array;
				lock (this.m_storeLocker)
				{
					this.StoreReplayController.LogReplayRequest(mdbGuid, (uint)replayGen, (uint)logReplayRpcFlags, out num, out jet_DBINFOMISC, out pagePatchReply, out array);
				}
				if (pagePatchReply != null)
				{
					ThreadPool.QueueUserWorkItem(delegate(object obj)
					{
						this.SendPageToActive(obj as IPagePatchReply);
					}, pagePatchReply);
				}
				if (num > 1U)
				{
					this.ReplayCompleted((long)((ulong)(num - 1U)), ref jet_DBINFOMISC);
				}
				else
				{
					ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "LogReplayRpc skipped ReplayCompleted as nextLogToReplay is zero or 1.");
				}
				if (array != null)
				{
					this.PatchPassiveDatabase(array);
				}
			}
			catch (MapiExceptionNotFound mapiExceptionNotFound)
			{
				ExTraceGlobals.LogReplayerTracer.TraceError<MapiExceptionNotFound>((long)this.GetHashCode(), "LogReplay MapiExceptionNotFound:{0}", mapiExceptionNotFound);
				if (this.m_setPassiveSeeding.PassiveSeedingSourceContext != PassiveSeedingSourceContextEnum.Database)
				{
					this.m_setBroken.SetBroken(LogReplayer.ChooseFailureTag(mapiExceptionNotFound), ReplayEventLogConstants.Tuple_LogReplayMapiException, mapiExceptionNotFound, new string[]
					{
						mapiExceptionNotFound.Message
					});
				}
				else
				{
					ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "LogReplay MapiExceptionNotFound is ignored and SetBroken() skipped because instance is a passive seeding source.");
				}
			}
			catch (MapiExceptionMdbOffline mapiExceptionMdbOffline)
			{
				ExTraceGlobals.LogReplayerTracer.TraceError<MapiExceptionMdbOffline>((long)this.GetHashCode(), "LogReplay MapiExceptionMdbOffline:{0}", mapiExceptionMdbOffline);
				if (this.m_setPassiveSeeding.PassiveSeedingSourceContext != PassiveSeedingSourceContextEnum.Database)
				{
					this.m_setBroken.SetBroken(LogReplayer.ChooseFailureTag(mapiExceptionMdbOffline), ReplayEventLogConstants.Tuple_LogReplayMapiException, mapiExceptionMdbOffline, new string[]
					{
						mapiExceptionMdbOffline.Message
					});
				}
				else
				{
					ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "LogReplay MapiExceptionMdbOffline is ignored and SetBroken() skipped because instance is a passive seeding source.");
				}
			}
			catch (MapiRetryableException ex)
			{
				ExTraceGlobals.LogReplayerTracer.TraceError<MapiRetryableException>((long)this.GetHashCode(), "LogReplay MapiRetryableException:{0}", ex);
				if (this.m_setPassiveSeeding.PassiveSeedingSourceContext != PassiveSeedingSourceContextEnum.Database)
				{
					this.m_setBroken.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_LogReplayMapiException, ex, new string[]
					{
						ex.Message
					});
				}
				else
				{
					ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "LogReplay MapiRetryableException is ignored and SetBroken() skipped because instance is a passive seeding source.");
				}
			}
			catch (MapiPermanentException ex2)
			{
				ExTraceGlobals.LogReplayerTracer.TraceError<MapiPermanentException>((long)this.GetHashCode(), "LogReplay MapiPermanentException:{0}", ex2);
				this.m_setBroken.SetBroken(LogReplayer.ChooseFailureTag(ex2), ReplayEventLogConstants.Tuple_LogReplayMapiException, ex2, new string[]
				{
					ex2.Message
				});
			}
			catch (UnauthorizedAccessException ex3)
			{
				ExTraceGlobals.LogReplayerTracer.TraceError<UnauthorizedAccessException>((long)this.GetHashCode(), "LogReplay UnauthorizedAccessException:{0}", ex3);
				this.m_setBroken.SetBroken(FailureTag.Configuration, ReplayEventLogConstants.Tuple_LogReplayMapiException, ex3, new string[]
				{
					ex3.Message
				});
			}
			finally
			{
				this.m_lastStoreRpcUTC = DateTime.UtcNow;
			}
			ExTraceGlobals.LogReplayerTracer.TraceDebug<Guid, long>((long)this.GetHashCode(), "LogReplayRpc completed for mdbGuid={0}, generation={1}", mdbGuid, replayGen);
		}

		internal void SetReportingCallback(ISetBroken setBroken)
		{
			this.m_shipLogsSetBroken.SetReportingCallbacksForAcll(setBroken, null);
		}

		internal void SendFinalLogReplayRequest()
		{
			ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "ReplayFinalLogsDuringAcll() called");
			this.m_fAttemptFinalCopyCalled = true;
			lock (this)
			{
				if (base.ShiplogsActive)
				{
					LogReplayer.Tracer.TraceDebug((long)this.GetHashCode(), "Another ShipLogs() is running, creating GoingIdleEvent");
					base.GoingIdleEvent = new ManualResetEvent(false);
				}
			}
			if (base.GoingIdleEvent != null)
			{
				ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "ReplayFinalLogsDuringAcll is waiting on GoingIdleEvent");
				base.GoingIdleEvent.WaitOne();
				ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "ReplayFinalLogsDuringAcll: GoingIdleEvent was set");
			}
			ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "ReplayFinalLogsDuringAcll: now running ShipLogs()");
			base.ShipLogs(true);
		}

		private void PatchPassiveDatabase(IEnumerable<uint> corruptedPages)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PatchPassiveDatabase(");
			foreach (uint num in corruptedPages)
			{
				stringBuilder.AppendFormat(" {0} ", num);
			}
			stringBuilder.Append(")");
			ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), stringBuilder.ToString());
			this.PublishPageFailureItems(corruptedPages, IoErrorCategory.Read, FailureTag.PagePatchRequested);
			lock (this.m_storeLocker)
			{
				this.StoreReplayController.UnmountDatabase(Guid.Empty, this.Configuration.IdentityGuid, 16);
				this.CloseReplayController();
			}
			Exception ex = null;
			FailureTag failureTag = FailureTag.ReplayFailedToPagePatch;
			ExEventLog.EventTuple setBrokenEventTuple = ReplayEventLogConstants.Tuple_LogReplayPatchFailedPrepareException;
			bool flag2 = false;
			try
			{
				this.StopTruncationWithRetry();
				this.GetPagesAndUpdateDatabase(corruptedPages);
				flag2 = true;
				this.PublishPageFailureItems(corruptedPages, IoErrorCategory.None, FailureTag.PagePatchCompleted);
			}
			catch (LogTruncationException ex2)
			{
				ex = ex2;
				string message = string.Format("Error occurred while stopping global truncation: {0}", ex2);
				ExTraceGlobals.LogReplayerTracer.TraceError((long)this.GetHashCode(), message);
				failureTag = FailureTag.NoOp;
			}
			catch (EsebcliException ex3)
			{
				ex = ex3;
				string message2 = string.Format("Esebcli error when retrieving pages for patching passive: {0}", ex3);
				ExTraceGlobals.LogReplayerTracer.TraceError((long)this.GetHashCode(), message2);
				failureTag = FailureTag.NoOp;
			}
			catch (EsentErrorException ex4)
			{
				ex = ex4;
				failureTag = ReplicaInstance.TargetIsamErrorExceptionToFailureTag(ex4);
				setBrokenEventTuple = ReplayEventLogConstants.Tuple_LogReplayPatchFailedIsamException;
				LogReplayer.Tracer.TraceError<FailureTag, EsentErrorException>((long)this.GetHashCode(), "PatchPassiveDatabase failed: FailTag: {0} Ex: {1}", failureTag, ex4);
			}
			finally
			{
				if (!flag2)
				{
					this.m_setBroken.SetBroken(failureTag, setBrokenEventTuple, ex, new string[]
					{
						ex.ToString()
					});
				}
				else
				{
					this.m_setBroken.RestartInstanceSoon(false);
				}
			}
		}

		private void PublishPageFailureItems(IEnumerable<uint> pages, IoErrorCategory category, FailureTag tag)
		{
			foreach (uint num in pages)
			{
				long offset = (long)((ulong)(num + 1U) * 32768UL);
				IoErrorInfo ioErrorInfo = new IoErrorInfo(category, this.Configuration.DestinationEdbPath, offset, 32768L);
				Dependencies.FailureItemPublisher.PublishAction(tag, this.Configuration.IdentityGuid, this.Configuration.DatabaseName, ioErrorInfo);
			}
		}

		private void StopTruncationWithRetry()
		{
			int num = 0;
			try
			{
				IL_04:
				this.LogTruncater.StopTruncation();
			}
			catch (FailedToOpenLogTruncContextException ex)
			{
				Exception ex2;
				string message;
				if (ex.TryGetExceptionOrInnerOfType(out ex2))
				{
					message = string.Format("Shutting down exception on attempt {1} to stop global truncation for page patching. Exception: {0}", ex, num);
					ExTraceGlobals.LogReplayerTracer.TraceError((long)this.GetHashCode(), message);
					throw;
				}
				message = string.Format("Error {0} on attempt {1} to stop global truncation for page patching", ex, num);
				ExTraceGlobals.LogReplayerTracer.TraceError((long)this.GetHashCode(), message);
				if (++num == 10)
				{
					throw;
				}
				Thread.Sleep(3000);
				goto IL_04;
			}
			catch (LogTruncationException arg)
			{
				string message = string.Format("Error {0} on attempt {1} to stop global truncation for page patching", arg, num);
				ExTraceGlobals.LogReplayerTracer.TraceError((long)this.GetHashCode(), message);
				if (++num == 10)
				{
					throw;
				}
				Thread.Sleep(3000);
				goto IL_04;
			}
		}

		private void GetPagesAndUpdateDatabase(IEnumerable<uint> corruptedPages)
		{
			int num;
			int num2;
			Dictionary<uint, byte[]> pagesWithRetry = this.GetPagesWithRetry(corruptedPages, out num, out num2);
			string message = string.Format("Waiting for generation {0}", num2);
			ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), message);
			while (!this.ShouldCancelReplay() && this.m_highestGenerationAvailable < (long)num2)
			{
				this.ReplayPause(new long?((long)num2));
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2414226749U);
			if (!this.ShouldCancelReplay())
			{
				this.UpdateDatabaseWithPages(pagesWithRetry, (long)num, (long)num2);
			}
		}

		private Dictionary<uint, byte[]> GetPagesWithRetry(IEnumerable<uint> corruptedPages, out int lowestGenRequired, out int highestGenRequired)
		{
			int num = 0;
			Dictionary<uint, byte[]> pages;
			try
			{
				IL_02:
				pages = this.GetPages(corruptedPages, out highestGenRequired, out lowestGenRequired);
			}
			catch (EsebcliException arg)
			{
				string message = string.Format("Error {0} on attempt {1} to get pages for page patching", arg, num);
				ExTraceGlobals.LogReplayerTracer.TraceError((long)this.GetHashCode(), message);
				if (++num == 10)
				{
					throw;
				}
				Thread.Sleep(3000);
				goto IL_02;
			}
			return pages;
		}

		private Dictionary<uint, byte[]> GetPages(IEnumerable<uint> corruptedPages, out int highestGenRequired, out int lowestGenRequired)
		{
			lowestGenRequired = int.MaxValue;
			highestGenRequired = 0;
			AmServerName amServerName = new AmServerName(this.Configuration.SourceMachine);
			string text;
			IEsebcli esebcli = this.GetEsebcli(amServerName, out text);
			Dictionary<uint, byte[]> dictionary = new Dictionary<uint, byte[]>();
			foreach (uint num in corruptedPages)
			{
				if (this.ShouldCancelReplay())
				{
					break;
				}
				string message = string.Format("Getting page {0} from server {1}", num, amServerName);
				ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), message);
				int val = 0;
				int val2 = 0;
				dictionary[num] = esebcli.GetDatabasePage(text, this.Configuration.DatabaseName, this.Configuration.DatabaseName, (int)num, ref val, ref val2);
				highestGenRequired = Math.Max(highestGenRequired, val2);
				lowestGenRequired = Math.Min(lowestGenRequired, val);
				message = string.Format("Got page {0} from server {1}", num, amServerName);
				ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), message);
			}
			if (!this.ShouldCancelReplay())
			{
				foreach (uint num2 in corruptedPages)
				{
				}
			}
			return dictionary;
		}

		private void UpdateDatabaseWithPages(Dictionary<uint, byte[]> pages, long lowestGenRequired, long highestGenRequired)
		{
			string destinationEdbPath = this.Configuration.DestinationEdbPath;
			string message = string.Format("Starting to patch {0}", destinationEdbPath);
			ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), message);
			JET_INSTANCE instance;
			Api.JetCreateInstance2(out instance, this.Configuration.Identity, this.Configuration.DatabaseName, CreateInstanceGrbit.None);
			try
			{
				InstanceParameters instanceParameters = new InstanceParameters(instance);
				instanceParameters.EventLoggingLevel = EventLoggingLevels.Low;
				instanceParameters.BaseName = this.Configuration.LogFilePrefix;
				instanceParameters.LogFileDirectory = this.Configuration.DestinationLogPath;
				instanceParameters.SystemDirectory = this.Configuration.DestinationSystemPath;
				instanceParameters.NoInformationEvent = true;
				SystemParameters.EventLoggingLevel = 25;
				this.m_setViable.ClearViable();
				UnpublishedApi.JetBeginDatabaseIncrementalReseed(instance, destinationEdbPath, BeginDatabaseIncrementalReseedGrbit.None);
				foreach (KeyValuePair<uint, byte[]> keyValuePair in pages)
				{
					message = string.Format("Patching page {0}", keyValuePair.Key);
					ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), message);
					UnpublishedApi.JetPatchDatabasePages(instance, destinationEdbPath, (int)keyValuePair.Key, 1, keyValuePair.Value, keyValuePair.Value.Length, PatchDatabasePagesGrbit.None);
				}
				UnpublishedApi.JetEndDatabaseIncrementalReseed(instance, destinationEdbPath, (int)lowestGenRequired, 0, (int)highestGenRequired, EndDatabaseIncrementalReseedGrbit.None);
				this.m_setViable.SetViable();
				message = string.Format("Finished patching {0}", destinationEdbPath);
				ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), message);
			}
			finally
			{
				Api.JetTerm(instance);
			}
		}

		private void SendPageToActive(IPagePatchReply reply)
		{
			string databaseName = this.Configuration.DatabaseName;
			AmServerName amServerName = new AmServerName(this.Configuration.SourceMachine);
			try
			{
				string message = string.Format("Sending page {0} for database '{1}' to {2}", reply.PageNumber, databaseName, amServerName.NetbiosName);
				ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), message);
				string text;
				IEsebcli esebcli = this.GetEsebcli(amServerName, out text);
				esebcli.OnlinePatchDatabasePage(text, databaseName, databaseName, reply.PageNumber, reply.Token, reply.Data);
			}
			catch (EsebcliException ex)
			{
				string message2 = string.Format("Error sending page {0} for database '{1}' to {2}: {3}", new object[]
				{
					reply.PageNumber,
					databaseName,
					amServerName,
					ex
				});
				ExTraceGlobals.LogReplayerTracer.TraceError((long)this.GetHashCode(), message2);
			}
		}

		private IEsebcli GetEsebcli(AmServerName serverName, out string endPoint)
		{
			endPoint = null;
			IEsebcli esebcli = Dependencies.Container.Resolve<IEsebcli>();
			esebcli.SetServer(serverName.Fqdn);
			esebcli.SetTimeout(30000);
			string identity = this.m_configuration.Identity;
			EndPointInfo[] endPointInfo = esebcli.GetEndPointInfo();
			if (endPointInfo.Length == 0)
			{
				string text = string.Format("Esebcli failed to connect to {0}: no ESEBACK servers were found", serverName.Fqdn);
				ExTraceGlobals.LogReplayerTracer.TraceError((long)this.GetHashCode(), text);
				throw new EsebcliException(text);
			}
			foreach (EndPointInfo endPointInfo2 in endPointInfo)
			{
				if (string.Equals(identity, endPointInfo2.Annotation, StringComparison.OrdinalIgnoreCase))
				{
					endPoint = endPointInfo2.Annotation;
				}
			}
			if (endPoint == null)
			{
				endPoint = "Microsoft Information Store";
				LogReplayer.Tracer.TraceError<string, string>((long)this.GetHashCode(), "Failed to locate ESEBAK for dbguid {0}. Trying as if single instance store with {1}", identity, endPoint);
			}
			return esebcli;
		}

		internal void DisableReplayLag()
		{
			ExTraceGlobals.LogReplayerTracer.TraceDebug<string>((long)this.GetHashCode(), "DisableReplayLag( {0} ): Replay lag is being disabled...", this.m_configuration.DisplayName);
			this.IsReplayLagDisabled = true;
			this.WakeupReplayer();
		}

		internal void EnableReplayLag()
		{
			ExTraceGlobals.LogReplayerTracer.TraceDebug<string>((long)this.GetHashCode(), "EnableReplayLag( {0} ): Replay lag is being enabled...", this.m_configuration.DisplayName);
			this.IsReplayLagDisabled = false;
		}

		internal void WakeupReplayer()
		{
			lock (this.m_replayThreadWakeupEventLocker)
			{
				if (this.m_replayThreadWakeupEvent != null)
				{
					this.m_replayThreadWakeupEvent.Set();
				}
			}
		}

		private bool WaitUntilRequiredLogFilesShowup()
		{
			while (!this.ShouldCancelReplay() && !this.ShouldReacquireLock())
			{
				if (this.CheckFilesForReplay())
				{
					return true;
				}
				this.ReplayPause(null);
				this.m_perfmonCounters.ReplayLagPercentage = (long)this.GetActualReplayLagPercentage();
			}
			return false;
		}

		private bool ShouldReacquireLock()
		{
			return !this.m_haveSuspendLock && this.UseSuspendLock;
		}

		private void RecoveryThreadProc(object unused)
		{
			bool flag = false;
			while (!this.m_stopCalled)
			{
				bool flag2 = false;
				if (this.UseSuspendLock)
				{
					flag2 = this.TryGetSuspendLock();
				}
				if (flag2 || !this.UseSuspendLock)
				{
					bool flag3 = false;
					try
					{
						try
						{
							if (this.WaitUntilRequiredLogFilesShowup())
							{
								while (!this.ShouldCancelReplay() && !this.ShouldReacquireLock())
								{
									bool flag4 = true;
									bool flag5 = false;
									if (this.m_highestGenerationToReplay <= this.m_highestGenerationAvailable)
									{
										this.FindNextHighestGenerationToReplay(ref flag4, out flag5);
									}
									if (flag4)
									{
										if (flag5 || this.State.ReplayGenerationNumber != this.m_highestGenerationToReplay || this.m_lastStoreRpcUTC.Add(this.m_waitForNextStoreRpc) < DateTime.UtcNow)
										{
											this.LogReplayRpc(this.Configuration.IdentityGuid, this.m_highestGenerationToReplay);
										}
										this.ReplayPause(new long?(this.m_highestGenerationToReplay));
									}
									this.m_perfmonCounters.ReplayLagPercentage = (long)this.GetActualReplayLagPercentage();
								}
							}
							flag3 = true;
						}
						catch (Win32Exception ex)
						{
							ExTraceGlobals.LogReplayerTracer.TraceError<Win32Exception>((long)this.GetHashCode(), "LogReplayer got Win32Exception: {0}", ex);
							FailureTag failureTag;
							FileOperations.Win32ErrorCodeToIOFailureTag(ex.NativeErrorCode, FailureTag.NoOp, out failureTag);
							this.m_setBroken.SetBroken(failureTag, ReplayEventLogConstants.Tuple_LogReplayGenericError, ex, new string[]
							{
								ex.Message
							});
						}
						catch (IOException ex2)
						{
							string text = ex2.ToString();
							ExTraceGlobals.LogReplayerTracer.TraceError<string>((long)this.GetHashCode(), "LogReplayer got: {0}", text);
							this.m_setBroken.SetBroken(ReplicaInstance.IOExceptionToFailureTag(ex2), ReplayEventLogConstants.Tuple_LogReplayGenericError, ex2, new string[]
							{
								text
							});
						}
						continue;
					}
					finally
					{
						if (!flag3 || !this.IsDismountControlledExternally)
						{
							this.DismountAndCloseReplayController();
						}
						else
						{
							this.CloseReplayController();
						}
						flag = true;
						this.ReleaseSuspendLock();
					}
				}
				this.ReplayPause(null);
			}
			if (!flag)
			{
				if (!this.IsDismountControlledExternally)
				{
					this.DismountAndCloseReplayController();
					return;
				}
				this.CloseReplayController();
			}
		}

		private void FindNextHighestGenerationToReplay(ref bool allowRpc, out bool maxReplayGenerationReset)
		{
			LogReplayPlayDownReason logReplayPlayDownReason = LogReplayPlayDownReason.None;
			maxReplayGenerationReset = false;
			this.m_highestGenerationToReplay = Math.Max(this.m_highestGenerationToReplay, this.GetHighestGenerationToReplay());
			string text;
			if (this.m_fLogsScannedAtLeastOnce && this.ShouldPlayDownReplayLag(this.m_highestGenerationToReplay + 1L, out logReplayPlayDownReason, out text))
			{
				long num = Math.Max(0L, this.m_highestGenerationToReplay - (long)RegistryParameters.LogReplayerScanMoreLogsWhenReplayWithinThreshold);
				if (this.State.ReplayGenerationNumber < num)
				{
					ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "FindNextHighestGenerationToReplay( {0} ): Skipped this call since it was throttled. m_fLogsScannedAtLeastOnce={1}, ReplayGen={2}, replayGenThresholdToContinue={3}, m_highestGenerationToReplay={4}", new object[]
					{
						this.m_configuration.DisplayName,
						this.m_fLogsScannedAtLeastOnce,
						this.State.ReplayGenerationNumber,
						num,
						this.m_highestGenerationToReplay
					});
					return;
				}
			}
			int num2 = 0;
			int logReplayerMaxLogsToScanInOneIteration = RegistryParameters.LogReplayerMaxLogsToScanInOneIteration;
			bool flag;
			while (!this.ShouldPauseReplay(this.m_highestGenerationToReplay + 1L, out flag, out logReplayPlayDownReason))
			{
				if (this.ShouldCancelReplay() || this.ShouldReacquireLock())
				{
					allowRpc = false;
					break;
				}
				this.m_fLogsScannedAtLeastOnce = true;
				if (!flag)
				{
					this.m_highestGenerationToReplay += 1L;
					if ((logReplayPlayDownReason == LogReplayPlayDownReason.LagDisabled || logReplayPlayDownReason == LogReplayPlayDownReason.NotEnoughFreeSpace || logReplayPlayDownReason == LogReplayPlayDownReason.NormalLogReplay) && ++num2 > logReplayerMaxLogsToScanInOneIteration)
					{
						ExTraceGlobals.LogReplayerTracer.TraceDebug<string, int, LogReplayPlayDownReason>((long)this.GetHashCode(), "FindNextHighestGenerationToReplay( {0} ): Throttling loop after scanning {1} logs. playDownReason={2}", this.m_configuration.DisplayName, logReplayerMaxLogsToScanInOneIteration, logReplayPlayDownReason);
						break;
					}
				}
				else
				{
					maxReplayGenerationReset = true;
					num2 = 0;
					this.m_fLogsScannedAtLeastOnce = false;
				}
			}
			ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "FindNextHighestGenerationToReplay( {0} ): Calculated m_highestGenerationToReplay={1}, allowRpc={2}, maxReplayGenerationReset={3}", new object[]
			{
				this.m_configuration.DisplayName,
				this.m_highestGenerationToReplay,
				allowRpc,
				maxReplayGenerationReset
			});
		}

		private void DismountAndCloseReplayController()
		{
			lock (this.m_storeLocker)
			{
				try
				{
					if (this.IsDismountPending)
					{
						LogReplayer.DismountReplayDatabase(this.m_configuration.IdentityGuid, this.m_configuration.Identity, this.m_configuration.Name, this.LocalNodeName);
					}
				}
				finally
				{
					this.IsDismountPending = false;
					this.CloseReplayController();
				}
			}
		}

		private void CloseReplayController()
		{
			lock (this.m_storeLocker)
			{
				if (this.m_storeReplayController != null)
				{
					LogReplayer.Tracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: ReplayController closed", this.DatabaseName);
					this.m_storeReplayController.Close();
					this.m_storeReplayController = null;
				}
			}
		}

		private long GetHighestGenerationToReplay()
		{
			return Math.Min(this.m_highestGenerationAvailable, Math.Max(this.State.ReplayGenerationNumber, this.m_fileChecker.FileState.HighestGenerationRequired));
		}

		private void SetHighestGenerationAvailable(long generation)
		{
			if (!this.ShouldCancelReplay())
			{
				this.m_highestGenerationAvailable = Math.Max(generation, this.m_highestGenerationAvailable);
				this.m_perfmonCounters.ReplayNotificationGenerationNumber = this.m_highestGenerationAvailable;
				ExTraceGlobals.LogReplayerTracer.TraceDebug<long>((long)this.GetHashCode(), "HighestGenerationAvailable = {0}=0x{0:x}", this.m_highestGenerationAvailable);
				if (this.m_thread == null)
				{
					this.StartReplayThread();
				}
				if (generation != 0L && !this.State.ReplaySuspended)
				{
					this.WakeupReplayer();
				}
			}
		}

		private void StartReplayThread()
		{
			this.m_thread = new Thread(new ParameterizedThreadStart(this.RecoveryThreadProc));
			this.m_thread.Start();
		}

		private void ReplayPause(long? generation)
		{
			TimeSpan waitForNextReplayLog = this.m_waitForNextReplayLog;
			ExTraceGlobals.LogReplayerTracer.TraceDebug<TimeSpan, long?>((long)this.GetHashCode(), "Pausing replay for {0} at generation {1}", waitForNextReplayLog, generation);
			this.m_replayThreadWakeupEvent.WaitOne(waitForNextReplayLog, false);
		}

		private bool ShouldPauseReplay(long generation, out bool maxReplayGenerationReset, out LogReplayPlayDownReason playDownReasonEnum)
		{
			maxReplayGenerationReset = false;
			playDownReasonEnum = LogReplayPlayDownReason.None;
			TimeSpan timeSpan = this.m_waitForNextReplayLog;
			if (generation == 0L)
			{
				return true;
			}
			string filenameFromGenerationNumber = base.GetFilenameFromGenerationNumber(generation);
			string text = Path.Combine(this.m_replayDir, filenameFromGenerationNumber);
			FileInfo fileInfo = new FileInfo(text);
			DateTime dateTime = fileInfo.LastWriteTimeUtc;
			if (dateTime < CultureInfo.InstalledUICulture.Calendar.MinSupportedDateTime)
			{
				dateTime = CultureInfo.InstalledUICulture.Calendar.MinSupportedDateTime;
			}
			DateTime utcNow = DateTime.UtcNow;
			string text2 = null;
			long highestGenerationToReplay = this.m_highestGenerationToReplay;
			string text3 = null;
			if (dateTime > utcNow)
			{
				dateTime = utcNow;
			}
			TimeSpan timeSpan2 = utcNow - dateTime;
			if (fileInfo.Exists)
			{
				if (this.m_configuration.ReplayState.ReplayGenerationNumber == this.m_highestGenerationToReplay)
				{
					this.m_configuration.ReplayState.CurrentReplayTime = fileInfo.LastWriteTimeUtc;
				}
			}
			else
			{
				timeSpan2 = TimeSpan.Zero;
			}
			bool flag;
			if (this.ReplayerShouldBeSuspended(out text2))
			{
				flag = true;
			}
			else if (generation > this.m_highestGenerationAvailable)
			{
				flag = true;
				text2 = "generation > m_highestGenerationAvailable";
			}
			else if (!fileInfo.Exists)
			{
				flag = false;
			}
			else if (generation <= this.m_fileChecker.FileState.HighestGenerationRequired)
			{
				flag = false;
				playDownReasonEnum = LogReplayPlayDownReason.InRequiredRange;
				this.m_lastPlayDownReason = playDownReasonEnum;
				ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "ShouldPauseReplay( {0} ) returns false. Log file is in required range. (generation = {1}, logfileAge = {2}, lowestGenReq = {3}, highestGenReq = {4})", new object[]
				{
					this.m_configuration.DisplayName,
					generation,
					timeSpan2,
					this.m_fileChecker.FileState.LowestGenerationRequired,
					this.m_fileChecker.FileState.HighestGenerationRequired
				});
				ReplayCrimsonEvents.LogReplayPlayingDownLogsInRequiredRange.LogPeriodic<string, Guid, long, TimeSpan, long, long>(this.m_configuration.Identity + this.m_fileChecker.FileState.HighestGenerationRequired, DiagCore.DefaultEventSuppressionInterval, this.m_configuration.Name, this.m_configuration.IdentityGuid, generation, timeSpan2, this.m_fileChecker.FileState.LowestGenerationRequired, this.m_fileChecker.FileState.HighestGenerationRequired);
			}
			else if (timeSpan2 < this.m_replayLag)
			{
				if (this.ShouldPlayDownReplayLag(generation, out playDownReasonEnum, out text3))
				{
					if (!this.m_fPlayingDownLag || this.m_lastPlayDownReason != playDownReasonEnum)
					{
						ReplayCrimsonEvents.LogReplayPlayingDownLogsInLagRange.Log<string, string, long, TimeSpan, string, LogReplayPlayDownReason>(this.m_configuration.Name, this.m_configuration.Identity, generation, timeSpan2, text3, playDownReasonEnum);
						this.m_fPlayingDownLag = true;
						this.m_lastPlayDownReason = playDownReasonEnum;
					}
					flag = false;
				}
				else
				{
					flag = true;
					text2 = "logfileAge < m_replayLag";
					timeSpan = this.m_replayLag - timeSpan2;
				}
			}
			else
			{
				flag = false;
				playDownReasonEnum = LogReplayPlayDownReason.NormalLogReplay;
				this.m_lastPlayDownReason = playDownReasonEnum;
			}
			if (flag && this.m_fPlayingDownLag)
			{
				if (!this.ShouldPlayDownReplayLag(generation, out playDownReasonEnum, out text3))
				{
					this.m_highestGenerationToReplay = this.GetHighestGenerationToReplay();
					this.m_fPlayingDownLag = false;
					maxReplayGenerationReset = true;
					flag = false;
					text2 = null;
					timeSpan = this.m_waitForNextReplayLog;
					ExTraceGlobals.LogReplayerTracer.TraceDebug<string, long, long>((long)this.GetHashCode(), "ShouldPauseReplay( {0} ) returns false. ReplayLag is not being played down anymore, so setting back m_highestGenerationToReplay to {1}, from {2}", this.m_configuration.DisplayName, this.m_highestGenerationToReplay, highestGenerationToReplay);
					ReplayCrimsonEvents.LogReplayReinstatingReplayLag.Log<string, Guid, long, TimeSpan, long, long>(this.m_configuration.Name, this.m_configuration.IdentityGuid, generation, timeSpan2, highestGenerationToReplay, this.m_highestGenerationToReplay);
				}
				else if (this.m_lastPlayDownReason != playDownReasonEnum)
				{
					ReplayCrimsonEvents.LogReplayPlayingDownLogsInLagRange.Log<string, string, long, TimeSpan, string, LogReplayPlayDownReason>(this.m_configuration.Name, this.m_configuration.Identity, generation, timeSpan2, text3, playDownReasonEnum);
					this.m_lastPlayDownReason = playDownReasonEnum;
				}
			}
			LogReplayPlayDownReason logReplayPlayDownReason = playDownReasonEnum;
			if (logReplayPlayDownReason == LogReplayPlayDownReason.NormalLogReplay)
			{
				logReplayPlayDownReason = LogReplayPlayDownReason.None;
			}
			this.Configuration.ReplayState.ReplayLagPlayDownReason = logReplayPlayDownReason;
			if (flag)
			{
				ExTraceGlobals.LogReplayerTracer.TraceDebug((long)this.GetHashCode(), "ShouldPauseReplay returns true ( generation = {0}=0x{0:x}, m_highestGenerationAvailable = {1}=0x{1:x}, logfileCreationTime = {2}, currentTime = {3}, logfileAge = {4}, m_replayLag = {5}, timeToPause = {6}, reasonForPause = '{7}', logfile = '{8}' )", new object[]
				{
					generation,
					this.m_highestGenerationAvailable,
					dateTime.ToString(),
					utcNow.ToString(),
					timeSpan2.ToString(),
					this.m_replayLag.ToString(),
					timeSpan.ToString(),
					text2,
					text
				});
			}
			return flag;
		}

		private bool ShouldPlayDownReplayLag(long generation, out LogReplayPlayDownReason reasonEnum, out string reason)
		{
			bool result = false;
			long num = 0L;
			reason = null;
			reasonEnum = LogReplayPlayDownReason.None;
			string arg;
			if (this.IsReplayLagDisabled)
			{
				result = true;
				reasonEnum = LogReplayPlayDownReason.LagDisabled;
				reason = "Replay lag has been disabled.";
				ExTraceGlobals.LogReplayerTracer.TraceDebug<string, long, string>((long)this.GetHashCode(), "ShouldPlayDownReplayLag( {0} ) returns true for generation {1}. {2}", this.m_configuration.DisplayName, generation, reason);
			}
			else if (this.IsReplayQueueTooHighWithSuppression(out num, out arg))
			{
				result = true;
				reasonEnum = LogReplayPlayDownReason.NotEnoughFreeSpace;
				reason = string.Format("There are too many logs ({0}) in the lag range, or suppression of {1} was applied. {2}", num, this.m_queueHighPlayDownDisabledSuppressionDuration, arg);
				ExTraceGlobals.LogReplayerTracer.TraceDebug<string, long, string>((long)this.GetHashCode(), "ShouldPlayDownReplayLag( {0} ) returns true for generation {1}. {2}", this.m_configuration.DisplayName, generation, reason);
			}
			return result;
		}

		private bool IsReplayQueueTooHighWithSuppression(out long replayQ, out string error)
		{
			bool result = this.m_fPlayingDownLag;
			if (this.IsReplayQueueTooHigh(out replayQ, out error))
			{
				if (this.m_queueHighPlayDownSuppression.ReportFailure(this.m_queueHighPlayDownEnabledSuppressionDuration))
				{
					result = true;
				}
			}
			else if (this.m_queueHighPlayDownSuppression.ReportSuccess(this.m_queueHighPlayDownDisabledSuppressionDuration))
			{
				result = false;
			}
			return result;
		}

		private long CurrentReplayQueue()
		{
			return Math.Max(0L, this.State.InspectorGenerationNumber - this.State.ReplayGenerationNumber);
		}

		private bool IsReplayQueueTooHigh(out long replayQ, out string error)
		{
			replayQ = this.CurrentReplayQueue();
			LogReplayer.LogSpaceInfo logSpaceInfo = new LogReplayer.LogSpaceInfo(this.Configuration);
			return logSpaceInfo.CheckLogsTakingUpTooMuchSpace(replayQ, out error);
		}

		private bool ShouldCancelReplay()
		{
			bool result = false;
			LockOwner lockOwner;
			if (this.State.SuspendLock.ShouldGiveUpLock(out lockOwner))
			{
				result = true;
				if (lockOwner != LockOwner.AttemptCopyLastLogs)
				{
					ExTraceGlobals.LogReplayerTracer.TraceDebug<LockOwner>((long)this.GetHashCode(), "should give up SuspendLock, highest priority pending owner: {0}, stopping the instance", lockOwner);
					this.m_setBroken.RestartInstanceSoon(true);
				}
				else
				{
					this.m_setBroken.RestartInstanceSoon(false);
				}
			}
			if (this.m_stopCalled)
			{
				result = true;
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(2149985597U, ref result);
			return result;
		}

		private bool ReplayerShouldBeSuspended(out string reasonForPause)
		{
			reasonForPause = null;
			if (this.m_setPassiveSeeding.PassiveSeedingSourceContext == PassiveSeedingSourceContextEnum.Database)
			{
				LogReplayer.Tracer.TraceDebug((long)this.GetHashCode(), "LogReplay suspending because copy is a passive seeding source.");
				reasonForPause = "Copy is a passive seeding source";
				this.Suspend();
				this.CloseReplayController();
			}
			else if (!this.IsCopyQueueBasedSuspendEnabled)
			{
				this.Resume();
			}
			else
			{
				long inspectorGenerationNumber = this.State.InspectorGenerationNumber;
				long copyGenerationNumber = this.State.CopyGenerationNumber;
				long num = Math.Max(0L, copyGenerationNumber - inspectorGenerationNumber);
				if (!this.State.ReplaySuspended && num > (long)this.SuspendThreshold)
				{
					reasonForPause = "InspectorQueue is too long";
					long arg;
					string text;
					if (this.IsReplayQueueTooHigh(out arg, out text))
					{
						LogReplayer.Tracer.TraceError((long)this.GetHashCode(), "Skipping suspending log replay because the replay queue is too high");
					}
					else if (!this.State.ReplaySuspended)
					{
						LogReplayer.Tracer.TraceError<long, long, long>((long)this.GetHashCode(), "Suspending log replay because lastCopied({0}) - lastInspected({1}) = {2}", copyGenerationNumber, inspectorGenerationNumber, num);
						this.Suspend();
						ReplayEventLogConstants.Tuple_LogReplaySuspendedDueToCopyQ.LogEvent(this.m_configuration.Identity, new object[]
						{
							this.m_configuration.DatabaseName,
							num,
							this.SuspendThreshold,
							this.ResumeThreshold
						});
					}
				}
				else if (this.State.ReplaySuspended)
				{
					long arg;
					string text;
					bool flag = this.IsReplayQueueTooHigh(out arg, out text);
					if (!flag && num >= (long)this.ResumeThreshold)
					{
						reasonForPause = "InspectorQueue is too long";
					}
					else
					{
						if (flag)
						{
							LogReplayer.Tracer.TraceDebug<long>((long)this.GetHashCode(), "Resuming log replay because the replay queue {0} is too high.", arg);
						}
						else
						{
							LogReplayer.Tracer.TraceDebug<long, long, long>((long)this.GetHashCode(), "Resuming log replay because lastCopied({0}) - lastInspected({1}) = {2}", copyGenerationNumber, inspectorGenerationNumber, num);
						}
						this.Resume();
						ReplayEventLogConstants.Tuple_LogReplayResumedDueToCopyQ.LogEvent(this.m_configuration.Identity, new object[]
						{
							this.m_configuration.DatabaseName,
							num,
							this.ResumeThreshold
						});
					}
				}
			}
			return this.State.ReplaySuspended;
		}

		private const int MaxPatchRetryAttempts = 10;

		private const int PatchRetrySleepIntervalMs = 3000;

		private const int EseBackTimeoutInMs = 30000;

		private readonly TimeSpan m_queueHighPlayDownEnabledSuppressionDuration = TimeSpan.FromSeconds((double)RegistryParameters.LogReplayQueueHighPlayDownEnableSuppressionWindowInSecs);

		private readonly TimeSpan m_queueHighPlayDownDisabledSuppressionDuration = TimeSpan.FromSeconds((double)RegistryParameters.LogReplayQueueHighPlayDownDisableSuppressionWindowInSecs);

		private readonly IPerfmonCounters m_perfmonCounters;

		private readonly IReplayConfiguration m_configuration;

		private readonly long m_initialFromNumber;

		private readonly IFileChecker m_fileChecker;

		private readonly ILogTruncater m_logTruncater;

		private readonly ISetViable m_setViable;

		private readonly TimeSpan m_replayLag;

		private readonly string m_replayDir;

		private readonly TimeSpan m_waitForNextReplayLog;

		private readonly TimeSpan m_waitForNextStoreRpc;

		private DateTime m_lastStoreRpcUTC;

		private EventWaitHandle m_replayThreadWakeupEvent;

		private ManualResetEvent m_testDelayEvent;

		private ISetPassiveSeeding m_setPassiveSeeding;

		private bool m_haveSuspendLock;

		private bool m_checkFilesForReplayHasRun;

		private bool m_stopCalled;

		private bool m_fStopped;

		private IStoreRpc m_storeReplayController;

		private Thread m_thread;

		private long m_highestGenerationAvailable;

		private long m_highestGenerationToReplay;

		private bool m_fPlayingDownLag;

		private LogReplayPlayDownReason m_lastPlayDownReason;

		private bool m_fLogsScannedAtLeastOnce;

		private TransientErrorInfo m_queueHighPlayDownSuppression;

		private object m_storeLocker = new object();

		private object m_replayThreadWakeupEventLocker = new object();

		private bool m_logRepairWasPending;

		private LogReplayScanControl m_dbScanControl;

		private struct LogSpaceInfo
		{
			public long MaxLogsForReplayLag { get; private set; }

			public ByteQuantifiedSize TotalDiskFreeSpace { get; private set; }

			public ByteQuantifiedSize TotalDiskSpace { get; private set; }

			public ulong LowSpacePlaydownThresholdInMB { get; private set; }

			public int CurrentFreeSpacePercentage { get; private set; }

			public LogSpaceInfo(IReplayConfiguration config)
			{
				this = default(LogReplayer.LogSpaceInfo);
				if (RegistryParameters.LogReplayerMaximumLogsForReplayLag > 0)
				{
					LogReplayer.Tracer.TraceDebug<string, int>((long)this.GetHashCode(), "LogSpaceInfo..ctor(): ({0}): LogReplayerMaximumLogsForReplayLag regkey is set to max of {1} logs", config.DisplayName, RegistryParameters.LogReplayerMaximumLogsForReplayLag);
					this.MaxLogsForReplayLag = (long)RegistryParameters.LogReplayerMaximumLogsForReplayLag;
				}
				if (RegistryParameters.ReplayLagLowSpacePlaydownThresholdInMB > 0)
				{
					this.LowSpacePlaydownThresholdInMB = (ulong)((long)RegistryParameters.ReplayLagLowSpacePlaydownThresholdInMB);
				}
				string destinationLogPath = config.DestinationLogPath;
				ulong bytesValue;
				ulong bytesValue2;
				Exception freeSpace = DiskHelper.GetFreeSpace(destinationLogPath, out bytesValue, out bytesValue2);
				if (freeSpace != null)
				{
					LogReplayer.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "LogSpaceInfo..ctor(): GetFreeSpace() failed against directory '{0}'. Exception: {1}", destinationLogPath, freeSpace);
					throw freeSpace;
				}
				this.TotalDiskFreeSpace = ByteQuantifiedSize.FromBytes(bytesValue2);
				this.TotalDiskSpace = ByteQuantifiedSize.FromBytes(bytesValue);
				this.CurrentFreeSpacePercentage = DiskHelper.GetFreeSpacePercentage(this.TotalDiskFreeSpace.ToBytes(), this.TotalDiskSpace.ToBytes());
				LogReplayer.Tracer.TraceDebug((long)this.GetHashCode(), "LogSpaceInfo..ctor(): ({4}): The free space limit is {2}MB and it is currently {3}% of total disk space. [Free Space: {0}, Total Space: {1}]", new object[]
				{
					this.TotalDiskFreeSpace,
					this.TotalDiskSpace,
					this.LowSpacePlaydownThresholdInMB,
					this.CurrentFreeSpacePercentage,
					config.DisplayName
				});
			}

			public bool CheckLogsTakingUpTooMuchSpace(long replayQueue, out string error)
			{
				error = null;
				if (this.TotalDiskFreeSpace.ToBytes() / 1048576UL < this.LowSpacePlaydownThresholdInMB)
				{
					error = string.Format("There is not enough free space on the disk. The allowed free space is {0}MB but it is currently only {1}MB of total disk space. [Free Space: {2}, Total Space: {3}]", new object[]
					{
						this.LowSpacePlaydownThresholdInMB,
						this.TotalDiskFreeSpace.ToBytes() / 1048576UL,
						this.TotalDiskFreeSpace,
						this.TotalDiskSpace
					});
					return true;
				}
				if (this.MaxLogsForReplayLag != 0L && replayQueue > this.MaxLogsForReplayLag)
				{
					error = string.Format("Maximum number of logs allowed based on registry override (LogReplayerMaximumLogsForReplayLag): {0} logs.", this.MaxLogsForReplayLag);
					return true;
				}
				return false;
			}
		}
	}
}
