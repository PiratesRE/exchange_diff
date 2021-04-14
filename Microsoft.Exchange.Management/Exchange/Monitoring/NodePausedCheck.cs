using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal class NodePausedCheck : DagMemberCheck
	{
		public NodePausedCheck(string serverName, IEventManager eventManager, string momeventsource, IADDatabaseAvailabilityGroup dag) : base(serverName, "NodePaused", CheckId.NodePaused, Strings.NodePausedCheckDesc, CheckCategory.SystemMediumPriority, eventManager, momeventsource, dag, false)
		{
		}

		public NodePausedCheck(string serverName, IEventManager eventManager, string momeventsource, uint? ignoreTransientErrorsThreshold, IADDatabaseAvailabilityGroup dag) : base(serverName, "NodePaused", CheckId.NodePaused, Strings.NodePausedCheckDesc, CheckCategory.SystemMediumPriority, eventManager, momeventsource, ignoreTransientErrorsThreshold, dag, false)
		{
		}

		protected override bool RunIndividualCheck(IAmClusterNode node)
		{
			base.InstanceIdentity = node.Name.NetbiosName;
			AmNodeState state = node.GetState(false);
			if (state == AmNodeState.Paused)
			{
				base.FailContinue(Strings.DagMemberPausedFailed(node.Name.NetbiosName, this.m_dag.Name));
				return false;
			}
			return true;
		}
	}
}
