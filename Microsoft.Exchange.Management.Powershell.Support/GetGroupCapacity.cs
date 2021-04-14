using System;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "GroupCapacity")]
	public class GetGroupCapacity : SymphonyTaskBase
	{
		public GetGroupCapacity()
		{
			this.Group = Array<string>.Empty;
		}

		[Parameter(Mandatory = false)]
		public string[] Group { get; set; }

		protected override void InternalProcessRecord()
		{
			using (ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints> workloadClient = new ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints>(base.WorkloadUri, base.Certificate))
			{
				GroupCapacity[] groupCapacityResults = null;
				workloadClient.CallSymphony(delegate
				{
					groupCapacityResults = workloadClient.Proxy.QueryCapacity(this.Group);
				}, base.WorkloadUri.ToString());
				foreach (GroupCapacity groupCapacity in groupCapacityResults)
				{
					foreach (CapacityBlock capacity in groupCapacity.Capacities)
					{
						base.WriteObject(new GroupCapacityDisplay(groupCapacity.GroupName, capacity));
					}
				}
			}
		}
	}
}
