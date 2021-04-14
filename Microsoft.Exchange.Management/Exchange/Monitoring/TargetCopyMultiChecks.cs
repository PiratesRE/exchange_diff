using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Monitoring
{
	internal class TargetCopyMultiChecks : MultiReplicationCheck
	{
		public TargetCopyMultiChecks(string serverName, IEventManager eventManager, string momEventSource, DatabaseHealthValidationRunner validationRunner, List<ReplayConfiguration> replayConfigs, Dictionary<Guid, RpcDatabaseCopyStatus2> copyStatuses, uint ignoreTransientErrorsThreshold) : base(serverName, eventManager, momEventSource, validationRunner, replayConfigs, copyStatuses, ignoreTransientErrorsThreshold)
		{
		}

		protected override void Initialize()
		{
			List<IReplicationCheck> list = new List<IReplicationCheck>();
			list.Add(new ReplayServiceCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)));
			list.Add(new ActiveManagerCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)));
			list.Add(new TasksRpcListenerCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)));
			list.Add(new DatabasesSuspendedCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, this.m_ReplayConfigs, this.m_CopyStatuses));
			list.Add(new DatabasesFailedCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, this.m_ReplayConfigs, this.m_CopyStatuses));
			if (ReplicationCheckGlobals.UsingReplayRpc)
			{
				list.Add(new DatabasesInitializingCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, this.m_ReplayConfigs, this.m_CopyStatuses, this.m_IgnoreTransientErrorsThreshold));
				list.Add(new DatabasesDisconnectedCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, this.m_ReplayConfigs, this.m_CopyStatuses, this.m_IgnoreTransientErrorsThreshold));
				list.Add(new DatabaseRedundancyCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, this.m_ValidationRunner, this.m_IgnoreTransientErrorsThreshold));
				list.Add(new DatabaseAvailabilityCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, this.m_ValidationRunner, this.m_IgnoreTransientErrorsThreshold));
			}
			list.Add(new DatabasesCopyKeepingUpCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, this.m_ReplayConfigs, this.m_CopyStatuses, this.m_IgnoreTransientErrorsThreshold));
			list.Add(new DatabasesReplayKeepingUpCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, this.m_ReplayConfigs, this.m_CopyStatuses, this.m_IgnoreTransientErrorsThreshold));
			this.m_Checks = list.ToArray();
		}
	}
}
