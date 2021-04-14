using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Set", "TenantReadiness", DefaultParameterSetName = "SingleTenantUpdate")]
	public class SetTenantReadiness : SymphonyTaskBase
	{
		[Parameter(Mandatory = false, ParameterSetName = "SingleTenantUpdate")]
		public string[] Constraints { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SingleTenantUpdate")]
		public string Group { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SingleTenantUpdate")]
		public bool IsReady { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "SingleTenantUpdate")]
		public Guid TenantId { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SingleTenantUpdate")]
		public int UpgradeUnits { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SingleTenantUpdate")]
		public SwitchParameter UseDefaultCapacity { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "MultiTenantUpdate")]
		public PSObject[] TenantReadinesses { get; set; }

		protected override void InternalProcessRecord()
		{
			SetTenantReadiness.<>c__DisplayClass1 CS$<>8__locals1 = new SetTenantReadiness.<>c__DisplayClass1();
			CS$<>8__locals1.toUpdate = null;
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (!(parameterSetName == "SingleTenantUpdate"))
				{
					if (parameterSetName == "MultiTenantUpdate")
					{
						if (this.TenantReadinesses.Length > 500)
						{
							base.ThrowTerminatingError(new PSArgumentException("Cannot update more than 500 tenants at a time"), ErrorCategory.InvalidArgument, this.TenantReadinesses);
						}
						List<TenantReadiness> list = new List<TenantReadiness>();
						foreach (PSObject psobject in this.TenantReadinesses)
						{
							string[] constraints = base.GetPropertyValue(psobject.Properties, "Constraints").ToString().Split(new char[]
							{
								';'
							}, StringSplitOptions.RemoveEmptyEntries);
							string groupName = base.GetPropertyValue(psobject.Properties, "GroupName").ToString();
							bool isReady;
							bool.TryParse(base.GetPropertyValue(psobject.Properties, "IsReady").ToString(), out isReady);
							Guid tenantId = new Guid(base.GetPropertyValue(psobject.Properties, "TenantID").ToString());
							int upgradeUnits;
							int.TryParse(base.GetPropertyValue(psobject.Properties, "UpgradeUnits").ToString(), out upgradeUnits);
							bool useDefaultCapacity;
							bool.TryParse(base.GetPropertyValue(psobject.Properties, "UseDefaultCapacity").ToString(), out useDefaultCapacity);
							list.Add(new TenantReadiness(constraints, groupName, isReady, tenantId, upgradeUnits, useDefaultCapacity));
						}
						CS$<>8__locals1.toUpdate = list.ToArray();
					}
				}
				else
				{
					TenantReadiness tenantReadiness = new TenantReadiness(this.Constraints, this.Group, this.IsReady, this.TenantId, this.UpgradeUnits, this.UseDefaultCapacity);
					CS$<>8__locals1.toUpdate = new TenantReadiness[]
					{
						tenantReadiness
					};
				}
			}
			using (ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints> workloadClient = new ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints>(base.WorkloadUri, base.Certificate))
			{
				workloadClient.CallSymphony(delegate
				{
					workloadClient.Proxy.UpdateTenantReadiness(CS$<>8__locals1.toUpdate);
				}, base.WorkloadUri.ToString());
			}
		}

		private const string SingleTenantUpdate = "SingleTenantUpdate";

		private const string MultiTenantUpdate = "MultiTenantUpdate";
	}
}
