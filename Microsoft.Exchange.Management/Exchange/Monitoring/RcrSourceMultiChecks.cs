using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Monitoring
{
	internal class RcrSourceMultiChecks : MultiReplicationCheck
	{
		public RcrSourceMultiChecks(string serverName, IEventManager eventManager, string momEventSource, DatabaseHealthValidationRunner validationRunner, List<ReplayConfiguration> replayConfigs, Dictionary<Guid, RpcDatabaseCopyStatus2> copyStatuses, uint ignoreTransientErrorsThreshold) : base(serverName, eventManager, momEventSource, validationRunner, replayConfigs, copyStatuses, ignoreTransientErrorsThreshold)
		{
		}

		protected override void Initialize()
		{
			this.m_Checks = new List<IReplicationCheck>
			{
				new ReplayServiceCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)),
				new ActiveManagerCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)),
				new TasksRpcListenerCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)),
				new DatabaseRedundancyCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, this.m_ValidationRunner, this.m_IgnoreTransientErrorsThreshold),
				new DatabaseAvailabilityCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, this.m_ValidationRunner, this.m_IgnoreTransientErrorsThreshold)
			}.ToArray();
		}
	}
}
