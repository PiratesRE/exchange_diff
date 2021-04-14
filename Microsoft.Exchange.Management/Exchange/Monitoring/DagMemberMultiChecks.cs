using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Monitoring
{
	internal class DagMemberMultiChecks : MultiReplicationCheck
	{
		public DagMemberMultiChecks(string serverName, IEventManager eventManager, string momEventSource, uint ignoreTransientErrorsThreshold, IADDatabaseAvailabilityGroup dag) : base(serverName, eventManager, momEventSource, dag, ignoreTransientErrorsThreshold)
		{
		}

		protected override void Initialize()
		{
			List<ReplicationCheck> list = new List<ReplicationCheck>(9);
			list.Add(new ClusterRpcCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)));
			list.Add(new ReplayServiceCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)));
			list.Add(new ActiveManagerCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)));
			list.Add(new TasksRpcListenerCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)));
			if (this.m_Dag.ThirdPartyReplication != ThirdPartyReplicationMode.Enabled)
			{
				list.Add(new TcpListenerCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold), this.m_Dag));
			}
			else
			{
				list.Add(new ThirdPartyReplCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)));
			}
			list.Add(new ServerLocatorServiceCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)));
			list.Add(new MonitoringServiceCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)));
			list.Add(new DagMembersUpCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold), this.m_Dag));
			list.Add(new ClusterNetworkCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold), this.m_Dag));
			list.Add(new QuorumGroupCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold), this.m_Dag));
			list.Add(new FileShareQuorumCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, this.m_IgnoreTransientErrorsThreshold, this.m_Dag));
			this.m_Checks = list.ToArray();
		}
	}
}
