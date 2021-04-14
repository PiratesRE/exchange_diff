using System;
using System.Text;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal class QuorumGroupCheck : DagMemberCheck
	{
		public QuorumGroupCheck(string serverName, IEventManager eventManager, string momeventsource, IADDatabaseAvailabilityGroup dag) : base(serverName, "QuorumGroup", CheckId.QuorumGroup, Strings.QuorumGroupCheckDesc, CheckCategory.SystemMediumPriority, eventManager, momeventsource, dag, true)
		{
		}

		public QuorumGroupCheck(string serverName, IEventManager eventManager, string momeventsource, uint? ignoreTransientErrorsThreshold, IADDatabaseAvailabilityGroup dag) : base(serverName, "QuorumGroup", CheckId.QuorumGroup, Strings.QuorumGroupCheckDesc, CheckCategory.SystemMediumPriority, eventManager, momeventsource, ignoreTransientErrorsThreshold, dag, true)
		{
		}

		protected override void RunCheck()
		{
			using (IAmClusterGroup amClusterGroup = base.Cluster.FindCoreClusterGroup())
			{
				if (amClusterGroup.State != AmGroupState.Online)
				{
					base.Fail(Strings.QuorumGroupNotOnline(amClusterGroup.Name, amClusterGroup.OwnerNode.NetbiosName, base.Cluster.Name, QuorumGroupCheck.ReportResourcesNotOnline(amClusterGroup), Environment.NewLine));
				}
			}
		}

		public static string ReportResourcesNotOnline(IAmClusterGroup resGroup)
		{
			if (resGroup == null)
			{
				throw new ArgumentNullException("resGroup cannot be null!");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (AmClusterResource amClusterResource in resGroup.EnumerateResources())
			{
				using (amClusterResource)
				{
					AmResourceState state = amClusterResource.GetState();
					if (state != AmResourceState.Online)
					{
						stringBuilder.AppendFormat("\t\t{0}: {1}{2}", amClusterResource.Name, state.ToString(), Environment.NewLine);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public const string ReportResourcesFormatString = "\t\t{0}: {1}{2}";
	}
}
