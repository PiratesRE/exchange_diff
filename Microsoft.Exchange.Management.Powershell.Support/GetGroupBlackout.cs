using System;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "GroupBlackout")]
	public class GetGroupBlackout : SymphonyTaskBase
	{
		public GetGroupBlackout()
		{
			this.Group = Array<string>.Empty;
		}

		[Parameter(Mandatory = false)]
		public string[] Group { get; set; }

		protected override void InternalProcessRecord()
		{
			using (ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints> workloadClient = new ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints>(base.WorkloadUri, base.Certificate))
			{
				GroupBlackout[] groupBlackoutResults = null;
				workloadClient.CallSymphony(delegate
				{
					groupBlackoutResults = workloadClient.Proxy.QueryBlackout(this.Group);
				}, base.WorkloadUri.ToString());
				foreach (GroupBlackout groupBlackout in groupBlackoutResults)
				{
					foreach (BlackoutInterval blackout in groupBlackout.Intervals)
					{
						base.WriteObject(new GroupBlackoutDisplay(groupBlackout.GroupName, blackout));
					}
				}
			}
		}
	}
}
