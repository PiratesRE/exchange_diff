using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "TenantReadiness")]
	public class GetTenantReadiness : SymphonyTaskBase
	{
		[Parameter(Mandatory = true)]
		public Guid[] TenantID { get; set; }

		protected override void InternalProcessRecord()
		{
			using (ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints> workloadClient = new ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints>(base.WorkloadUri, base.Certificate))
			{
				TenantReadiness[] readiness = null;
				workloadClient.CallSymphony(delegate
				{
					readiness = workloadClient.Proxy.QueryTenantReadiness(this.TenantID);
				}, base.WorkloadUri.ToString());
				base.WriteObject(readiness);
			}
		}
	}
}
