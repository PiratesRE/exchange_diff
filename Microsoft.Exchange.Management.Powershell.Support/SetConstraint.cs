using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Set", "Constraint", DefaultParameterSetName = "SingleConstraintUpdate")]
	public class SetConstraint : SymphonyTaskBase
	{
		[Parameter(Mandatory = true, ParameterSetName = "SingleConstraintUpdate")]
		public DateTime FixByDate { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "SingleConstraintUpdate")]
		public string Owner { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "SingleConstraintUpdate")]
		public ConstraintStatus Status { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "SingleConstraintUpdate")]
		public bool IsBlocking { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SingleConstraintUpdate")]
		public string Comment { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "SingleConstraintUpdate")]
		public string Name { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "MultiConstraintUpdate")]
		public PSObject[] Constraints { get; set; }

		protected override void InternalProcessRecord()
		{
			SetConstraint.<>c__DisplayClass1 CS$<>8__locals1 = new SetConstraint.<>c__DisplayClass1();
			CS$<>8__locals1.toUpdate = null;
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (!(parameterSetName == "SingleConstraintUpdate"))
				{
					if (parameterSetName == "MultiConstraintUpdate")
					{
						List<Constraint> list = new List<Constraint>();
						foreach (PSObject psobject in this.Constraints)
						{
							string constraintName = base.GetPropertyValue(psobject.Properties, "ConstraintName").ToString();
							string owner = base.GetPropertyValue(psobject.Properties, "Owner").ToString();
							string comment = base.GetPropertyValue(psobject.Properties, "Comment").ToString();
							bool isBlocking;
							bool.TryParse(base.GetPropertyValue(psobject.Properties, "IsBlocking").ToString(), out isBlocking);
							int num;
							int.TryParse(base.GetPropertyValue(psobject.Properties, "Status").ToString(), out num);
							ConstraintStatus status = (ConstraintStatus)num;
							DateTime fixByDate;
							DateTime.TryParse(base.GetPropertyValue(psobject.Properties, "FixByDate").ToString(), out fixByDate);
							list.Add(new Constraint(constraintName, owner, fixByDate, status, isBlocking, comment));
						}
						CS$<>8__locals1.toUpdate = list.ToArray();
					}
				}
				else
				{
					Constraint constraint = new Constraint(this.Name, this.Owner, this.FixByDate, this.Status, this.IsBlocking, this.Comment);
					CS$<>8__locals1.toUpdate = new Constraint[]
					{
						constraint
					};
				}
			}
			using (ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints> workloadClient = new ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints>(base.WorkloadUri, base.Certificate))
			{
				workloadClient.CallSymphony(delegate
				{
					workloadClient.Proxy.UpdateConstraint(CS$<>8__locals1.toUpdate);
				}, base.WorkloadUri.ToString());
			}
		}

		private const string SingleConstraintUpdate = "SingleConstraintUpdate";

		private const string MultiConstraintUpdate = "MultiConstraintUpdate";
	}
}
