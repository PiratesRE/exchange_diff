using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "SymphonyGroup")]
	public class GetSymphonyGroup : SymphonyTaskBase
	{
		[Parameter(Mandatory = true)]
		public string[] Name { get; set; }

		protected override void InternalProcessRecord()
		{
			using (ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints> workloadClient = new ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints>(base.WorkloadUri, base.Certificate))
			{
				Group[] groupResults = null;
				workloadClient.CallSymphony(delegate
				{
					groupResults = workloadClient.Proxy.QueryGroup(this.Name);
				}, base.WorkloadUri.ToString());
				base.WriteObject(groupResults);
			}
		}
	}
}
