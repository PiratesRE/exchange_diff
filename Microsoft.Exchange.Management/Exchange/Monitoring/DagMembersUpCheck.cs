using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal class DagMembersUpCheck : DagMemberCheck
	{
		public DagMembersUpCheck(string serverName, IEventManager eventManager, string momeventsource, uint? ignoreTransientErrorsThreshold, IADDatabaseAvailabilityGroup dag) : base(serverName, "DagMembersUp", CheckId.DagMembersUp, Strings.DagMembersUpCheckDesc, CheckCategory.SystemHighPriority, eventManager, momeventsource, ignoreTransientErrorsThreshold, dag, false)
		{
		}

		public DagMembersUpCheck(string serverName, IEventManager eventManager, string momeventsource, IADDatabaseAvailabilityGroup dag) : base(serverName, "DagMembersUp", CheckId.DagMembersUp, Strings.DagMembersUpCheckDesc, CheckCategory.SystemHighPriority, eventManager, momeventsource, dag, false)
		{
		}

		protected override bool RunIndividualCheck(IAmClusterNode node)
		{
			base.InstanceIdentity = node.Name.NetbiosName;
			AmNodeState state = node.GetState(false);
			if ((state == AmNodeState.Down || state == AmNodeState.Joining || state == AmNodeState.Unknown) && !base.IsNodeStopped(node.Name))
			{
				base.FailContinue(Strings.DagMemberUpCheckFailed(node.Name.NetbiosName, this.m_dag.Name));
				return false;
			}
			return true;
		}

		public const string CheckTitle = "DagMembersUp";
	}
}
