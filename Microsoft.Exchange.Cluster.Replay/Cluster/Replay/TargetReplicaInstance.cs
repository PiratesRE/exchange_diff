using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.EseRepl;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TargetReplicaInstance : ReplicaInstance
	{
		public TargetReplicaInstance(ReplayConfiguration replayConfiguration, ReplicaInstance previousReplicaInstance, IPerfmonCounters perfCounters) : base(replayConfiguration, true, previousReplicaInstance, perfCounters)
		{
			TargetReplicaInstance <>4__this = this;
			ReplicaInstance.DisposeIfActionUnsuccessful(delegate
			{
				<>4__this.TraceDebug("created");
				ExTraceGlobals.PFDTracer.TracePfd<int, ReplayConfigType>((long)<>4__this.GetHashCode(), "PFD CRS {0} TargetReplicaIntance {1} is created", 23197, replayConfiguration.Type);
				if (<>4__this.IsThirdPartyReplicationEnabled)
				{
					return;
				}
				<>4__this.InitializeSuspendState();
				<>4__this.InitializeCurrentContext();
			}, this);
		}

		internal static void EnsureTargetDismounted(IReplayConfiguration config)
		{
			Exception ex = null;
			try
			{
				using (IStoreMountDismount storeMountDismountInstance = Dependencies.GetStoreMountDismountInstance(null))
				{
					storeMountDismountInstance.UnmountDatabase(Guid.Empty, config.IdentityGuid, 16);
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>(0L, "EnsureTargetDismounted {0}: UnmountDatabase successfully unmounted.", config.DisplayName);
				}
			}
			catch (MapiExceptionNotFound)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>(0L, "EnsureTargetDismounted {0}: UnmountDatabase MapiExceptionNotFound indicates store did not have the db mounted.", config.DisplayName);
			}
			catch (MapiPermanentException ex2)
			{
				ex = ex2;
			}
			catch (MapiRetryableException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, Exception>(0L, "EnsureTargetDismounted {0}: UnmountDatabase exception ignored: {1}", config.DisplayName, ex);
				ReplayEventLogConstants.Tuple_LogReplayMapiException.LogEvent(config.Identity, new object[]
				{
					config.DisplayName,
					ex.Message
				});
			}
		}

		internal static void SyncTargetSuspendResumeState(ReplayConfiguration config)
		{
			Exception ex = null;
			try
			{
				Dependencies.ReplayCoreManager.ReplicaInstanceManager.SyncSuspendResumeState(config.IdentityGuid);
			}
			catch (TaskServerException ex2)
			{
				ex = ex2;
			}
			catch (TaskServerTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, Exception>(0L, "SyncTargetSuspendResumeState {0}: SyncSuspendResumeState exception ignored: {1}", config.DisplayName, ex);
				ReplayEventLogConstants.Tuple_SyncSuspendResumeOperationFailed.LogEvent(config.Identity, new object[]
				{
					config.DisplayName,
					ex.Message
				});
			}
		}

		internal bool IsIncrementalReseedInProgress(string database, Guid databaseGuid)
		{
			bool result;
			try
			{
				JET_DBINFOMISC passiveDatabaseFileInfo = FileChecker.GetPassiveDatabaseFileInfo(database, base.Configuration.DatabaseName, databaseGuid, base.Configuration.ServerName);
				result = EseHelper.IsIncrementalReseedInProgress(passiveDatabaseFileInfo);
			}
			catch (FileCheckException arg)
			{
				ExTraceGlobals.FileCheckerTracer.TraceError<string, Guid, FileCheckException>((long)this.GetHashCode(), "{0} FileChecker.IsIncrementalReseedInProgress(): GetPassiveDatabaseFileInfo() for database {1} threw exception. Returning true: {2}", base.Configuration.DisplayName, databaseGuid, arg);
				result = true;
			}
			return result;
		}

		internal override bool GetSignatureAndCheckpoint(out JET_SIGNATURE? logfileSignature, out long lowestGenerationRequired, out long highestGenerationRequired, out long lastGenerationBackedUp)
		{
			logfileSignature = base.FileChecker.FileState.LogfileSignature;
			lowestGenerationRequired = base.FileChecker.FileState.LowestGenerationRequired;
			highestGenerationRequired = base.FileChecker.FileState.HighestGenerationRequired;
			lastGenerationBackedUp = base.FileChecker.FileState.LastGenerationBackedUp;
			return logfileSignature != null;
		}

		internal override AmAcllReturnStatus AttemptCopyLastLogsRcr(AmAcllArgs acllArgs, AcllPerformanceTracker acllPerf)
		{
			bool mountPending = acllArgs.MountPending;
			DatabaseMountDialOverride mountDialOverride = acllArgs.MountDialOverride;
			string uniqueOperationId = acllArgs.UniqueOperationId;
			int subactionAttemptNumber = acllArgs.SubactionAttemptNumber;
			bool fSkipHealthChecks = (acllArgs.SkipValidationChecks & AmBcsSkipFlags.SkipHealthChecks) != AmBcsSkipFlags.None;
			string displayName = base.Configuration.DisplayName;
			AmAcllReturnStatus result;
			try
			{
				base.CurrentContext.AttemptCopyLastLogsEnter();
				StateLock suspendLock = base.Configuration.ReplayState.SuspendLock;
				acllPerf.RunTimedOperation(AcllTimedOperation.AcquireSuspendLock, delegate
				{
					this.AcquireSuspendLockForAcll(suspendLock, fSkipHealthChecks);
				});
				acllPerf.IsSkipHealthChecks = fSkipHealthChecks;
				base.CurrentContext.ClearAttemptCopyLastLogsEndTime();
				result = this.AttemptCopyLastLogsInternal(mountPending, fSkipHealthChecks, mountDialOverride, uniqueOperationId, subactionAttemptNumber, suspendLock, acllPerf);
			}
			finally
			{
				base.CurrentContext.AttemptCopyLastLogsLeave();
			}
			return result;
		}

		protected override bool ConfigurationCheckerInternal()
		{
			this.TraceDebug("ConfigurationCheckerInternal()");
			base.StopMonitoredDatabase();
			if (base.CurrentContext.Suspended)
			{
				this.TraceDebug("leaving ConfigurationCheckerInternal() due to instance suspended");
				return false;
			}
			base.CheckInstanceAbortRequested();
			ReplayState replayState = base.Configuration.ReplayState;
			base.CheckEdbLogDirectoryUnderMountPoint();
			base.CheckEdbLogDirectoriesIfNeeded();
			base.CheckInstanceAbortRequested();
			string directoryName = Path.GetDirectoryName(base.Configuration.DestinationEdbPath);
			string[] directoriesToCreate = new string[]
			{
				base.Configuration.DestinationLogPath,
				base.Configuration.DestinationSystemPath,
				directoryName,
				base.Configuration.LogInspectorPath,
				base.Configuration.E00LogBackupPath
			};
			base.CreateDirectories(directoriesToCreate);
			base.CheckInstanceAbortRequested();
			string[] array = new string[]
			{
				base.Configuration.DestinationLogPath,
				base.Configuration.DestinationSystemPath,
				base.Configuration.E00LogBackupPath,
				directoryName
			};
			List<string> list = new List<string>(array.Length + 2);
			list.AddRange(array);
			base.CheckDirectories(list);
			base.CheckInstanceAbortRequested();
			string destinationEdbPath = base.Configuration.DestinationEdbPath;
			base.CheckInstanceAbortRequested();
			this.m_initialNetworkPath = NetworkManager.ChooseNetworkPath(base.Configuration.SourceMachine, null, NetworkPath.ConnectionPurpose.LogCopy);
			base.CheckInstanceAbortRequested();
			base.ClearReplayState();
			base.CheckInstanceAbortRequested();
			this.CheckConnectionToStore();
			base.CheckInstanceAbortRequested();
			LastLogReplacer.RollbackLastLogIfNecessary(base.Configuration);
			LogRepair logRepair = null;
			try
			{
				ReplayState replayState2 = base.Configuration.ReplayState;
				if (replayState2.LogRepairMode != LogRepairMode.Off)
				{
					logRepair = new LogRepair(base.Configuration);
				}
				bool flag = base.FileChecker.RunChecks(logRepair);
				base.CheckInstanceAbortRequested();
				if (flag)
				{
					this.PreserveInspectorLogs(base.CurrentContext, false);
					base.SetReplayState(base.FileChecker.FileState);
					base.CheckInstanceAbortRequested();
					base.CleanUpTempIncReseedFiles();
					base.CheckInstanceAbortRequested();
					this.TraceDebug("state change from init->resync");
					ExTraceGlobals.FaultInjectionTracer.TraceTest(3932564797U);
					base.CurrentContext.UpdateInstanceProgress(ReplicaInstanceStage.Resynchronizing);
					if (base.CurrentContext.IsLogStreamStartGenerationResetPending)
					{
						this.TraceDebug(string.Format("Log stream automatic position reset requested. New start generation: 0x{0:X}", base.Configuration.ReplayState.LogStreamStartGeneration));
						base.Configuration.ReplayState.ResetForSeed();
						AgedOutDirectoryHelper.MoveLogFiles(base.Configuration, base.FileChecker.FileState, base.CurrentContext, base.Configuration.ReplayState.LogStreamStartGeneration - 1L);
						this.TraceDebug("Re-running filechecker after logs have been deleted so that we can get the right file state.");
						base.FileChecker.RunChecks(null, true);
					}
					else if (base.FileChecker.CheckRequiredLogFilesForDatabaseMountable())
					{
						this.TraceDebug("state viable is true");
						base.CurrentContext.SetViable();
					}
					else
					{
						this.TraceDebug("Initial check for viable returns false, LogReplayer will check the condition later");
					}
					base.CheckInstanceAbortRequested();
					this.TraceDebug("about to check for database divergence");
					this.IncrementalReseedIfNecessary();
					base.CheckInstanceAbortRequested();
					if (logRepair != null)
					{
						replayState2.LogRepairMode = LogRepairMode.ReplayPending;
					}
					base.CurrentContext.ClearBroken();
				}
				else
				{
					base.DeleteAllLogFiles(base.Configuration.LogInspectorPath, base.CurrentContext);
					base.CheckInstanceAbortRequested();
				}
			}
			finally
			{
				if (logRepair != null)
				{
					logRepair.Dispose();
				}
			}
			this.TraceDebug("Leaving ConfigurationCheckerInternal()");
			return true;
		}

		private void PreserveInspectorLogs(ISetBroken setBroken, bool fCalledFromAcll)
		{
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			Exception ex = null;
			LogVerifier logVerifier = null;
			try
			{
				if (base.FileChecker.FileState.HighestGenerationPresent > 0L && base.FileChecker.FileState.LogfileSignature != null)
				{
					num = base.FileChecker.FileState.HighestGenerationPresent + 1L;
					string logFileName = base.Configuration.BuildFullLogfileName(base.FileChecker.FileState.HighestGenerationPresent);
					JET_LOGINFOMISC jet_LOGINFOMISC;
					UnpublishedApi.JetGetLogFileInfo(logFileName, out jet_LOGINFOMISC, JET_LogInfo.Misc2);
					DateTime dateTime = jet_LOGINFOMISC.logtimeCreate.ToDateTime() ?? DateTime.MinValue;
					string text;
					EsentErrorException ex2;
					for (;;)
					{
						ExTraceGlobals.FaultInjectionTracer.TraceTest(2292591933U);
						if (!fCalledFromAcll)
						{
							base.CheckInstanceAbortRequested();
						}
						string path = base.Configuration.BuildShortLogfileName(num);
						text = Path.Combine(base.Configuration.LogInspectorPath, path);
						if (!File.Exists(text))
						{
							break;
						}
						if (logVerifier == null)
						{
							logVerifier = new LogVerifier(base.Configuration.LogFilePrefix);
						}
						ex2 = logVerifier.Verify(text);
						if (ex2 != null)
						{
							goto Block_9;
						}
						UnpublishedApi.JetGetLogFileInfo(text, out jet_LOGINFOMISC, JET_LogInfo.Misc2);
						if ((long)jet_LOGINFOMISC.ulGeneration != num)
						{
							goto Block_10;
						}
						if (!jet_LOGINFOMISC.signLog.Equals(base.FileChecker.FileState.LogfileSignature))
						{
							goto Block_11;
						}
						if (jet_LOGINFOMISC.logtimePreviousGeneration.ToDateTime() != dateTime)
						{
							goto Block_13;
						}
						string destFileName = base.Configuration.BuildFullLogfileName(num);
						File.Move(text, destFileName);
						base.FileChecker.FileState.SetLowestAndHighestGenerationsPresent(base.FileChecker.FileState.LowestGenerationPresent, num);
						base.CurrentContext.ReportOneLogCopied();
						dateTime = (jet_LOGINFOMISC.logtimeCreate.ToDateTime() ?? DateTime.MinValue);
						if (num2 == 0L)
						{
							num2 = num;
						}
						num3 = num;
						num += 1L;
					}
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, long>((long)this.GetHashCode(), "PreserveInspectorLogs({0}): found end at 0x{1:X}", base.Configuration.Name, num);
					goto IL_2DD;
					Block_9:
					throw new LogInspectorFailedException(ex2.Message, ex2);
					Block_10:
					throw new FileCheckLogfileGenerationException(text, (long)jet_LOGINFOMISC.ulGeneration, num);
					Block_11:
					throw new FileCheckLogfileSignatureException(text, jet_LOGINFOMISC.signLog.ToString(), base.FileChecker.FileState.LogfileSignature.ToString());
					Block_13:
					throw new FileCheckLogfileCreationTimeException(text, jet_LOGINFOMISC.logtimePreviousGeneration.ToDateTime() ?? DateTime.MinValue, dateTime);
				}
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "PreserveInspectorLogs({0}): unusual startup condition prevents preservation", base.Configuration.Name);
				IL_2DD:;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (EsentErrorException ex4)
			{
				ex = ex4;
			}
			catch (FileCheckException ex5)
			{
				ex = ex5;
			}
			catch (LogInspectorFailedException ex6)
			{
				ex = ex6;
			}
			finally
			{
				if (logVerifier != null)
				{
					logVerifier.Term();
				}
				if (ex != null)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, long, Exception>((long)this.GetHashCode(), "PreserveInspectorLogs({0}): halted at 0x{1:X}: {2}", base.Configuration.Name, num, ex);
					ReplayCrimsonEvents.LogPreservationFailedDuringStartup.Log<string, string, string, string>(base.Configuration.DatabaseName, Environment.MachineName, string.Format("0x{0:x}", num), ex.Message);
				}
				if (num2 > 0L)
				{
					ReplayCrimsonEvents.LogsPreservedDuringStartup.Log<string, string, string, string>(base.Configuration.DatabaseName, Environment.MachineName, string.Format("0x{0:x}", num2), string.Format("0x{0:x}", num3));
				}
				base.DeleteAllLogFiles(base.Configuration.LogInspectorPath, setBroken);
			}
		}

		protected override void TraceDebug(string message)
		{
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "TargetReplicaInstance {0}: {1}", base.Configuration.Name, message);
		}

		protected override void TraceError(string message)
		{
			ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, string>((long)this.GetHashCode(), "TargetReplicaInstance {0}: {1}", base.Configuration.Name, message);
		}

		protected override void StartComponents()
		{
			this.TraceDebug("Starting Components");
			ExTraceGlobals.PFDTracer.TracePfd<int, string>((long)this.GetHashCode(), "PFD CRS {0} Starting: {1}", 27293, base.Configuration.Name);
			ReplayEventLogConstants.Tuple_TargetInstanceStart.LogEvent(null, new object[]
			{
				base.Configuration.ServerName,
				base.Configuration.SourceMachine,
				base.Configuration.Type,
				base.Configuration.Name
			});
			string text = "." + base.Configuration.LogExtension;
			long fromNumber = ReplicaInstance.CopyGeneration(base.Configuration, base.FileChecker.FileState, base.CurrentContext, this.PrepareToStopCalledEvent);
			long highestGenerationRequired = base.FileChecker.FileState.HighestGenerationRequired;
			base.CheckInstanceAbortRequested();
			this.m_logCopier = new LogCopier(base.PerfmonCounters, base.Configuration.LogFilePrefix, fromNumber, text, base.Configuration.LogInspectorPath, base.Configuration.DestinationLogPath, base.Configuration, base.FileChecker.FileState, base.CurrentContext, base.CurrentContext, base.CurrentContext, this.m_initialNetworkPath, false);
			base.CheckInstanceAbortRequested();
			this.m_logTruncater = new LogTruncater(base.PerfmonCounters, base.FileChecker, base.CurrentContext, base.Configuration, base.Configuration, base.CurrentContext, this.PrepareToStopCalledEvent);
			base.CheckInstanceAbortRequested();
			this.m_logInspector = new LogInspector(base.PerfmonCounters, base.Configuration, base.Configuration.LogFilePrefix, text, base.Configuration.DestinationLogPath, base.FileChecker.FileState, this.m_logTruncater, base.CurrentContext, base.CurrentContext, base.CurrentContext, this.m_initialNetworkPath);
			base.CheckInstanceAbortRequested();
			this.m_logReplayer = new LogReplayer(base.PerfmonCounters, base.Configuration.LogFilePrefix, highestGenerationRequired, text, base.FileChecker, this.m_logInspector, this.m_logTruncater, base.CurrentContext, base.CurrentContext, base.Configuration, base.CurrentContext, base.CurrentContext);
			base.CheckInstanceAbortRequested();
			this.m_perfCounterUpdater = new PerfCounterUpdater(base.PerfmonCounters, base.Configuration);
			base.CheckInstanceAbortRequested();
			base.StartComponent(this.m_logCopier);
			base.StartComponent(this.m_logInspector);
			base.StartComponent(this.m_logReplayer);
			base.StartComponent(this.m_logTruncater);
			base.StartComponent(this.m_perfCounterUpdater);
			base.CheckInstanceAbortRequested();
			RemoteDataProvider.StartMonitoredDatabase(base.Configuration);
			this.TraceDebug("All components started");
			ExTraceGlobals.PFDTracer.TracePfd<int, ReplayConfigType, string>((long)this.GetHashCode(), "PFD CRS {0} TargetReplicaInstance started {1} {2}", 19101, base.Configuration.Type, base.Configuration.Name);
		}

		protected override void PrepareToStopInternal()
		{
			this.TraceDebug("TRI.PrepareToStopInternal()");
			if (base.IsThirdPartyReplicationEnabled)
			{
				return;
			}
			base.StopMonitoredDatabase();
			if (this.m_ir != null)
			{
				this.m_ir.PrepareToStop();
			}
		}

		protected override void StopInternal()
		{
			this.TraceDebug("TRI.StopInternal()");
			if (base.IsThirdPartyReplicationEnabled)
			{
				return;
			}
			if (base.StartedComponents)
			{
				ReplayEventLogConstants.Tuple_TargetInstanceStop.LogEvent(null, new object[]
				{
					base.Configuration.ServerName,
					base.Configuration.SourceMachine,
					base.Configuration.Type,
					base.Configuration.Name
				});
			}
			ExTraceGlobals.PFDTracer.TracePfd<int, string>((long)this.GetHashCode(), "PFD CRS {0} ReplicaInstance stopped: {1}", 30365, base.Configuration.Name);
		}

		private void InitializeCurrentContext()
		{
			ReplayState replayState = base.Configuration.ReplayState;
			if (!base.IsBroken)
			{
				if (replayState.Suspended && replayState.ConfigBroken)
				{
					base.CurrentContext.SetFailedAndSuspended((uint)replayState.ConfigBrokenEventId, new LocalizedString(replayState.ConfigBrokenMessage), replayState.ConfigBrokenExtendedErrorInfo);
				}
				else if (replayState.Suspended)
				{
					base.CurrentContext.SetSuspended();
				}
				else if (base.PreviousContext != null && base.PreviousContext.FailureInfo.IsFailed && this.PerformanceCounters != null)
				{
					this.PerformanceCounters.Failed = 1L;
					this.PerformanceCounters.Disconnected = 0L;
				}
			}
			base.CurrentContext.UpdateInstanceProgress(ReplicaInstanceStage.Initializing);
		}

		private void CheckConnectionToStore()
		{
			LocalizedException ex = null;
			using (IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(null))
			{
				if (!newStoreControllerInstance.TestStoreConnectivity(TimeSpan.FromSeconds((double)RegistryParameters.TestStoreConnectivityTimeoutInSec), out ex))
				{
					base.CurrentContext.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_CheckConnectionToStoreFailed, ex, new string[]
					{
						ex.Message
					});
				}
			}
		}

		private void IncrementalReseedIfNecessary()
		{
			long highestLogGenCompared = -1L;
			bool isIncReseedNeeded = false;
			bool e00IsEndOfLogStream = false;
			IncReseedPerformanceTracker incReseedPerformanceTracker = new IncReseedPerformanceTracker(base.Configuration);
			this.m_ir = new IncrementalReseeder(this.PerformanceCounters, incReseedPerformanceTracker, base.Configuration, base.FileChecker.FileState, base.CurrentContext, base.CurrentContext, base.CurrentContext, this.m_initialNetworkPath, false, this.PrepareToStopCalledEvent);
			try
			{
				incReseedPerformanceTracker.RunTimedOperation(IncReseedOperation.IsIncrementalReseedRequiredOverall, delegate
				{
					isIncReseedNeeded = this.m_ir.IsIncrementalReseedRequired(new Action(this.CheckInstanceAbortRequested), out highestLogGenCompared, out e00IsEndOfLogStream);
				});
				if (isIncReseedNeeded)
				{
					ReplayEventLogConstants.Tuple_ReplicaInstanceStartIncrementalReseed.LogEvent(null, new object[]
					{
						base.Configuration.Name,
						MachineName.GetNodeNameFromFqdn(base.Configuration.SourceMachine),
						Environment.MachineName
					});
					base.CheckInstanceAbortRequested();
					incReseedPerformanceTracker.RunTimedOperation(IncReseedOperation.PerformIncrementalReseedOverall, delegate
					{
						this.m_ir.PerformIncrementalReseed(highestLogGenCompared);
					});
				}
			}
			finally
			{
				incReseedPerformanceTracker.IsIncReseedNeeded = isIncReseedNeeded;
				incReseedPerformanceTracker.LogEvent();
			}
		}

		private bool IsInstanceAcllReady()
		{
			return base.CurrentContext.Running;
		}

		private void AcquireSuspendLockForAcll(StateLock suspendLock, bool fSkipHealthChecks)
		{
			bool flag = false;
			bool flag2 = false;
			string name = base.Configuration.DisplayName;
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: Entering ACLL lock w/o wait: TryEnterAcll(false)...", name);
			LogReplayer logReplayer = null;
			ActionToRunBeforeWaitingForLock actionBeforeWaitForLock = null;
			logReplayer = base.GetComponent<LogReplayer>();
			if (logReplayer != null)
			{
				logReplayer.IsDismountControlledExternally = true;
				actionBeforeWaitForLock = delegate()
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "RI.AcquireSuspendLockForAcll {0}: Signalling replayer thread so that ACLL can acquire the lock quickly.", name);
					logReplayer.WakeupReplayer();
				};
			}
			LockOwner lockOwner;
			if (!suspendLock.TryEnterAcll(false, null, out lockOwner))
			{
				flag2 = true;
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: ACLL lock could not be acquired because '{1}' currently owns it!", name, lockOwner);
				if (lockOwner == LockOwner.Suspend)
				{
					if (!fSkipHealthChecks)
					{
						throw new AcllCopyStatusInvalidException(name, ReplayStrings.Suspended);
					}
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: SkipHealthChecks was used so ACLL will now give up trying to acquire the StateLock, since 'Suspend' currently owns it.", name);
					flag2 = false;
				}
				else
				{
					if (lockOwner == LockOwner.AttemptCopyLastLogs)
					{
						throw new AcllAlreadyRunningException(name);
					}
					if (!this.IsInstanceAcllReady())
					{
						flag = true;
						ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: RI is not in an ACLL-ready state. RI will be restarted...", name);
					}
				}
			}
			else
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: ACLL got the lock with 'fWait==false'", name);
			}
			if (base.AcllAutoLockReleaseInstance != null)
			{
				base.AcllAutoLockReleaseInstance.Stop();
				base.AcllAutoLockReleaseInstance = null;
			}
			if (flag2)
			{
				if (flag)
				{
					base.CurrentContext.RestartInstanceSoon(true);
				}
				else
				{
					base.CurrentContext.RestartInstanceSoon(false);
				}
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: Entering ACLL lock again, but with wait: TryEnterAcll(true)...", name);
				if (!suspendLock.TryEnterAcll(true, new TimeSpan?(TimeSpan.FromSeconds((double)RegistryParameters.AcllSuspendLockTimeoutInSec)), out lockOwner, actionBeforeWaitForLock))
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: ACLL lock could not be acquired because '{1}' currently owns it!", name, lockOwner);
					if (lockOwner == LockOwner.Suspend)
					{
						throw new AcllCopyStatusInvalidException(name, ReplayStrings.Suspended);
					}
					if (lockOwner == LockOwner.AttemptCopyLastLogs)
					{
						throw new AcllAlreadyRunningException(name);
					}
					if (lockOwner == LockOwner.Backup)
					{
						throw new AcllBackupInProgressException(name);
					}
					if (lockOwner == LockOwner.Component)
					{
						AcllCouldNotControlReplicaInstanceException ex = new AcllCouldNotControlReplicaInstanceException(name);
						throw ex;
					}
					DiagCore.RetailAssert(false, "ACLL lock could not be acquired because '{0}' currently owns it!", new object[]
					{
						lockOwner
					});
				}
				else
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: ACLL got the lock with 'fWait==true'", name);
				}
			}
			base.AcllAutoLockReleaseInstance = new AcllAutoLockRelease(this);
			base.AcllAutoLockReleaseInstance.Start();
		}

		private AmAcllReturnStatus AttemptCopyLastLogsInternal(bool mountPending, bool fSkipHealthChecks, DatabaseMountDialOverride mountDialOverride, string uniqueOperationId, int subactionAttemptNumber, StateLock suspendLock, AcllPerformanceTracker acllPerf)
		{
			AmAcllReturnStatus amAcllReturnStatus = new AmAcllReturnStatus();
			bool flag = false;
			string displayName = base.Configuration.DisplayName;
			if (base.Configuration.ReplayState.ResumeBlocked)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: ReplicaInstance is marked as ResumeBlocked! Failing out of ACLL.", displayName);
				throw new AcllCopyStatusResumeBlockedException(displayName, AmExceptionHelper.GetMessageOrNoneString(base.CurrentContext.ErrorMessage));
			}
			if (!fSkipHealthChecks && base.CurrentContext.IsBroken)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, CopyStatusEnum>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: ReplicaInstance is already Broken. Failing out of ACLL. CopyStatus='{1}'", displayName, base.CurrentContext.GetStatus());
				CopyStatusEnum status = base.CurrentContext.GetStatus();
				throw new AcllCopyStatusFailedException(displayName, (status == CopyStatusEnum.FailedAndSuspended) ? ReplayStrings.FailedAndSuspended : ReplayStrings.Failed, base.CurrentContext.ErrorMessage);
			}
			if (base.PrepareToStopCalled)
			{
				flag = true;
				acllPerf.IsPrepareToStopCalled = true;
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: ReplicaInstance PrepareToStopCalled='true', IsBroken='{1}', IsDisconnected='{2}', CopyStatus='{3}', Viable='{4}'. Constructing new LogCopier and LogInspector!", new object[]
				{
					displayName,
					base.CurrentContext.IsBroken,
					base.CurrentContext.IsDisconnected,
					base.CurrentContext.GetStatus(),
					base.CurrentContext.Viable
				});
			}
			else if (!this.IsInstanceAcllReady())
			{
				flag = true;
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: ReplicaInstance IsBroken='{1}', IsDisconnected='{2}', CopyStatus='{3}', Viable='{4}'. Constructing new LogCopier and LogInspector due to RI not being in Resynchronizing or Running stage!", new object[]
				{
					displayName,
					base.CurrentContext.IsBroken,
					base.CurrentContext.IsDisconnected,
					base.CurrentContext.GetStatus(),
					base.CurrentContext.Viable
				});
			}
			else if (suspendLock.CurrentOwner != LockOwner.AttemptCopyLastLogs)
			{
				flag = true;
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: ReplicaInstance StateLock is held by '{4}'. IsBroken='{1}', IsDisconnected='{2}', CopyStatus='{3}'. Constructing new LogCopier and LogInspector!", new object[]
				{
					displayName,
					base.CurrentContext.IsBroken,
					base.CurrentContext.IsDisconnected,
					base.CurrentContext.GetStatus(),
					suspendLock.CurrentOwner
				});
			}
			else if (!base.CurrentContext.Viable)
			{
				flag = true;
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: ReplicaInstance IsBroken='{1}', IsDisconnected='{2}', CopyStatus='{3}', Viable='{4}'. Constructing new LogCopier and LogInspector due to RI not being Viable!", new object[]
				{
					displayName,
					base.CurrentContext.IsBroken,
					base.CurrentContext.IsDisconnected,
					base.CurrentContext.GetStatus(),
					base.CurrentContext.Viable
				});
			}
			acllPerf.IsNewCopierInspectorCreated = flag;
			acllPerf.ReplicaInstanceStage = base.CurrentContext.ProgressStage;
			acllPerf.CopyStatus = base.CurrentContext.GetStatus();
			FileChecker fileChecker = null;
			LogCopier logCopier = null;
			LogInspector logInspector = null;
			LogReplayer logReplayer = null;
			try
			{
				if (!flag)
				{
					logCopier = base.GetComponent<LogCopier>();
					logInspector = base.GetComponent<LogInspector>();
					logReplayer = base.GetComponent<LogReplayer>();
					fileChecker = base.FileChecker;
					FileState fileState = base.FileChecker.FileState;
				}
				else
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: Running FileChecker due to new Copier/Inspector constructed, or due to RI not in Resynchronizing or Running stages.", displayName);
					acllPerf.RunTimedOperation(AcllTimedOperation.RollbackLastLogIfNecessary, delegate
					{
						LastLogReplacer.RollbackLastLogIfNecessary(base.Configuration);
					});
					fileChecker = new FileChecker(base.Configuration.Name, base.Configuration.DestinationLogPath, base.Configuration.DestinationSystemPath, base.Configuration.LogFilePrefix, "." + base.Configuration.LogExtension, base.Configuration.DestinationEdbPath, base.Configuration.IdentityGuid);
					acllPerf.RunTimedOperation(AcllTimedOperation.FileCheckerAtStart, delegate
					{
						fileChecker.RunChecks();
					});
					base.FileChecker = fileChecker;
					FileState fileState = fileChecker.FileState;
					SimpleSetBroken simpleSetBroken = new SimpleSetBroken(displayName);
					this.PreserveInspectorLogs(simpleSetBroken, true);
					if (simpleSetBroken.IsBroken)
					{
						throw new PreserveInspectorLogsException(simpleSetBroken.ErrorMessage);
					}
					base.ClearReplayState();
					base.SetReplayState(fileState);
					acllPerf.RunTimedOperation(AcllTimedOperation.CleanUpTempIncReseedFiles, delegate
					{
						base.CleanUpTempIncReseedFiles();
					});
				}
				amAcllReturnStatus = AttemptCopyLastLogs.AttemptCopyLastLogsOnceRcr(base.PerfmonCounters, base.Configuration, fileChecker, base.CurrentContext, base.CurrentContext, logCopier, logInspector, logReplayer, flag, fSkipHealthChecks, mountDialOverride, acllPerf, uniqueOperationId, subactionAttemptNumber, mountPending);
				if (amAcllReturnStatus.MountAllowed)
				{
					base.Configuration.ReplayState.LastAcllLossAmount = amAcllReturnStatus.NumberOfLogsLost;
					base.Configuration.ReplayState.LastAcllRunWithSkipHealthChecks = fSkipHealthChecks;
				}
			}
			catch (LastLogReplacementException ex)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, LastLogReplacementException>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: An exception occurred: {1}", displayName, ex);
				amAcllReturnStatus.LastError = ex.Message;
			}
			catch (FileCheckException ex2)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, FileCheckException>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: An exception occurred: {1}", displayName, ex2);
				amAcllReturnStatus.LastError = ex2.Message;
			}
			catch (IncrementalReseedFailedException ex3)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, IncrementalReseedFailedException>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: An exception occurred: {1}", displayName, ex3);
				amAcllReturnStatus.LastError = ex3.Message;
			}
			catch (IncrementalReseedRetryableException ex4)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, IncrementalReseedRetryableException>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: An exception occurred: {1}", displayName, ex4);
				amAcllReturnStatus.LastError = ex4.Message;
			}
			catch (IOException ex5)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, IOException>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: An exception occurred: {1}", displayName, ex5);
				amAcllReturnStatus.LastError = ex5.Message;
			}
			catch (UnauthorizedAccessException ex6)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, UnauthorizedAccessException>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: An exception occurred: {1}", displayName, ex6);
				amAcllReturnStatus.LastError = ex6.Message;
			}
			catch (PreserveInspectorLogsException ex7)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, PreserveInspectorLogsException>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: An exception occurred: {1}", displayName, ex7);
				amAcllReturnStatus.LastError = ex7.Message;
			}
			catch (LogInspectorFailedException ex8)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, LogInspectorFailedException>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: An exception occurred: {1}", displayName, ex8);
				amAcllReturnStatus.LastError = ex8.Message;
			}
			finally
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: Restarting instance (fPrepareToStopCalled = 'true') since ACLL is done.", displayName);
				base.CurrentContext.RestartInstanceSoon(true);
				base.CurrentContext.SetAttemptCopyLastLogsEndTime();
				if (!mountPending || !amAcllReturnStatus.MountAllowed)
				{
					lock (this)
					{
						ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "RI.AttemptCopyLastLogs {0}: Leaving ACLL lock...", displayName);
						suspendLock.LeaveAttemptCopyLastLogs();
						base.CurrentContext.ClearAttemptCopyLastLogsEndTime();
					}
					base.CurrentContext.BestEffortDismountReplayDatabase();
				}
				if (logReplayer != null && logReplayer.IsDismountControlledExternally && logReplayer.IsDismountPending)
				{
					base.CurrentContext.IsReplayDatabaseDismountPending = true;
				}
			}
			return amAcllReturnStatus;
		}

		private NetworkPath m_initialNetworkPath;

		private LogCopier m_logCopier;

		private LogInspector m_logInspector;

		private LogReplayer m_logReplayer;

		private LogTruncater m_logTruncater;

		private PerfCounterUpdater m_perfCounterUpdater;

		private IncrementalReseeder m_ir;
	}
}
