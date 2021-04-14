using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Set", "GroupCapacity", DefaultParameterSetName = "SingleGroupCapacityUpdate")]
	public class SetGroupCapacity : SymphonyTaskBase
	{
		[Parameter(Mandatory = false, ParameterSetName = "SingleGroupCapacityUpdate")]
		public DateTime StartDate { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SingleGroupCapacityUpdate")]
		public int UpgradeUnits { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SingleGroupCapacityUpdate")]
		public CapacityBlock[] CapacityBlocks { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "SingleGroupCapacityUpdate")]
		public string GroupName { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "MultiGroupCapacityUpdate")]
		public PSObject[] GroupCapacities { get; set; }

		protected override void InternalProcessRecord()
		{
			SetGroupCapacity.<>c__DisplayClass1 CS$<>8__locals1 = new SetGroupCapacity.<>c__DisplayClass1();
			CS$<>8__locals1.toUpdate = null;
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (!(parameterSetName == "SingleGroupCapacityUpdate"))
				{
					if (parameterSetName == "MultiGroupCapacityUpdate")
					{
						Dictionary<string, List<CapacityBlock>> dictionary = new Dictionary<string, List<CapacityBlock>>();
						foreach (PSObject psobject in this.GroupCapacities)
						{
							string text = base.GetPropertyValue(psobject.Properties, "GroupName").ToString();
							int upgradeUnits;
							int.TryParse(base.GetPropertyValue(psobject.Properties, "UpgradeUnits").ToString(), out upgradeUnits);
							DateTime startDate;
							DateTime.TryParse(base.GetPropertyValue(psobject.Properties, "StartDate").ToString(), out startDate);
							Console.WriteLine("Capacity Group name is {0}", text);
							if (dictionary.ContainsKey(text))
							{
								Console.WriteLine("CSV Input Contains this groupname already");
								if (dictionary[text].Count >= 20)
								{
									base.ThrowTerminatingError(new PSArgumentException("Cannot update more than 20 capacities per group"), ErrorCategory.InvalidArgument, this.GroupCapacities);
								}
								dictionary[text].Add(new CapacityBlock(startDate, upgradeUnits));
							}
							else
							{
								Console.WriteLine("CSV Input is creating a new group name");
								dictionary.Add(text, new List<CapacityBlock>
								{
									new CapacityBlock(startDate, upgradeUnits)
								});
							}
						}
						List<GroupCapacity> list = new List<GroupCapacity>();
						foreach (KeyValuePair<string, List<CapacityBlock>> keyValuePair in dictionary)
						{
							list.Add(new GroupCapacity(keyValuePair.Key, keyValuePair.Value.ToArray()));
						}
						CS$<>8__locals1.toUpdate = list.ToArray();
					}
				}
				else
				{
					CapacityBlock[] capacities;
					if (this.CapacityBlocks == null)
					{
						CapacityBlock capacityBlock = new CapacityBlock(this.StartDate, this.UpgradeUnits);
						capacities = new CapacityBlock[]
						{
							capacityBlock
						};
					}
					else
					{
						capacities = this.CapacityBlocks;
					}
					GroupCapacity groupCapacity = new GroupCapacity(this.GroupName, capacities);
					CS$<>8__locals1.toUpdate = new GroupCapacity[]
					{
						groupCapacity
					};
				}
			}
			using (ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints> workloadClient = new ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints>(base.WorkloadUri, base.Certificate))
			{
				workloadClient.CallSymphony(delegate
				{
					workloadClient.Proxy.UpdateCapacity(CS$<>8__locals1.toUpdate);
				}, base.WorkloadUri.ToString());
			}
		}

		private const string SingleGroupCapacityUpdate = "SingleGroupCapacityUpdate";

		private const string MultiGroupCapacityUpdate = "MultiGroupCapacityUpdate";
	}
}
