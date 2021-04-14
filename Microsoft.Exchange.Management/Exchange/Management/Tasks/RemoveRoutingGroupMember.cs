using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Remove", "RoutingGroupMember", SupportsShouldProcess = true)]
	public sealed class RemoveRoutingGroupMember : ManageRoutingGroupMember
	{
		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			this.add = false;
			base.InternalBeginProcessing();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.routingGroup != null)
			{
				this.server.HomeRoutingGroup = null;
				this.configurationSession.Save(this.server);
				if (this.routingGroup.RoutingMasterDN != null && this.routingGroup.RoutingMasterDN.Equals(this.server.Id))
				{
					ADObjectId routingMasterDN = null;
					foreach (ADObjectId adobjectId in this.routingGroup.RoutingMembersDN)
					{
						if (!adobjectId.Equals(this.server.Id))
						{
							routingMasterDN = adobjectId;
							break;
						}
					}
					this.routingGroup.RoutingMasterDN = routingMasterDN;
					this.configurationSession.Save(this.routingGroup);
				}
			}
			TaskLogger.LogExit();
		}
	}
}
