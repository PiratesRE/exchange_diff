using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.LoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CapacitySummaryCapabilityDecorator : MissingCapabilityLoadBalanceClientDecorator
	{
		public CapacitySummaryCapabilityDecorator(ILoadBalanceService service, DirectoryServer targetServer, LoadBalanceAnchorContext context) : base(service, targetServer)
		{
			this.context = context;
		}

		public override HeatMapCapacityData GetCapacitySummary(DirectoryIdentity objectIdentity, bool refreshData)
		{
			if (objectIdentity.Equals(base.TargetServer.Identity))
			{
				LoadContainer localServerData = this.GetLocalServerData(this.context.GetActiveBands());
				return localServerData.ToCapacityData();
			}
			return base.GetCapacitySummary(objectIdentity, refreshData);
		}

		private readonly LoadBalanceAnchorContext context;
	}
}
