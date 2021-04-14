using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Add", "RoutingGroupMember", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class AddRoutingGroupMember : ManageRoutingGroupMember
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.server.HomeRoutingGroup == null)
			{
				this.server.HomeRoutingGroup = this.routingGroup.Id;
				this.configurationSession.Save(this.server);
				if (this.routingGroup.RoutingMasterDN == null)
				{
					this.routingGroup.RoutingMasterDN = this.server.Id;
					this.configurationSession.Save(this.routingGroup);
				}
			}
			TaskLogger.LogExit();
		}
	}
}
