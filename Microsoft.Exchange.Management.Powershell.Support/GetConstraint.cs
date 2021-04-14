using System;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "Constraint")]
	public class GetConstraint : SymphonyTaskBase
	{
		public GetConstraint()
		{
			this.Name = Array<string>.Empty;
		}

		[Parameter(Mandatory = false)]
		public string[] Name { get; set; }

		protected override void InternalProcessRecord()
		{
			using (ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints> workloadClient = new ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints>(base.WorkloadUri, base.Certificate))
			{
				Constraint[] constraintResults = null;
				workloadClient.CallSymphony(delegate
				{
					constraintResults = workloadClient.Proxy.QueryConstraint(this.Name);
				}, base.WorkloadUri.ToString());
				base.WriteObject(constraintResults);
			}
		}
	}
}
