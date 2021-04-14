using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExternalReplicaInstanceStatus
	{
		private ReplayConfigType ConfigType { get; set; }

		private ReplicaInstanceContext CurrentContext { get; set; }

		private ReplicaInstanceContextMinimal PreviousContext { get; set; }

		private IPerfmonCounters PerfmonCounters { get; set; }

		private ReplayState ReplayState { get; set; }

		private string DatabaseName { get; set; }

		public CopyStatusEnum LastCopyStatus
		{
			get
			{
				CopyStatusEnum lastCopyStatus;
				lock (this)
				{
					lastCopyStatus = this.m_lastCopyStatus;
				}
				return lastCopyStatus;
			}
			private set
			{
				lock (this)
				{
					this.m_lastCopyStatus = value;
				}
			}
		}

		public ExternalReplicaInstanceStatus(ReplicaInstanceContext currentContext, ReplicaInstanceContextMinimal previousContext, ReplayConfigType configurationType, IPerfmonCounters perfmonCounters, ReplayState replayState)
		{
			this.CurrentContext = currentContext;
			this.PreviousContext = previousContext;
			this.ConfigType = configurationType;
			this.PerfmonCounters = perfmonCounters;
			this.ReplayState = replayState;
			if (ThirdPartyManager.IsThirdPartyReplicationEnabled && this.ConfigType == ReplayConfigType.RemoteCopyTarget)
			{
				this.LastCopyStatus = CopyStatusEnum.NonExchangeReplication;
			}
			else
			{
				this.LastCopyStatus = CopyStatusEnum.Unknown;
			}
			this.m_displayName = currentContext.DisplayName;
			this.m_identity = currentContext.Identity;
			this.DatabaseName = currentContext.DatabaseName;
		}

		private bool ShouldTrackTransitions
		{
			get
			{
				return !ThirdPartyManager.IsThirdPartyReplicationEnabled && this.ConfigType == ReplayConfigType.RemoteCopyTarget;
			}
		}

		public void CarryOverPreviousStatus(CopyStatusEnum lastCopyStatusEnum)
		{
			this.LastCopyStatus = lastCopyStatusEnum;
		}

		public void Refresh()
		{
			lock (this)
			{
				if (ThirdPartyManager.IsThirdPartyReplicationEnabled && this.ConfigType == ReplayConfigType.RemoteCopyTarget)
				{
					this.LastCopyStatus = CopyStatusEnum.NonExchangeReplication;
				}
				else
				{
					CopyStatusEnum copyStatusEnum;
					if (this.CurrentContext.Seeding)
					{
						copyStatusEnum = CopyStatusEnum.Seeding;
					}
					else if (this.CurrentContext.PassiveSeedingSourceContext == PassiveSeedingSourceContextEnum.Database)
					{
						copyStatusEnum = CopyStatusEnum.SeedingSource;
					}
					else if (this.CurrentContext.Suspended)
					{
						if (this.CurrentContext.IsBroken || (this.PreviousContext != null && this.PreviousContext.FailureInfo.IsFailed))
						{
							copyStatusEnum = CopyStatusEnum.FailedAndSuspended;
						}
						else
						{
							copyStatusEnum = CopyStatusEnum.Suspended;
						}
					}
					else if (this.CurrentContext.IsBroken)
					{
						copyStatusEnum = CopyStatusEnum.Failed;
					}
					else if (!this.CurrentContext.IsDisconnected && this.PreviousContext != null && this.PreviousContext.FailureInfo.IsFailed)
					{
						copyStatusEnum = CopyStatusEnum.Failed;
					}
					else if (this.CurrentContext.IsDisconnected || (this.PreviousContext != null && this.PreviousContext.FailureInfo.IsDisconnected))
					{
						if (this.CurrentContext.Initializing || this.CurrentContext.Resynchronizing || !this.CurrentContext.Viable)
						{
							copyStatusEnum = CopyStatusEnum.DisconnectedAndResynchronizing;
						}
						else
						{
							copyStatusEnum = CopyStatusEnum.DisconnectedAndHealthy;
						}
					}
					else if (this.CurrentContext.Initializing || this.CurrentContext.AdminVisibleRestart)
					{
						copyStatusEnum = CopyStatusEnum.Initializing;
					}
					else if (this.CurrentContext.Resynchronizing || !this.CurrentContext.Viable)
					{
						copyStatusEnum = CopyStatusEnum.Resynchronizing;
					}
					else
					{
						copyStatusEnum = CopyStatusEnum.Healthy;
					}
					if (this.ShouldTrackTransitions && this.LastCopyStatus != copyStatusEnum)
					{
						ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, CopyStatusEnum, CopyStatusEnum>((long)this.GetHashCode(), "{0} CopyStatusEnum changing from '{1}' to '{2}'.", this.m_displayName, this.LastCopyStatus, copyStatusEnum);
						this.LogCrimsonEventOnStateChange<CopyStatusEnum>("CopyStatusEnum", this.LastCopyStatus, copyStatusEnum);
						this.UpdateLastStatusTransitionTime(copyStatusEnum);
						this.LastCopyStatus = copyStatusEnum;
					}
				}
			}
		}

		private void UpdateLastStatusTransitionTime(CopyStatusEnum copyStatus)
		{
			bool flag = false;
			if (copyStatus == CopyStatusEnum.Suspended || copyStatus == CopyStatusEnum.FailedAndSuspended)
			{
				if (this.LastCopyStatus != CopyStatusEnum.Unknown || this.ReplayState.LastStatusTransitionTime.Equals(ReplayState.ZeroFileTime))
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.ReplayState.LastStatusTransitionTime = DateTime.UtcNow;
			}
		}

		private void LogCrimsonEventOnStateChange<T>(string stateName, T oldValue, T newValue)
		{
			ReplayState.LogCrimsonEventOnStateChange<T>(this.DatabaseName, this.m_identity, Environment.MachineName, stateName, oldValue, newValue);
		}

		private string m_displayName;

		private string m_identity;

		private CopyStatusEnum m_lastCopyStatus;
	}
}
