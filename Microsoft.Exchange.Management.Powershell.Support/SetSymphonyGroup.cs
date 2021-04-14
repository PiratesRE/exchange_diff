using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Set", "SymphonyGroup", DefaultParameterSetName = "SingleGroupUpdate")]
	public class SetSymphonyGroup : SymphonyTaskBase
	{
		[Parameter(Mandatory = true, ParameterSetName = "SingleGroupUpdate")]
		public DataCenterRegion Region { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "SingleGroupUpdate")]
		public string Group { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "MultiGroupUpdate")]
		public PSObject[] Groups { get; set; }

		protected override void InternalProcessRecord()
		{
			SetSymphonyGroup.<>c__DisplayClass1 CS$<>8__locals1 = new SetSymphonyGroup.<>c__DisplayClass1();
			CS$<>8__locals1.toUpdate = null;
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (!(parameterSetName == "SingleGroupUpdate"))
				{
					if (parameterSetName == "MultiGroupUpdate")
					{
						List<Group> list = new List<Group>();
						foreach (PSObject psobject in this.Groups)
						{
							string groupName = base.GetPropertyValue(psobject.Properties, "GroupName").ToString();
							int num;
							int.TryParse(base.GetPropertyValue(psobject.Properties, "Region").ToString(), out num);
							DataCenterRegion regionName = (DataCenterRegion)num;
							list.Add(new Group(groupName, regionName));
						}
						CS$<>8__locals1.toUpdate = list.ToArray();
					}
				}
				else
				{
					Group group = new Group(this.Group, this.Region);
					CS$<>8__locals1.toUpdate = new Group[]
					{
						group
					};
				}
			}
			using (ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints> workloadClient = new ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints>(base.WorkloadUri, base.Certificate))
			{
				workloadClient.CallSymphony(delegate
				{
					workloadClient.Proxy.UpdateGroup(CS$<>8__locals1.toUpdate);
				}, base.WorkloadUri.ToString());
			}
		}

		private const string SingleGroupUpdate = "SingleGroupUpdate";

		private const string MultiGroupUpdate = "MultiGroupUpdate";
	}
}
