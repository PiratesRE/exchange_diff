using System;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Replay.Dumpster;
using Microsoft.Exchange.Cluster.Replay.MountPoint;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class ReplayState
	{
		internal static ReplayState GetReplayState(string nodeName, string sourceNodeName, LockType lockType, string identity, string databaseName)
		{
			return new ReplayState(nodeName, sourceNodeName, lockType, identity, databaseName);
		}

		internal static ReplayState TestGetReplayState(string serverName, string identity, bool fservice)
		{
			return ReplayState.TestGetReplayState(serverName, identity, string.Empty, fservice);
		}

		internal static ReplayState TestGetReplayState(string serverName, string identity, string databaseName, bool fservice)
		{
			return new ReplayState(Environment.MachineName, Environment.MachineName, fservice ? LockType.ReplayService : LockType.Remote, identity, databaseName);
		}

		internal static void DeleteState(string nodeName, Database db)
		{
			ReplayState.DeleteState(nodeName, db, true);
		}

		internal static void DeleteState(string nodeName, Database db, bool fDeleteGlobalAlso)
		{
			ReplayState replayState = ReplayState.GetReplayState(nodeName, nodeName, LockType.Remote, ReplayConfiguration.GetIdentityFromGuid(db.Guid), db.Name);
			replayState.m_stateIO.DeleteState();
			if (fDeleteGlobalAlso)
			{
				replayState.m_stateIOGlobal.DeleteState();
				return;
			}
			ExTraceGlobals.StateTracer.TraceDebug<string>((long)replayState.GetHashCode(), "DeleteState(): Skipping deletion of global state m_stateIOGlobal for DB '{0}'.", db.Name);
		}

		internal static void LogCrimsonEventOnStateChange<T>(string databaseName, string databaseGuidStr, string serverName, string stateName, T oldValue, T newValue)
		{
			if (oldValue == null || !oldValue.Equals(newValue))
			{
				ReplayCrimsonEvents.ReplayStateChange.Log<string, string, string, string, string, string>(databaseName, databaseGuidStr, serverName, stateName, (oldValue != null) ? oldValue.ToString() : "<Null>", (newValue != null) ? newValue.ToString() : "<Null>");
			}
		}

		private ReplayState(string nodeName, string sourceNodeName, LockType lockType, string identity, string databaseName)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (sourceNodeName == null)
			{
				throw new ArgumentNullException("sourceNodeName");
			}
			this.m_nodeName = nodeName;
			this.m_sourceNodeName = sourceNodeName;
			this.m_replayIdentity = identity;
			this.m_replayIdentityGuid = new Guid(this.m_replayIdentity);
			this.m_databaseName = databaseName;
			string machineName = null;
			if (LockType.Remote == lockType)
			{
				machineName = this.m_nodeName;
			}
			this.m_stateIO = new RegistryStateIO(machineName, this.m_replayIdentity, false);
			this.m_stateIOGlobal = new RegistryStateIO(machineName, this.m_replayIdentity, true);
			this.m_safetyNetTable = new SafetyNetInfoCache(this.m_replayIdentity, this.m_databaseName);
			if (lockType == LockType.ReplayService)
			{
				this.m_suspendLock = StateLock.GetNewOrExistingStateLock(this.m_databaseName, this.m_replayIdentity);
				this.m_suspendLockRemote = new StateLockRemote("Suspend", this.m_databaseName, this.m_stateIOGlobal);
				return;
			}
			if (lockType == LockType.Remote)
			{
				this.m_suspendLockRemote = new StateLockRemote("Suspend", this.m_databaseName, this.m_stateIOGlobal);
			}
		}

		internal void UseSetBrokenForIOFailures(ISetBroken setBroken)
		{
			this.m_stateIO.UseSetBrokenForIOFailures(setBroken);
			this.m_stateIOGlobal.UseSetBrokenForIOFailures(setBroken);
		}

		internal IStateIO StateIOTestHook
		{
			get
			{
				return this.m_stateIO;
			}
			set
			{
				this.m_stateIO = value;
			}
		}

		internal IPerfmonCounters PerfmonCounters
		{
			set
			{
				this.m_perfmonCounters = value;
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.ActivationSuspended = (this.ActivationSuspended ? 1L : 0L);
					this.m_perfmonCounters.ReplayLagDisabled = (this.ReplayLagDisabled ? 1L : 0L);
				}
			}
		}

		public LockOwner SuspendLockOwner
		{
			get
			{
				return this.m_suspendLock.CurrentOwner;
			}
		}

		public StateLock SuspendLock
		{
			get
			{
				return this.m_suspendLock;
			}
		}

		public StateLockRemote SuspendLockRemote
		{
			get
			{
				return this.m_suspendLockRemote;
			}
		}

		public string SuspendMessage
		{
			get
			{
				string result;
				this.m_stateIOGlobal.TryReadString("SuspendMessage", null, out result);
				return result;
			}
			set
			{
				this.m_stateIOGlobal.WriteString("SuspendMessage", value, false);
			}
		}

		public bool ConfigBroken
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("ConfigBroken", false, out result);
				return result;
			}
			set
			{
				this.LogCrimsonEventOnStateChange<bool>("ConfigBroken", this.ConfigBroken, value);
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "ConfigBroken is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteBool("ConfigBroken", value, false);
			}
		}

		public string ConfigBrokenMessage
		{
			get
			{
				if (this.m_configBrokenMessage == null)
				{
					this.m_stateIO.TryReadString("ConfigBrokenMessage", null, out this.m_configBrokenMessage);
				}
				return this.m_configBrokenMessage;
			}
			set
			{
				this.m_configBrokenMessage = value;
				this.m_stateIO.WriteString("ConfigBrokenMessage", value, false);
			}
		}

		public long ConfigBrokenEventId
		{
			get
			{
				long result;
				this.m_stateIO.TryReadLong("ConfigBrokenEventId", 0L, out result);
				return result;
			}
			set
			{
				this.m_stateIO.WriteLong("ConfigBrokenEventId", value, false);
			}
		}

		public ExtendedErrorInfo ConfigBrokenExtendedErrorInfo
		{
			get
			{
				if (this.m_configBrokenExtendedErrorInfo == null)
				{
					string text;
					this.m_stateIO.TryReadString("ConfigBrokenExtendedErrorInfo", null, out text);
					if (text != null)
					{
						this.m_configBrokenExtendedErrorInfo = ExtendedErrorInfo.Deserialize(text);
					}
				}
				return this.m_configBrokenExtendedErrorInfo;
			}
			set
			{
				this.m_configBrokenExtendedErrorInfo = value;
				string value2 = null;
				if (value != null)
				{
					value2 = this.m_configBrokenExtendedErrorInfo.SerializeToString();
				}
				this.m_stateIO.WriteString("ConfigBrokenExtendedErrorInfo", value2, false);
			}
		}

		public bool Suspended
		{
			get
			{
				return this.SuspendLockOwner == LockOwner.Suspend;
			}
		}

		public bool ActivationSuspended
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("ActivationSuspended", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "ActivationSuspended is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("ActivationSuspended", this.ActivationSuspended, value);
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.ActivationSuspended = (value ? 1L : 0L);
				}
				this.m_stateIO.WriteBool("ActivationSuspended", value, false);
			}
		}

		public bool ReplaySuspended
		{
			get
			{
				return this.m_replaySuspended;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "ReplaySuspended is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("ReplaySuspended", this.ReplaySuspended, value);
				this.m_replaySuspended = value;
			}
		}

		public LogReplayPlayDownReason ReplayLagPlayDownReason
		{
			get
			{
				return this.m_replayLagPlayDownReason;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<LogReplayPlayDownReason, string>((long)this.GetHashCode(), "ReplayLagPlayDownReason is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<LogReplayPlayDownReason>("ReplayLagPlayDownReason", this.ReplayLagPlayDownReason, value);
				this.m_replayLagPlayDownReason = value;
			}
		}

		public bool ReplayLagDisabled
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("ReplayLagDisabled", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "ReplayLagDisabled is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("ReplayLagDisabled", this.ReplayLagDisabled, value);
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.ReplayLagDisabled = (value ? 1L : 0L);
				}
				this.m_stateIO.WriteBool("ReplayLagDisabled", value, false);
			}
		}

		internal ActionInitiatorType ReplayLagActionInitiator
		{
			get
			{
				ActionInitiatorType result;
				this.m_stateIO.TryReadEnum<ActionInitiatorType>("ReplayLagActionInitiator", ActionInitiatorType.Unknown, out result);
				return result;
			}
			set
			{
				this.m_stateIO.WriteEnum<ActionInitiatorType>("ReplayLagActionInitiator", value, false);
			}
		}

		public string ReplayLagDisabledReason
		{
			get
			{
				string result;
				this.m_stateIO.TryReadString("ReplayLagDisabledReason", null, out result);
				return result;
			}
			set
			{
				this.m_stateIO.WriteString("ReplayLagDisabledReason", value, false);
			}
		}

		public bool ResumeBlocked
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("ResumeBlocked", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "ResumeBlocked is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("ResumeBlocked", this.ResumeBlocked, value);
				this.m_stateIO.WriteBool("ResumeBlocked", value, true);
			}
		}

		public bool ReseedBlocked
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("ReseedBlocked", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "ReseedBlocked is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("ReseedBlocked", this.ReseedBlocked, value);
				this.m_stateIO.WriteBool("ReseedBlocked", value, true);
			}
		}

		public bool InPlaceReseedBlocked
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("InPlaceReseedBlocked", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "InPlaceReseedBlocked is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("InPlaceReseedBlocked", this.InPlaceReseedBlocked, value);
				this.m_stateIO.WriteBool("InPlaceReseedBlocked", value, true);
			}
		}

		public bool LostWrite
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("LostWrite", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "LostWrite is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("LostWrite", this.LostWrite, value);
				this.m_stateIO.WriteBool("LostWrite", value, false);
			}
		}

		public LogRepairMode LogRepairMode
		{
			get
			{
				LogRepairMode result;
				this.m_stateIO.TryReadEnum<LogRepairMode>("LogRepairMode", LogRepairMode.Off, out result);
				return result;
			}
			set
			{
				this.m_stateIO.WriteEnum<LogRepairMode>("LogRepairMode", value, true);
			}
		}

		public long LogRepairRetryCount
		{
			get
			{
				long result;
				this.m_stateIO.TryReadLong("LogRepairRetryCount", 0L, out result);
				return result;
			}
			set
			{
				this.m_stateIO.WriteLong("LogRepairRetryCount", value, true);
			}
		}

		public LastLogInfo GetLastLogInfo()
		{
			LastLogInfo result = new LastLogInfo();
			result.CollectionTime = DateTime.UtcNow;
			result.ClusterTimeIsMissing = false;
			result.ClusterLastLogException = AmHelper.RunAmClusterOperation(delegate
			{
				result.ClusterLastLogGen = this.GetLastLogCommittedGenerationNumberFromCluster();
				bool flag = false;
				result.ClusterLastLogTime = (DateTime)this.GetLatestLogGenerationTimeStampFromCluster(out flag);
				if (!flag)
				{
					result.ClusterTimeIsMissing = true;
				}
			});
			if (result.ClusterLastLogException != null)
			{
				ExTraceGlobals.StateTracer.TraceError<string, Exception>((long)this.GetHashCode(), "LastLogInfo for db '{0}' failed to read from cluster: {1}", this.m_databaseName, result.ClusterLastLogException);
			}
			result.ReplLastLogGen = this.CopyNotificationGenerationNumber;
			result.ReplLastLogTime = this.LatestCopierContactTime;
			if (RegistryParameters.UnboundedDatalossDisableClusterInput && RegistryParameters.UnboundedDatalossDisableReplicationInput)
			{
				result.StaleCheckTime = result.CollectionTime;
			}
			else
			{
				if (RegistryParameters.UnboundedDatalossDisableClusterInput)
				{
					result.StaleCheckTime = result.ReplLastLogTime;
				}
				else if (RegistryParameters.UnboundedDatalossDisableReplicationInput)
				{
					result.StaleCheckTime = result.ClusterLastLogTime;
				}
				else if (result.ReplLastLogTime > result.ClusterLastLogTime)
				{
					result.StaleCheckTime = result.ReplLastLogTime;
				}
				else
				{
					result.StaleCheckTime = result.ClusterLastLogTime;
				}
				if (!result.ClusterTimeIsMissing || RegistryParameters.UnboundedDatalossDisableClusterInput)
				{
					TimeSpan timeSpan = DateTime.UtcNow - result.StaleCheckTime;
					if (timeSpan.TotalSeconds > (double)RegistryParameters.UnboundedDatalossSafeGuardDurationInSec)
					{
						result.IsStale = true;
						ExTraceGlobals.ReplicaInstanceTracer.TraceError((long)this.GetHashCode(), "GetLastLogInfo reports that unbounded dataloss may occur for for db '{0}'LastHeardSpan={1} staleCheckTime={2} replTime={3} clusterTime={4}", new object[]
						{
							this.m_databaseName,
							timeSpan,
							result.StaleCheckTime,
							result.ReplLastLogTime,
							result.ClusterLastLogTime
						});
					}
				}
			}
			if (result.IsStale)
			{
				result.LastLogGenToReport = long.MaxValue;
			}
			else
			{
				result.LastLogGenToReport = Math.Max(result.ReplLastLogGen, result.ClusterLastLogGen);
			}
			return result;
		}

		public ExDateTime GetLatestLogGenerationTimeStampFromCluster(out bool doesValueExist)
		{
			ExDateTime lastLogGenerationTimeStamp = ActiveManagerCore.GetLastLogGenerationTimeStamp(this.m_replayIdentityGuid, out doesValueExist);
			ExTraceGlobals.StateTracer.TraceDebug<ExDateTime, bool>((long)this.GetHashCode(), "LatestLogGenerationTimeStamp {0}, DoesValueExist {1}", lastLogGenerationTimeStamp, doesValueExist);
			return lastLogGenerationTimeStamp;
		}

		public long CopyNotificationGenerationNumber
		{
			get
			{
				if (this.m_copyNotificationGenerationNumber != null)
				{
					return this.m_copyNotificationGenerationNumber.Value;
				}
				long num;
				this.m_stateIO.TryReadLong("CopyNotificationGenerationNumber", 0L, out num);
				this.m_copyNotificationGenerationNumber = new long?(num);
				return num;
			}
			set
			{
				this.m_copyNotificationGenerationNumber = new long?(value);
				ExTraceGlobals.StateTracer.TraceDebug<long, string>((long)this.GetHashCode(), "CopyNotificationGenerationNumber is changing to {0} on replica {1}", value, this.m_replayIdentity);
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.CopyNotificationGenerationNumber = value;
				}
				this.m_stateIO.WriteLong("CopyNotificationGenerationNumber", value, false);
			}
		}

		public long LogStreamStartGeneration
		{
			get
			{
				long result;
				this.m_stateIO.TryReadLong("LogStreamStartGeneration", 0L, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<long, string>((long)this.GetHashCode(), "LogStreamStartGeneration is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteLong("LogStreamStartGeneration", value, true);
			}
		}

		public long CopyGenerationNumber
		{
			get
			{
				if (this.m_copyGenerationNumber != null)
				{
					return this.m_copyGenerationNumber.Value;
				}
				long num;
				this.m_stateIO.TryReadLong("CopyGenerationNumber", 0L, out num);
				this.m_copyGenerationNumber = new long?(num);
				return num;
			}
			set
			{
				this.m_copyGenerationNumber = new long?(value);
				ExTraceGlobals.StateTracer.TraceDebug<long, string>((long)this.GetHashCode(), "CopyGenerationNumber is changing to {0} on replica {1}", value, this.m_replayIdentity);
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.CopyGenerationNumber = value;
				}
				this.m_stateIO.WriteLong("CopyGenerationNumber", value, false);
			}
		}

		public long InspectorGenerationNumber
		{
			get
			{
				if (this.m_inspectorGenerationNumber != null)
				{
					return this.m_inspectorGenerationNumber.Value;
				}
				long num;
				this.m_stateIO.TryReadLong("InspectorGenerationNumber", 0L, out num);
				this.m_inspectorGenerationNumber = new long?(num);
				return num;
			}
			set
			{
				this.m_inspectorGenerationNumber = new long?(value);
				ExTraceGlobals.StateTracer.TraceDebug<long, string>((long)this.GetHashCode(), "InspectorGenerationNumber is changing to {0} on replica {1}", value, this.m_replayIdentity);
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.InspectorGenerationNumber = value;
				}
				this.m_stateIO.WriteLong("InspectorGenerationNumber", value, false);
			}
		}

		public long ReplayGenerationNumber
		{
			get
			{
				if (this.m_replayGenerationNumber != null)
				{
					return this.m_replayGenerationNumber.Value;
				}
				long num;
				this.m_stateIO.TryReadLong("ReplayGenerationNumber", 0L, out num);
				this.m_replayGenerationNumber = new long?(num);
				return num;
			}
			set
			{
				this.m_replayGenerationNumber = new long?(value);
				ExTraceGlobals.StateTracer.TraceDebug<long, string>((long)this.GetHashCode(), "ReplayGenerationNumber is changing to {0} on replica {1}", value, this.m_replayIdentity);
				if (this.m_perfmonCounters != null)
				{
					this.m_perfmonCounters.ReplayGenerationNumber = value;
				}
				this.m_stateIO.WriteLong("ReplayGenerationNumber", value, false);
			}
		}

		public DateTime LatestDataWriteTime
		{
			get
			{
				DateTime latestCopyNotificationTime = this.LatestCopyNotificationTime;
				ExTraceGlobals.StateTracer.TraceDebug<DateTime>((long)this.GetHashCode(), "LatestDataWriteTime from LatestCopyNotificationTime: {0}", latestCopyNotificationTime);
				return latestCopyNotificationTime;
			}
		}

		public DateTime LatestCopyNotificationTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LatestCopyNotificationTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LatestCopyNotificationTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteDateTime("LatestCopyNotificationTime", value, false);
			}
		}

		public DateTime LatestCopierContactTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LatestCopierContactTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LatestCopierContactTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteDateTime("LatestCopierContactTime", value, false);
			}
		}

		public DateTime LatestCopyTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LatestCopyTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LatestCopyTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteDateTime("LatestCopyTime", value, false);
			}
		}

		public DateTime LatestInspectorTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LatestInspectorTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LatestInspectorTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteDateTime("LatestInspectorTime", value, false);
			}
		}

		public DateTime LatestReplayTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LatestReplayTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LatestReplayTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteDateTime("LatestReplayTime", value, false);
			}
		}

		public DateTime CurrentReplayTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("CurrentReplayTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "CurrentReplayTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteDateTime("CurrentReplayTime", value, false);
			}
		}

		public DateTime LastAttemptCopyLastLogsEndTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LastAttemptCopyLastLogsEndTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LastAttemptCopyLastLogsEndTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				if (value != ReplayState.ZeroFileTime)
				{
					this.LogCrimsonEventOnStateChange<DateTime>("LastAttemptCopyLastLogsEndTime", this.LastAttemptCopyLastLogsEndTime, value);
				}
				this.m_stateIO.WriteDateTime("LastAttemptCopyLastLogsEndTime", value, false);
			}
		}

		public long LastAcllLossAmount
		{
			get
			{
				long result;
				this.m_stateIO.TryReadLong("LastAcllLossAmount", 0L, out result);
				return result;
			}
			set
			{
				this.m_stateIO.WriteLong("LastAcllLossAmount", value, true);
			}
		}

		public bool LastAcllRunWithSkipHealthChecks
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("LastAcllRunWithSkipHealthChecks", false, out result);
				return result;
			}
			set
			{
				this.m_stateIO.WriteBool("LastAcllRunWithSkipHealthChecks", value, true);
			}
		}

		public DateTime LastStatusTransitionTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LastStatusTransitionTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LastStatusTransitionTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<DateTime>("LastStatusTransitionTime", this.LastStatusTransitionTime, value);
				this.m_stateIO.WriteDateTime("LastStatusTransitionTime", value, false);
			}
		}

		internal bool SinglePageRestore
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("SinglePageRestore", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "SinglePageRestore is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("SinglePageRestore", this.SinglePageRestore, value);
				this.m_stateIO.WriteBool("SinglePageRestore", value, true);
			}
		}

		internal long SinglePageRestoreNumber
		{
			get
			{
				long result;
				this.m_stateIO.TryReadLong("SinglePageRestoreNumber", 0L, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<long, string>((long)this.GetHashCode(), "SinglePageRestoreNumber is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteLong("SinglePageNumber", value, true);
			}
		}

		public bool SafetyNetRedeliveryRequired
		{
			get
			{
				return this.m_safetyNetTable.IsRedeliveryRequired(false, false);
			}
		}

		public bool DumpsterRedeliveryRequired
		{
			get
			{
				return this.SafetyNetRedeliveryRequired;
			}
		}

		public string DumpsterRedeliveryServers
		{
			get
			{
				return this.m_safetyNetTable.RedeliveryServers;
			}
		}

		public DateTime DumpsterRedeliveryStartTime
		{
			get
			{
				return this.m_safetyNetTable.RedeliveryStartTime;
			}
		}

		public DateTime DumpsterRedeliveryEndTime
		{
			get
			{
				return this.m_safetyNetTable.RedeliveryEndTime;
			}
		}

		public SafetyNetInfoCache GetSafetyNetTable()
		{
			return this.m_safetyNetTable;
		}

		public DateTime LatestFullBackupTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LatestFullBackupTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				this.m_stateIO.WriteDateTime("LatestFullBackupTime", value, false);
			}
		}

		public DateTime LatestIncrementalBackupTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LatestIncrementalBackupTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LatestIncrementalBackupTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteDateTime("LatestIncrementalBackupTime", value, false);
			}
		}

		public DateTime LatestDifferentialBackupTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LatestDifferentialBackupTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LatestDifferentialBackupTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteDateTime("LatestDifferentialBackupTime", value, false);
			}
		}

		public DateTime LatestCopyBackupTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LatestCopyBackupTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LatestCopyBackupTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteDateTime("LatestCopyBackupTime", value, false);
			}
		}

		public bool SnapshotBackup
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("SnapshotBackup", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "SnapshotBackup is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteBool("SnapshotBackup", value, false);
			}
		}

		public bool SnapshotLatestFullBackup
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("SnapshotLatestFullBackup", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "SnapshotLatestFullBackup is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteBool("SnapshotLatestFullBackup", value, false);
			}
		}

		public bool SnapshotLatestIncrementalBackup
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("SnapshotLatestIncrementalBackup", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "SnapshotLatestIncrementalBackup is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteBool("SnapshotLatestIncrementalBackup", value, false);
			}
		}

		public bool SnapshotLatestDifferentialBackup
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("SnapshotLatestDifferentialBackup", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "SnapshotLatestDifferentialBackup is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteBool("SnapshotLatestDifferentialBackup", value, false);
			}
		}

		public bool SnapshotLatestCopyBackup
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("SnapshotLatestCopyBackup", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "SnapshotLatestCopyBackup is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteBool("SnapshotLatestCopyBackup", value, false);
			}
		}

		internal ActionInitiatorType ActionInitiator
		{
			get
			{
				ActionInitiatorType result;
				this.m_stateIO.TryReadEnum<ActionInitiatorType>("ActionInitiator", ActionInitiatorType.Unknown, out result);
				return result;
			}
			set
			{
				this.m_stateIO.WriteEnum<ActionInitiatorType>("ActionInitiator", value, false);
			}
		}

		internal int WorkerProcessId
		{
			get
			{
				if (this.m_workerProcessId == 0)
				{
					long num;
					this.m_stateIO.TryReadLong("WorkerProcessId", 0L, out num);
					this.m_workerProcessId = (int)num;
				}
				return this.m_workerProcessId;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<int, string>((long)this.GetHashCode(), "StoreWorkerProcessId is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_workerProcessId = value;
				this.m_stateIO.WriteLong("WorkerProcessId", (long)value, false);
			}
		}

		internal DateTime LastCopyAvailabilityChecksPassedTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LastCopyAvailabilityChecksPassedTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LastCopyAvailabilityChecksPassedTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteDateTime("LastCopyAvailabilityChecksPassedTime", value, false);
			}
		}

		internal bool IsLastCopyAvailabilityChecksPassed
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("IsLastCopyAvailabilityChecksPassed", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "IsLastCopyAvailabilityChecksPassed is being set to '{0}' on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("IsLastCopyAvailabilityChecksPassed", this.IsLastCopyAvailabilityChecksPassed, value);
				this.m_stateIO.WriteBool("IsLastCopyAvailabilityChecksPassed", value, true);
			}
		}

		internal DateTime LastCopyRedundancyChecksPassedTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LastCopyRedundancyChecksPassedTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LastCopyRedundancyChecksPassedTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.m_stateIO.WriteDateTime("LastCopyRedundancyChecksPassedTime", value, false);
			}
		}

		internal bool IsLastCopyRedundancyChecksPassed
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("IsLastCopyRedundancyChecksPassed", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "IsLastCopyRedundancyChecksPassed is being set to '{0}' on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("IsLastCopyRedundancyChecksPassed", this.IsLastCopyRedundancyChecksPassed, value);
				this.m_stateIO.WriteBool("IsLastCopyRedundancyChecksPassed", value, true);
			}
		}

		internal string LastDatabaseVolumeName
		{
			get
			{
				string result;
				this.m_stateIO.TryReadString("LastDatabaseVolumeName", null, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<string, string>((long)this.GetHashCode(), "LastDatabaseVolumeName is being set to '{0}' on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<string>("LastDatabaseVolumeName", this.LastDatabaseVolumeName, value);
				this.m_stateIO.WriteString("LastDatabaseVolumeName", value, false);
			}
		}

		public DateTime LastDatabaseVolumeNameTransitionTime
		{
			get
			{
				DateTime result;
				this.m_stateIO.TryReadDateTime("LastDatabaseVolumeNameTransitionTime", DateTime.FromFileTimeUtc(0L), out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "LastDatabaseVolumeNameTransitionTime is changing to {0} on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<DateTime>("LastDatabaseVolumeNameTransitionTime", this.LastDatabaseVolumeNameTransitionTime, value);
				this.m_stateIO.WriteDateTime("LastDatabaseVolumeNameTransitionTime", value, false);
			}
		}

		internal string ExchangeVolumeMountPoint
		{
			get
			{
				string result;
				this.m_stateIO.TryReadString("ExchangeVolumeMountPoint", null, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ExchangeVolumeMountPoint is being set to '{0}' on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<string>("ExchangeVolumeMountPoint", this.ExchangeVolumeMountPoint, value);
				this.m_stateIO.WriteString("ExchangeVolumeMountPoint", value, true);
			}
		}

		internal string DatabaseVolumeMountPoint
		{
			get
			{
				string result;
				this.m_stateIO.TryReadString("DatabaseVolumeMountPoint", null, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<string, string>((long)this.GetHashCode(), "DatabaseVolumeMountPoint is being set to '{0}' on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<string>("DatabaseVolumeMountPoint", this.DatabaseVolumeMountPoint, value);
				this.m_stateIO.WriteString("DatabaseVolumeMountPoint", value, true);
			}
		}

		internal string DatabaseVolumeName
		{
			get
			{
				string result;
				this.m_stateIO.TryReadString("DatabaseVolumeName", null, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<string, string>((long)this.GetHashCode(), "DatabaseVolumeName is being set to '{0}' on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<string>("DatabaseVolumeName", this.DatabaseVolumeName, value);
				this.m_stateIO.WriteString("DatabaseVolumeName", value, true);
			}
		}

		internal bool IsDatabasePathOnMountedFolder
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("IsDatabasePathOnMountedFolder", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "IsDatabasePathOnMountedFolder is being set to '{0}' on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("IsDatabasePathOnMountedFolder", this.IsDatabasePathOnMountedFolder, value);
				this.m_stateIO.WriteBool("IsDatabasePathOnMountedFolder", value, true);
			}
		}

		internal string LogVolumeMountPoint
		{
			get
			{
				string result;
				this.m_stateIO.TryReadString("LogVolumeMountPoint", null, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<string, string>((long)this.GetHashCode(), "LogVolumeMountPoint is being set to '{0}' on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<string>("LogVolumeMountPoint", this.LogVolumeMountPoint, value);
				this.m_stateIO.WriteString("LogVolumeMountPoint", value, true);
			}
		}

		internal string LogVolumeName
		{
			get
			{
				string result;
				this.m_stateIO.TryReadString("LogVolumeName", null, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<string, string>((long)this.GetHashCode(), "LogVolumeName is being set to '{0}' on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<string>("LogVolumeName", this.LogVolumeName, value);
				this.m_stateIO.WriteString("LogVolumeName", value, true);
			}
		}

		internal bool IsLogPathOnMountedFolder
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("IsLogPathOnMountedFolder", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "IsLogPathOnMountedFolder is being set to '{0}' on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("IsLogPathOnMountedFolder", this.IsLogPathOnMountedFolder, value);
				this.m_stateIO.WriteBool("IsLogPathOnMountedFolder", value, true);
			}
		}

		internal bool VolumeInfoIsValid
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool("VolumeInfoIsValid", false, out result);
				return result;
			}
			set
			{
				ExTraceGlobals.StateTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "VolumeInfoIsValid is being set to '{0}' on replica {1}", value, this.m_replayIdentity);
				this.LogCrimsonEventOnStateChange<bool>("VolumeInfoIsValid", this.VolumeInfoIsValid, value);
				this.m_stateIO.WriteBool("VolumeInfoIsValid", value, true);
			}
		}

		internal void SetVolumeInfoIfValid(DatabaseVolumeInfo vi)
		{
			if (!vi.IsValid)
			{
				ExTraceGlobals.StateTracer.TraceError<string>((long)this.GetHashCode(), "DatabaseVolumeInfo is not valid for replica {0}, so skipping persisting it.", this.m_replayIdentity);
				return;
			}
			this.VolumeInfoIsValid = false;
			if (this.m_stateIO.IOFailures)
			{
				ExTraceGlobals.StateTracer.TraceError<string>((long)this.GetHashCode(), "An error occurred when clearing VolumeInfoIsValid state for replica {0}.", this.m_replayIdentity);
				return;
			}
			this.DatabaseVolumeMountPoint = vi.DatabaseVolumeMountPoint.Path;
			if (this.m_stateIO.IOFailures)
			{
				ExTraceGlobals.StateTracer.TraceError<string>((long)this.GetHashCode(), "An error occurred when writing DatabaseVolumeMountPoint state for replica {0}.", this.m_replayIdentity);
				return;
			}
			this.DatabaseVolumeName = vi.DatabaseVolumeName.Path;
			if (this.m_stateIO.IOFailures)
			{
				ExTraceGlobals.StateTracer.TraceError<string>((long)this.GetHashCode(), "An error occurred when writing DatabaseVolumeName state for replica {0}.", this.m_replayIdentity);
				return;
			}
			this.IsDatabasePathOnMountedFolder = vi.IsDatabasePathOnMountedFolder;
			if (this.m_stateIO.IOFailures)
			{
				ExTraceGlobals.StateTracer.TraceError<string>((long)this.GetHashCode(), "An error occurred when writing IsDatabasePathOnMountedFolder state for replica {0}.", this.m_replayIdentity);
				return;
			}
			this.LogVolumeMountPoint = vi.LogVolumeMountPoint.Path;
			if (this.m_stateIO.IOFailures)
			{
				ExTraceGlobals.StateTracer.TraceError<string>((long)this.GetHashCode(), "An error occurred when writing LogVolumeMountPoint state for replica {0}.", this.m_replayIdentity);
				return;
			}
			this.LogVolumeName = vi.LogVolumeName.Path;
			if (this.m_stateIO.IOFailures)
			{
				ExTraceGlobals.StateTracer.TraceError<string>((long)this.GetHashCode(), "An error occurred when writing LogVolumeName state for replica {0}.", this.m_replayIdentity);
				return;
			}
			this.IsLogPathOnMountedFolder = vi.IsLogPathOnMountedFolder;
			if (this.m_stateIO.IOFailures)
			{
				ExTraceGlobals.StateTracer.TraceError<string>((long)this.GetHashCode(), "An error occurred when writing IsLogPathOnMountedFolder state for replica {0}.", this.m_replayIdentity);
				return;
			}
			if (vi.IsExchangeVolumeMountPointValid)
			{
				this.ExchangeVolumeMountPoint = vi.ExchangeVolumeMountPoint.Path;
				if (this.m_stateIO.IOFailures)
				{
					ExTraceGlobals.StateTracer.TraceError<string>((long)this.GetHashCode(), "An error occurred when writing ExchangeVolumeMountPoint state for replica {0}.", this.m_replayIdentity);
					return;
				}
			}
			this.VolumeInfoIsValid = true;
			if (this.m_stateIO.IOFailures)
			{
				ExTraceGlobals.StateTracer.TraceError<string>((long)this.GetHashCode(), "An error occurred when setting VolumeInfoIsValid state to true for replica {0}.", this.m_replayIdentity);
			}
		}

		internal long GetLastLogCommittedGenerationNumberFromCluster()
		{
			long num = long.MaxValue;
			long lastLogGenerationNumber = ActiveManagerCore.GetLastLogGenerationNumber(this.m_replayIdentityGuid);
			if (lastLogGenerationNumber != 9223372036854775807L)
			{
				num = lastLogGenerationNumber;
			}
			ExTraceGlobals.StateTracer.TraceDebug<long, long>((long)this.GetHashCode(), "LastLogCommittedGenerationNumber {0}; ClusdbLastLog {1}.", num, lastLogGenerationNumber);
			return num;
		}

		private void LogCrimsonEventOnStateChange<T>(string stateName, T oldValue, T newValue)
		{
			ReplayState.LogCrimsonEventOnStateChange<T>(this.m_databaseName, this.m_replayIdentity, this.m_nodeName, stateName, oldValue, newValue);
		}

		internal static DateTime ZeroTicksTimeUtc
		{
			get
			{
				return new DateTime(0L).ToUniversalTime();
			}
		}

		internal static DateTime ZeroFileTime
		{
			get
			{
				return DateTime.FromFileTimeUtc(0L);
			}
		}

		internal void ResetForSeed()
		{
			this.CopyGenerationNumber = 0L;
			this.CopyNotificationGenerationNumber = 0L;
			this.CurrentReplayTime = ReplayState.ZeroFileTime;
			this.InspectorGenerationNumber = 0L;
			this.LastAttemptCopyLastLogsEndTime = ReplayState.ZeroFileTime;
			this.LatestCopyNotificationTime = ReplayState.ZeroFileTime;
			this.LatestCopyTime = ReplayState.ZeroFileTime;
			this.LatestInspectorTime = ReplayState.ZeroFileTime;
			this.LatestReplayTime = ReplayState.ZeroFileTime;
			this.ReplayGenerationNumber = 0L;
			this.LogRepairMode = LogRepairMode.Off;
			this.LogRepairRetryCount = 0L;
			this.ResumeBlocked = false;
			this.ReseedBlocked = false;
			this.InPlaceReseedBlocked = false;
		}

		public const string LogRepairModeValueName = "LogRepairMode";

		public const string LogRepairRetryCountValueName = "LogRepairRetryCount";

		internal const string LastCopyAvailabilityChecksPassedTime_Name = "LastCopyAvailabilityChecksPassedTime";

		internal const string LastCopyRedundancyChecksPassedTime_Name = "LastCopyRedundancyChecksPassedTime";

		internal const string IsLastCopyAvailabilityChecksPassed_Name = "IsLastCopyAvailabilityChecksPassed";

		internal const string IsLastCopyRedundancyChecksPassed_Name = "IsLastCopyRedundancyChecksPassed";

		private string m_nodeName;

		private string m_sourceNodeName;

		private string m_replayIdentity;

		private string m_databaseName;

		private Guid m_replayIdentityGuid;

		private StateLock m_suspendLock;

		private StateLockRemote m_suspendLockRemote;

		private IStateIO m_stateIO;

		private IStateIO m_stateIOGlobal;

		private SafetyNetInfoCache m_safetyNetTable;

		private string m_configBrokenMessage;

		private ExtendedErrorInfo m_configBrokenExtendedErrorInfo;

		private long? m_copyNotificationGenerationNumber = null;

		private long? m_copyGenerationNumber = null;

		private long? m_inspectorGenerationNumber = null;

		private long? m_replayGenerationNumber = null;

		private IPerfmonCounters m_perfmonCounters;

		private bool m_replaySuspended;

		private LogReplayPlayDownReason m_replayLagPlayDownReason;

		private int m_workerProcessId;
	}
}
