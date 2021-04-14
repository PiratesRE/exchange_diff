using System;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay.MountPoint;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.HA.FailureItem;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceContext : IReplicaInstanceContext, ISetBroken, ISetDisconnected, ISetViable, ISetSeeding, ISetPassiveSeeding, IReplicaProgress, ISetGeneration, ISetActiveSeeding, IGetStatus, ISetVolumeInfo
	{
		public ReplicaInstanceContext()
		{
			this.FailureInfo = new FailureInfo();
			this.m_PassiveSeedingSourceContext = PassiveSeedingSourceContextEnum.None;
		}

		public string Identity
		{
			get
			{
				return this.m_identity;
			}
		}

		internal FailureInfo FailureInfo { get; private set; }

		public DatabaseVolumeInfo DatabaseVolumeInfo { get; private set; }

		public bool RestartSoon
		{
			get
			{
				bool fRestartSoon;
				lock (this.m_instance)
				{
					fRestartSoon = this.m_fRestartSoon;
				}
				return fRestartSoon;
			}
			set
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "{0}: RestartSoon is being set to: {1}", this.m_displayName, value);
				this.LogCrimsonEventOnStateChange<bool>("RestartSoon", this.m_fRestartSoon, value);
				this.m_fRestartSoon = value;
			}
		}

		public bool DoNotRestart
		{
			get
			{
				bool fDoNotRestart;
				lock (this.m_instance)
				{
					fDoNotRestart = this.m_fDoNotRestart;
				}
				return fDoNotRestart;
			}
			set
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "{0}: DoNotRestart is being set to: {1}", this.m_displayName, value);
				this.LogCrimsonEventOnStateChange<bool>("DoNotRestart", this.m_fDoNotRestart, value);
				this.m_fDoNotRestart = value;
			}
		}

		public bool Suspended
		{
			get
			{
				bool fSuspended;
				lock (this.m_instance)
				{
					fSuspended = this.m_fSuspended;
				}
				return fSuspended;
			}
			private set
			{
				this.m_fSuspended = value;
			}
		}

		public bool Seeding
		{
			get
			{
				bool fSeeding;
				lock (this.m_instance)
				{
					fSeeding = this.m_fSeeding;
				}
				return fSeeding;
			}
			private set
			{
				if (this.m_perfmonCounters != null)
				{
					if (value)
					{
						this.m_perfmonCounters.Seeding = 1L;
					}
					else
					{
						this.m_perfmonCounters.Seeding = 0L;
					}
				}
				this.m_fSeeding = value;
			}
		}

		public PassiveSeedingSourceContextEnum PassiveSeedingSourceContext
		{
			get
			{
				PassiveSeedingSourceContextEnum passiveSeedingSourceContext;
				lock (this.m_instance)
				{
					passiveSeedingSourceContext = this.m_PassiveSeedingSourceContext;
				}
				return passiveSeedingSourceContext;
			}
			private set
			{
				if (this.m_perfmonCounters != null)
				{
					if (value != PassiveSeedingSourceContextEnum.None)
					{
						this.m_perfmonCounters.PassiveSeedingSource = 1L;
					}
					else
					{
						this.m_perfmonCounters.PassiveSeedingSource = 0L;
					}
				}
				this.m_PassiveSeedingSourceContext = value;
			}
		}

		public bool ActiveSeedingSource
		{
			get
			{
				bool fActiveSeedingSource;
				lock (this.m_instance)
				{
					fActiveSeedingSource = this.m_fActiveSeedingSource;
				}
				return fActiveSeedingSource;
			}
			private set
			{
				this.m_fActiveSeedingSource = value;
			}
		}

		public bool IsSeedingSourceForDB { get; private set; }

		public bool IsSeedingSourceForCI { get; private set; }

		public bool Viable
		{
			get
			{
				bool fViable;
				lock (this.m_instance)
				{
					fViable = this.m_fViable;
				}
				return fViable;
			}
			private set
			{
				this.m_fViable = value;
			}
		}

		public bool AdminVisibleRestart
		{
			get
			{
				bool fAdminVisibleRestart;
				lock (this.m_instance)
				{
					fAdminVisibleRestart = this.m_fAdminVisibleRestart;
				}
				return fAdminVisibleRestart;
			}
			private set
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "{0}: AdminVisibleRestart is being set to: {1}", this.m_displayName, value);
				this.LogCrimsonEventOnStateChange<bool>("AdminVisibleRestart", this.m_fAdminVisibleRestart, value);
				this.m_fAdminVisibleRestart = value;
			}
		}

		public bool InAttemptCopyLastLogs
		{
			get
			{
				return this.m_numThreadsInAcll > 0;
			}
		}

		public long LogsCopiedSinceInstanceStart
		{
			get
			{
				return this.m_countLogsCopied;
			}
		}

		public long LogsReplayedSinceInstanceStart
		{
			get
			{
				return this.m_countLogsReplayed;
			}
		}

		public ReplicaInstance ReplicaInstance
		{
			get
			{
				return this.m_instance;
			}
		}

		public bool IsReplayDatabaseDismountPending { get; set; }

		public void InitializeContext(ReplicaInstance instance)
		{
			this.m_instance = instance;
			this.m_replayState = instance.Configuration.ReplayState;
			this.m_identity = instance.Configuration.Identity;
			this.m_databaseName = instance.Configuration.DatabaseName;
			this.m_displayName = instance.Configuration.DisplayName;
			this.m_guid = instance.Configuration.IdentityGuid;
			this.m_perfmonCounters = instance.PerfmonCounters;
			this.ExternalStatus = new ExternalReplicaInstanceStatus(this, this.m_instance.PreviousContext, this.m_instance.Configuration.Type, this.m_perfmonCounters, this.m_replayState);
			if (instance.IsThirdPartyReplicationEnabled && instance.IsTarget)
			{
				this.m_progressStage = ReplicaInstanceStage.Running;
			}
			SeederServerContext seederServerContext = SourceSeedTable.Instance.TryGetContext(this.m_guid);
			if (seederServerContext != null)
			{
				if (instance.Configuration.IsPassiveCopy)
				{
					seederServerContext.LinkWithNewPassiveRIStatus(this);
				}
				else
				{
					seederServerContext.LinkWithNewActiveRIStatus(this);
				}
			}
			this.m_countLogsThreshold = (long)RegistryParameters.ReplicaProgressNumberOfLogsThreshold;
		}

		public void CarryOverPreviousStatus(ReplicaInstanceContextMinimal previousContext)
		{
			if (previousContext != null)
			{
				this.ExternalStatus.CarryOverPreviousStatus(previousContext.LastCopyStatus);
			}
		}

		public string DisplayName
		{
			get
			{
				return this.m_displayName;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.m_databaseName;
			}
		}

		public bool Initializing
		{
			get
			{
				return this.ProgressStage == ReplicaInstanceStage.Initializing;
			}
			private set
			{
				if (value)
				{
					ExTraceGlobals.StateTracer.TraceDebug<string>((long)this.GetHashCode(), "TargetState is changing to Initializing on replica {0}", this.m_identity);
					this.ProgressStage = ReplicaInstanceStage.Initializing;
					if (!this.m_replayState.ConfigBroken && this.m_perfmonCounters != null)
					{
						this.m_perfmonCounters.Initializing = 1L;
						this.m_perfmonCounters.Resynchronizing = 0L;
					}
				}
			}
		}

		public bool Resynchronizing
		{
			get
			{
				return this.ProgressStage == ReplicaInstanceStage.Resynchronizing;
			}
			private set
			{
				if (value)
				{
					ExTraceGlobals.StateTracer.TraceDebug<string>((long)this.GetHashCode(), "TargetState is changing to Resynchronizing on replica {0}", this.m_identity);
					this.ProgressStage = ReplicaInstanceStage.Resynchronizing;
					if (!this.m_replayState.ConfigBroken && this.m_perfmonCounters != null)
					{
						this.m_perfmonCounters.Initializing = 0L;
						this.m_perfmonCounters.Resynchronizing = 1L;
					}
				}
			}
		}

		public bool Running
		{
			get
			{
				return this.ProgressStage == ReplicaInstanceStage.Running;
			}
			private set
			{
				if (value)
				{
					ExTraceGlobals.StateTracer.TraceDebug<string>((long)this.GetHashCode(), "TargetState is changing to Running on replica {0}", this.m_identity);
					this.ProgressStage = ReplicaInstanceStage.Running;
					if (this.m_perfmonCounters != null)
					{
						this.m_perfmonCounters.Initializing = 0L;
						this.m_perfmonCounters.Resynchronizing = 0L;
					}
				}
			}
		}

		public LocalizedString ErrorMessage
		{
			get
			{
				LocalizedString errorMessage;
				lock (this.m_instance)
				{
					errorMessage = this.FailureInfo.ErrorMessage;
				}
				return errorMessage;
			}
		}

		public uint ErrorEventId
		{
			get
			{
				uint errorEventId;
				lock (this.m_instance)
				{
					errorEventId = this.FailureInfo.ErrorEventId;
				}
				return errorEventId;
			}
		}

		public ExtendedErrorInfo ExtendedErrorInfo
		{
			get
			{
				ExtendedErrorInfo extendedErrorInfo;
				lock (this.m_instance)
				{
					extendedErrorInfo = this.FailureInfo.ExtendedErrorInfo;
				}
				return extendedErrorInfo;
			}
		}

		public bool IsDisconnected
		{
			get
			{
				bool isDisconnected;
				lock (this.m_instance)
				{
					isDisconnected = this.FailureInfo.IsDisconnected;
				}
				return isDisconnected;
			}
		}

		public bool IsBroken
		{
			get
			{
				bool isFailed;
				lock (this.m_instance)
				{
					isFailed = this.FailureInfo.IsFailed;
				}
				return isFailed;
			}
		}

		public CopyStatusEnum GetStatus()
		{
			CopyStatusEnum lastCopyStatus;
			lock (this.m_instance)
			{
				lastCopyStatus = this.ExternalStatus.LastCopyStatus;
			}
			return lastCopyStatus;
		}

		public void SetViable()
		{
			lock (this.m_instance)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0} Viable is now 'true'.", this.m_displayName);
				if (!this.m_replayState.ConfigBroken)
				{
					this.LogCrimsonEventOnStateChange<bool>("Viable", this.Viable, true);
				}
				this.Viable = true;
				this.ExternalStatus.Refresh();
			}
		}

		public void ClearViable()
		{
			lock (this.m_instance)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0} Viable is now 'false'.", this.m_displayName);
				if (!this.m_replayState.ConfigBroken)
				{
					this.LogCrimsonEventOnStateChange<bool>("Viable", this.Viable, false);
				}
				this.Viable = false;
				this.ExternalStatus.Refresh();
			}
		}

		public void SetCopyGeneration(long generation, DateTime writeTime)
		{
			if (generation > this.m_replayState.CopyGenerationNumber)
			{
				this.m_replayState.CopyGenerationNumber = generation;
			}
			if (writeTime > this.m_replayState.LatestCopyTime)
			{
				this.m_replayState.LatestCopyTime = writeTime;
			}
		}

		public void SetInspectGeneration(long generation, DateTime writeTime)
		{
			if (generation > this.m_replayState.InspectorGenerationNumber)
			{
				this.m_replayState.InspectorGenerationNumber = generation;
			}
			if (generation == 0L || writeTime > this.m_replayState.LatestInspectorTime)
			{
				this.m_replayState.LatestInspectorTime = writeTime;
			}
		}

		public void SetCopyNotificationGeneration(long generation, DateTime writeTime)
		{
			if (generation > this.m_replayState.CopyNotificationGenerationNumber)
			{
				this.m_replayState.CopyNotificationGenerationNumber = generation;
			}
			if (writeTime > this.m_replayState.LatestCopyNotificationTime)
			{
				this.m_replayState.LatestCopyNotificationTime = writeTime;
			}
		}

		public void SetLogStreamStartGeneration(long generation)
		{
			lock (this.m_instance)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, long>((long)this.GetHashCode(), "{0} SetLogStreamStartGeneration( 0x{1:X} ) called.", this.m_displayName, generation);
				this.LogCrimsonEventOnStateChange<long>("LogStreamStartGeneration", this.m_replayState.LogStreamStartGeneration, generation);
				this.m_replayState.LogStreamStartGeneration = generation;
			}
		}

		public void ClearLogStreamStartGeneration()
		{
			lock (this.m_instance)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0} ClearLogStreamStartGeneration() called.", this.m_displayName);
				this.LogCrimsonEventOnStateChange<long>("LogStreamStartGeneration", this.m_replayState.LogStreamStartGeneration, 0L);
				this.m_replayState.LogStreamStartGeneration = 0L;
			}
		}

		public bool IsLogStreamStartGenerationResetPending
		{
			get
			{
				bool result;
				lock (this.m_instance)
				{
					result = (this.m_replayState.LogStreamStartGeneration > 0L);
				}
				return result;
			}
		}

		public void AttemptCopyLastLogsEnter()
		{
			Interlocked.Increment(ref this.m_numThreadsInAcll);
		}

		public void AttemptCopyLastLogsLeave()
		{
			Interlocked.Decrement(ref this.m_numThreadsInAcll);
		}

		public void ClearAttemptCopyLastLogsEndTime()
		{
			this.m_replayState.LastAttemptCopyLastLogsEndTime = ReplayState.ZeroFileTime;
		}

		public void SetAttemptCopyLastLogsEndTime()
		{
			this.m_replayState.LastAttemptCopyLastLogsEndTime = DateTime.UtcNow;
		}

		public bool IsFailoverPending()
		{
			bool result;
			lock (this.m_instance)
			{
				result = (this.InAttemptCopyLastLogs || this.m_replayState.SuspendLockOwner == LockOwner.AttemptCopyLastLogs);
			}
			return result;
		}

		public bool ShouldNotRestartInstanceDueToAcll()
		{
			bool result;
			lock (this.m_instance)
			{
				bool flag2 = this.IsFailoverPending();
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "ShouldNotRestartInstanceDueToAcll() {0}: IsFailoverPending() returned {1}.", this.m_displayName, flag2);
				result = flag2;
			}
			return result;
		}

		public void SetDisconnected(FailureTag failureTag, ExEventLog.EventTuple setDisconnectedEventTuple, params string[] setDisconnectedArgs)
		{
			this.SetDisconnectedInternal(failureTag, setDisconnectedEventTuple, setDisconnectedArgs);
		}

		public void ClearDisconnected()
		{
			lock (this.m_instance)
			{
				this.ClearPreviousDisconnectedState();
				if (this.IsDisconnected)
				{
					this.LogCrimsonEventOnStateChange<bool>("Disconnected", this.IsDisconnected, false);
					this.FailureInfo.Reset();
					if (this.m_perfmonCounters != null)
					{
						this.m_perfmonCounters.Disconnected = 0L;
					}
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0} Disconnected state cleared.", this.m_displayName);
				}
				else
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "{0} ClearDisconnected() ignored because RI is not currently disconnected. FailureInfo.Failed = '{1}'.", this.m_displayName, this.FailureInfo.IsFailed);
				}
				this.ExternalStatus.Refresh();
			}
		}

		public void SetBroken(FailureTag failureTag, ExEventLog.EventTuple setBrokenEventTuple, params string[] setBrokenArgs)
		{
			this.SetBroken(failureTag, setBrokenEventTuple, null, setBrokenArgs);
		}

		public void SetBroken(FailureTag failureTag, ExEventLog.EventTuple setBrokenEventTuple, Exception exception, params string[] setBrokenArgs)
		{
			string[] argumentsWithDb = ReplicaInstanceContext.GetArgumentsWithDb(setBrokenArgs, this.m_displayName);
			this.SetBrokenInternal(failureTag, setBrokenEventTuple, new ExtendedErrorInfo(exception), argumentsWithDb);
		}

		public void SetBrokenAndThrow(FailureTag failureTag, ExEventLog.EventTuple setBrokenEventTuple, Exception exception, params string[] setBrokenArgs)
		{
			string[] argumentsWithDb = ReplicaInstanceContext.GetArgumentsWithDb(setBrokenArgs, this.m_displayName);
			this.SetBrokenInternal(failureTag, setBrokenEventTuple, new ExtendedErrorInfo(exception), argumentsWithDb);
			throw new SetBrokenControlTransferException();
		}

		public void ClearBroken()
		{
			lock (this.m_instance)
			{
				this.ClearPreviousFailedState();
				if (!this.IsBroken && !this.IsDisconnected)
				{
					this.ClearCurrentFailedState();
				}
				else
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: We did not actually clear: Config is broken we won't start the run", this.m_displayName);
				}
				if (this.m_instance.Configuration.Type == ReplayConfigType.RemoteCopyTarget && !this.m_instance.PrepareToStopCalled)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: ClearBroken(): State is set to running(healthy)", this.m_displayName);
					this.UpdateInstanceProgress(ReplicaInstanceStage.Running);
				}
				this.ExternalStatus.Refresh();
			}
		}

		public void RestartInstanceSoon(bool fPrepareToStop)
		{
			this.RestartInstanceSoonInternal(false, fPrepareToStop);
		}

		public void RestartInstanceSoonAdminVisible()
		{
			this.RestartInstanceSoonInternal(true, true);
		}

		public void RestartInstanceNow(ReplayConfigChangeHints restartReason)
		{
			this.RestartInstanceSoonInternal(false, true);
			Dependencies.ConfigurationUpdater.NotifyChangedReplayConfiguration(this.m_guid, false, true, true, true, restartReason, -1);
		}

		public void UpdateInstanceProgress(ReplicaInstanceStage stage)
		{
			lock (this.m_instance)
			{
				switch (stage)
				{
				case ReplicaInstanceStage.Initializing:
					this.Initializing = true;
					break;
				case ReplicaInstanceStage.Resynchronizing:
					this.Resynchronizing = true;
					break;
				case ReplicaInstanceStage.Running:
					this.Running = true;
					break;
				default:
					DiagCore.RetailAssert(false, "Invalid ReplicaInstanceStage: {0}", new object[]
					{
						stage.ToString()
					});
					break;
				}
				this.ExternalStatus.Refresh();
			}
		}

		public void SetSuspended()
		{
			lock (this.m_instance)
			{
				this.SetSuspendedInternal();
				this.ExternalStatus.Refresh();
			}
		}

		public void SetFailedAndSuspended(uint failureEventId, LocalizedString errorMessage, ExtendedErrorInfo errorInfo)
		{
			lock (this.m_instance)
			{
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.Failed = 1L;
					this.m_perfmonCounters.FailedSuspended = 1L;
					this.m_perfmonCounters.Disconnected = 0L;
				}
				this.SetSuspendedInternal();
				this.FailureInfo.SetBroken(new uint?(failureEventId), errorMessage, errorInfo);
				this.ExternalStatus.Refresh();
			}
		}

		public void InitializeForSource()
		{
			if (this.m_replayState.ConfigBroken)
			{
				this.FailureInfo.SetBroken(new uint?((uint)this.m_replayState.ConfigBrokenEventId), new LocalizedString(this.m_replayState.ConfigBrokenMessage), this.m_replayState.ConfigBrokenExtendedErrorInfo);
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.Failed = 1L;
					this.m_perfmonCounters.Disconnected = 0L;
					if (this.m_replayState.Suspended)
					{
						this.m_perfmonCounters.FailedSuspended = 1L;
					}
				}
				if (this.m_replayState.Suspended)
				{
					this.SetSuspendedInternal();
				}
				this.ExternalStatus.Refresh();
			}
		}

		public void PersistFailure(uint errorEventId, LocalizedString errorMessage)
		{
			lock (this.m_instance)
			{
				this.FailureInfo.SetBroken(new uint?(errorEventId), errorMessage, null);
				this.FailureInfo.PersistFailure(this.m_replayState);
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.Failed = 1L;
					this.m_perfmonCounters.Disconnected = 0L;
					if (this.Suspended)
					{
						this.m_perfmonCounters.FailedSuspended = 1L;
					}
				}
				this.ExternalStatus.Refresh();
			}
		}

		public void ClearSuspended(bool isActiveCopy, bool restartInstance, bool syncOnly)
		{
			lock (this.m_instance)
			{
				this.LogCrimsonEventOnStateChange<bool>("Suspended", this.Suspended, false);
				this.Suspended = false;
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.Suspended = 0L;
					this.m_perfmonCounters.SuspendedAndNotSeeding = 0L;
				}
				if (!syncOnly)
				{
					this.ClearCurrentFailedState();
					this.ClearPreviousFailedOrDisconnectedState();
					this.DoNotRestart = false;
				}
				else
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "ClearSuspended: {0}: Skipping clearing failure states because 'syncOnly == true'.", this.m_displayName);
				}
				if (!isActiveCopy && restartInstance)
				{
					this.AdminVisibleRestart = true;
					this.RestartSoon = true;
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "ClearSuspended: {0}: Setting RestartSoon flag to ensure RI gets restarted.", this.m_displayName);
				}
				this.ExternalStatus.Refresh();
			}
		}

		public void CheckReseedBlocked()
		{
			lock (this.m_instance)
			{
				if (this.m_replayState.ReseedBlocked)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "CheckReseedBlocked: {0}: ReseedBlocked is set, so throwing.", this.m_displayName);
					throw new SeederInstanceReseedBlockedException(this.m_displayName, AmExceptionHelper.GetMessageOrNoneString(this.ErrorMessage));
				}
			}
		}

		public bool TryBeginDbSeed(RpcSeederArgs rpcSeederArgs)
		{
			bool result;
			lock (this.m_instance)
			{
				if (!this.Suspended)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "TryBeginDbSeed: {0}: Could not mark RI for seed because it is not suspended.", this.m_displayName);
					result = false;
				}
				else
				{
					this.CheckReseedBlocked();
					this.ClearViable();
					this.LogCrimsonEventOnStateChange<bool>("Seeding", this.Seeding, true);
					this.Seeding = true;
					if (this.m_perfmonCounters != null)
					{
						this.m_perfmonCounters.Initializing = 0L;
						this.m_perfmonCounters.Resynchronizing = 0L;
						this.m_perfmonCounters.SuspendedAndNotSeeding = 0L;
					}
					this.m_replayState.ResetForSeed();
					this.InitializeVolumeInfo();
					this.ClearLogStreamStartGeneration();
					this.ClearCurrentFailedState();
					this.ClearPreviousFailedOrDisconnectedState();
					this.SetSeedingStartedErrorMessage(rpcSeederArgs);
					this.ExternalStatus.Refresh();
					result = true;
				}
			}
			return result;
		}

		public void EndDbSeed()
		{
			lock (this.m_instance)
			{
				this.LogCrimsonEventOnStateChange<bool>("Seeding", this.Seeding, false);
				this.Seeding = false;
				if (this.Suspended)
				{
					this.SetSuspendedInternal();
				}
				this.ClearCurrentFailedState();
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Seeding has been finished.", this.m_displayName);
				this.ExternalStatus.Refresh();
			}
		}

		public void FailedDbSeed(ExEventLog.EventTuple errorEventTuple, LocalizedString errorMessage, ExtendedErrorInfo errorInfo)
		{
			lock (this.m_instance)
			{
				this.LogCrimsonEventOnStateChange<bool>("Seeding", this.Seeding, false);
				this.Seeding = false;
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.Failed = 1L;
					this.m_perfmonCounters.Disconnected = 0L;
					if (this.Suspended)
					{
						this.m_perfmonCounters.FailedSuspended = 1L;
					}
				}
				if (this.Suspended)
				{
					this.SetSuspendedInternal();
				}
				this.FailureInfo.SetBroken(errorEventTuple, errorMessage, errorInfo);
				this.FailureInfo.PersistFailure(this.m_replayState);
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, LocalizedString>((long)this.GetHashCode(), "{0}: Seeding failed with error: {1}", this.m_displayName, errorMessage);
				this.ExternalStatus.Refresh();
			}
		}

		public void BeginPassiveSeeding(PassiveSeedingSourceContextEnum passiveSeedingSourceContext, bool invokedForRestart)
		{
			lock (this.m_instance)
			{
				if (!invokedForRestart && this.ExternalStatus.LastCopyStatus != CopyStatusEnum.Healthy && this.ExternalStatus.LastCopyStatus != CopyStatusEnum.DisconnectedAndHealthy)
				{
					throw new CannotBeginSeedingInstanceNotInStateException(this.DisplayName, this.ExternalStatus.LastCopyStatus.ToString());
				}
				this.LogCrimsonEventOnStateChange<PassiveSeedingSourceContextEnum>("PassiveSeedingSourceContext", this.PassiveSeedingSourceContext, passiveSeedingSourceContext);
				this.PassiveSeedingSourceContext = passiveSeedingSourceContext;
				this.ExternalStatus.Refresh();
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Passive Seeding has been started and this server is the source.", this.m_displayName);
			}
		}

		public void EndPassiveSeeding()
		{
			lock (this.m_instance)
			{
				this.LogCrimsonEventOnStateChange<PassiveSeedingSourceContextEnum>("PassiveSeedingSourceContext", this.PassiveSeedingSourceContext, PassiveSeedingSourceContextEnum.None);
				this.PassiveSeedingSourceContext = PassiveSeedingSourceContextEnum.None;
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Passive Seeding has been finished and this server was the source.", this.m_displayName);
				this.ExternalStatus.Refresh();
			}
		}

		public SeedType SeedType { get; private set; }

		public void BeginActiveSeeding(SeedType seedType)
		{
			lock (this.m_instance)
			{
				this.LogCrimsonEventOnStateChange<bool>("ActiveSeedingSource", this.ActiveSeedingSource, true);
				this.ActiveSeedingSource = true;
				this.SeedType = seedType;
				switch (seedType)
				{
				case SeedType.Database:
					this.IsSeedingSourceForDB = true;
					break;
				case SeedType.Catalog:
					this.IsSeedingSourceForCI = true;
					break;
				}
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Active Seeding has been started and this server is the source.", this.m_displayName);
			}
		}

		public void EndActiveSeeding()
		{
			lock (this.m_instance)
			{
				this.LogCrimsonEventOnStateChange<bool>("ActiveSeedingSource", this.ActiveSeedingSource, false);
				this.ActiveSeedingSource = false;
				this.IsSeedingSourceForDB = false;
				this.IsSeedingSourceForCI = false;
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Active Seeding has been finished and this server was the source.", this.m_displayName);
			}
		}

		public void InitializeVolumeInfo()
		{
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: InitializeVolumeInfo() called.", this.m_displayName);
			DatabaseVolumeInfo instance = DatabaseVolumeInfo.GetInstance(this.m_instance.Configuration);
			lock (this.m_instance)
			{
				this.DatabaseVolumeInfo = instance;
				this.m_replayState.SetVolumeInfoIfValid(instance);
			}
		}

		public void UpdateVolumeInfo()
		{
			DatabaseVolumeInfo databaseVolumeInfo = this.DatabaseVolumeInfo;
			MountedFolderPath mountedFolderPath = databaseVolumeInfo.DatabaseVolumeName;
			if (MountedFolderPath.IsNullOrEmpty(mountedFolderPath) && this.m_replayState.VolumeInfoIsValid)
			{
				mountedFolderPath = new MountedFolderPath(this.m_replayState.DatabaseVolumeName);
			}
			DatabaseVolumeInfo instance = DatabaseVolumeInfo.GetInstance(this.m_instance.Configuration);
			lock (this.m_instance)
			{
				this.DatabaseVolumeInfo = instance;
				this.m_replayState.SetVolumeInfoIfValid(instance);
				if (!MountedFolderPath.IsNullOrEmpty(instance.DatabaseVolumeName) && !MountedFolderPath.IsEqual(instance.DatabaseVolumeName, mountedFolderPath))
				{
					this.m_replayState.LastDatabaseVolumeName = mountedFolderPath.Path;
					this.m_replayState.LastDatabaseVolumeNameTransitionTime = DateTime.UtcNow;
				}
			}
		}

		public ReplicaInstanceStage ProgressStage
		{
			get
			{
				ReplicaInstanceStage progressStage;
				lock (this.m_instance)
				{
					progressStage = this.m_progressStage;
				}
				return progressStage;
			}
			private set
			{
				lock (this.m_instance)
				{
					if (!this.m_replayState.ConfigBroken)
					{
						this.LogCrimsonEventOnStateChange<ReplicaInstanceStage>("ReplicaInstanceStage", this.m_progressStage, value);
					}
					this.m_progressStage = value;
				}
			}
		}

		public void ReportOneLogCopied()
		{
			Interlocked.Increment(ref this.m_countLogsCopied);
			this.LogGreenEventIfNecessary();
		}

		public void ReportLogsReplayed(long numLogs)
		{
			Interlocked.Add(ref this.m_countLogsReplayed, numLogs);
			this.LogGreenEventIfNecessary();
		}

		public void LogGreenEventIfNecessary()
		{
			if (this.m_greenEventLogged == 0L && this.m_countLogsCopied > this.m_countLogsThreshold && this.m_countLogsReplayed > this.m_countLogsThreshold && Interlocked.CompareExchange(ref this.m_greenEventLogged, 1L, 0L) == 0L)
			{
				ReplayEventLogConstants.Tuple_ReplicaInstanceMadeProgress.LogEvent(null, new object[]
				{
					this.m_databaseName
				});
			}
		}

		internal void BestEffortDismountReplayDatabase()
		{
			if (this.IsReplayDatabaseDismountPending)
			{
				try
				{
					LogReplayer.DismountReplayDatabase(this.m_guid, this.m_identity, this.m_databaseName, null);
				}
				finally
				{
					this.IsReplayDatabaseDismountPending = false;
				}
			}
		}

		private static string[] GetArgumentsWithDb(string[] argumentsWithoutDb, string database)
		{
			string[] array = new string[argumentsWithoutDb.Length + 1];
			array[0] = database;
			if (argumentsWithoutDb.Length > 0)
			{
				argumentsWithoutDb.CopyTo(array, 1);
			}
			return array;
		}

		private void SetDisconnectedInternal(FailureTag failureTag, ExEventLog.EventTuple setDisconnectedEventTuple, params string[] setDisconnectedArgs)
		{
			string[] argumentsWithDb = ReplicaInstanceContext.GetArgumentsWithDb(setDisconnectedArgs, this.m_displayName);
			int num;
			string text = setDisconnectedEventTuple.EventLogToString(out num, argumentsWithDb);
			lock (this.m_instance)
			{
				if (!this.FailureInfo.IsFailed)
				{
					this.LogCrimsonEventOnStateChange<bool>("Disconnected", this.FailureInfo.IsDisconnected, true);
					this.FailureInfo.SetDisconnected(setDisconnectedEventTuple, new LocalizedString(text));
					if (this.m_perfmonCounters != null)
					{
						this.m_perfmonCounters.Disconnected = 1L;
					}
				}
				else
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0} SetDisconnected ignored because RI is already marked Failed.", this.m_displayName);
				}
				this.ExternalStatus.Refresh();
			}
			bool flag2;
			setDisconnectedEventTuple.LogEvent(this.m_identity, out flag2, argumentsWithDb);
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0} SetDisconnected because {1}", this.m_displayName, text);
		}

		private ExternalReplicaInstanceStatus ExternalStatus { get; set; }

		private void RestartInstanceSoonInternal(bool fAdminVisible, bool fPrepareToStop)
		{
			lock (this.m_instance)
			{
				this.RestartSoon = true;
				if (!this.AdminVisibleRestart && fAdminVisible)
				{
					this.AdminVisibleRestart = fAdminVisible;
					this.ClearCurrentFailedState();
					this.ClearPreviousFailedOrDisconnectedState();
				}
				this.ExternalStatus.Refresh();
			}
			if (fPrepareToStop)
			{
				this.m_instance.PrepareToStop();
			}
		}

		private void ClearCurrentFailedState()
		{
			this.FailureInfo.Reset();
			if (this.m_perfmonCounters != null)
			{
				this.m_perfmonCounters.Failed = 0L;
				this.m_perfmonCounters.FailedSuspended = 0L;
				this.m_perfmonCounters.Disconnected = 0L;
			}
			if (this.m_replayState.ConfigBroken)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0} Failed persisted state cleared.", this.m_displayName);
				this.m_replayState.ConfigBroken = false;
				this.m_replayState.ConfigBrokenMessage = null;
				this.m_replayState.ConfigBrokenEventId = 0L;
				this.m_replayState.ConfigBrokenExtendedErrorInfo = null;
			}
		}

		private void ClearPreviousFailedState()
		{
			if (this.m_instance.PreviousContext != null && this.m_instance.PreviousContext.FailureInfo.IsFailed)
			{
				this.m_instance.PreviousContext.FailureInfo.Reset();
			}
		}

		private void ClearPreviousDisconnectedState()
		{
			if (this.m_instance.PreviousContext != null && this.m_instance.PreviousContext.FailureInfo.IsDisconnected)
			{
				this.m_instance.PreviousContext.FailureInfo.Reset();
			}
		}

		private void ClearPreviousFailedOrDisconnectedState()
		{
			if (this.m_instance.PreviousContext != null)
			{
				this.m_instance.PreviousContext.FailureInfo.Reset();
			}
		}

		private void SetSuspendedInternal()
		{
			this.LogCrimsonEventOnStateChange<bool>("Suspended", this.Suspended, true);
			this.Suspended = true;
			if (this.m_perfmonCounters != null)
			{
				this.m_perfmonCounters.Initializing = 0L;
				this.m_perfmonCounters.Resynchronizing = 0L;
				this.m_perfmonCounters.Suspended = 1L;
				if (this.m_perfmonCounters.FailedSuspended == 1L)
				{
					this.m_perfmonCounters.SuspendedAndNotSeeding = 0L;
					return;
				}
				this.m_perfmonCounters.SuspendedAndNotSeeding = 1L;
			}
		}

		private void SetBrokenInternal(FailureTag failureTag, ExEventLog.EventTuple setBrokenEventTuple, ExtendedErrorInfo extendedErrorInfo, params string[] setBrokenArgsPlusDb)
		{
			int num;
			string text = setBrokenEventTuple.EventLogToString(out num, setBrokenArgsPlusDb);
			Exception failureException = extendedErrorInfo.FailureException;
			int num2 = 0;
			if (failureException != null)
			{
				num2 = failureException.HResult;
			}
			ReplayCrimsonEvents.SetBroken.LogPeriodic<Guid, string, string, string, Exception, int>(this.m_databaseName, DiagCore.DefaultEventSuppressionInterval, this.m_guid, this.m_databaseName, text, Environment.StackTrace, failureException, num2);
			bool flag = false;
			lock (this.m_instance)
			{
				flag = this.IsBroken;
				this.FailureInfo.SetBroken(setBrokenEventTuple, new LocalizedString(text), extendedErrorInfo);
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.Failed = 1L;
					this.m_perfmonCounters.Disconnected = 0L;
					if (this.Suspended)
					{
						this.m_perfmonCounters.FailedSuspended = 1L;
					}
				}
				bool flag3;
				setBrokenEventTuple.LogEvent(this.m_identity, out flag3, setBrokenArgsPlusDb);
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, FailureTag, string>((long)this.GetHashCode(), "{0} SetBroken with tag {1} because {2}", this.m_displayName, failureTag, text);
				MonitoredDatabase monitoredDatabase = MonitoredDatabase.FindMonitoredDatabase(this.ReplicaInstance.Configuration.ServerName, this.m_guid);
				if (monitoredDatabase != null && this.PassiveSeedingSourceContext != PassiveSeedingSourceContextEnum.None)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Cancel seeding for mdb {0}", this.m_guid);
					SourceSeedTable.Instance.CancelSeedingIfAppropriate(SourceSeedTable.CancelReason.CopyFailed, monitoredDatabase.DatabaseGuid);
				}
				bool flag4 = false;
				if (flag3 && (!RegistryParameters.DisableSetBrokenFailureItemSuppression || this.IsSuppressableFailureTag(failureTag)) && !this.IsNonSuppressableFailureTag(failureTag))
				{
					flag4 = true;
				}
				if (!flag4 && failureTag != FailureTag.NoOp)
				{
					FailureItemPublisherHelper.PublishAction(failureTag, this.m_guid, this.m_databaseName);
				}
				if (!flag)
				{
					this.FailureInfo.PersistFailure(this.m_replayState);
				}
				this.ExternalStatus.Refresh();
			}
			if (!flag)
			{
				this.m_instance.PrepareToStop();
			}
		}

		private void SetSeedingStartedErrorMessage(RpcSeederArgs rpcSeederArgs)
		{
			string[] argumentsWithDb = ReplicaInstanceContext.GetArgumentsWithDb(new string[]
			{
				rpcSeederArgs.ToString()
			}, this.m_displayName);
			ExEventLog.EventTuple tuple_SeedInstanceStartedSetBroken = ReplayEventLogConstants.Tuple_SeedInstanceStartedSetBroken;
			int num;
			string value = tuple_SeedInstanceStartedSetBroken.EventLogToString(out num, argumentsWithDb);
			this.FailureInfo.SetBroken(tuple_SeedInstanceStartedSetBroken, new LocalizedString(value), null);
			this.FailureInfo.PersistFailure(this.m_replayState);
		}

		private bool IsSuppressableFailureTag(FailureTag tag)
		{
			return tag == FailureTag.AlertOnly || tag == FailureTag.NoOp;
		}

		private bool IsNonSuppressableFailureTag(FailureTag tag)
		{
			return tag == FailureTag.Reseed || tag == FailureTag.Configuration;
		}

		private void LogCrimsonEventOnStateChange<T>(string stateName, T oldValue, T newValue)
		{
			ReplayState.LogCrimsonEventOnStateChange<T>(this.m_databaseName, this.m_identity, Environment.MachineName, stateName, oldValue, newValue);
		}

		private string m_identity;

		private string m_databaseName;

		private string m_displayName;

		private Guid m_guid;

		private IPerfmonCounters m_perfmonCounters;

		private ReplicaInstance m_instance;

		private ReplicaInstanceStage m_progressStage;

		private ReplayState m_replayState;

		private long m_greenEventLogged;

		private long m_countLogsCopied;

		private long m_countLogsReplayed;

		private long m_countLogsThreshold;

		private bool m_fRestartSoon;

		private bool m_fDoNotRestart;

		private bool m_fSuspended;

		private bool m_fSeeding;

		private PassiveSeedingSourceContextEnum m_PassiveSeedingSourceContext;

		private bool m_fActiveSeedingSource;

		private bool m_fViable;

		private bool m_fAdminVisibleRestart;

		private int m_numThreadsInAcll;
	}
}
