using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Set", "GroupBlackout", DefaultParameterSetName = "SingleGroupBlackoutUpdate")]
	public class SetGroupBlackout : SymphonyTaskBase
	{
		[Parameter(Mandatory = false, ParameterSetName = "SingleGroupBlackoutUpdate")]
		public DateTime StartDate { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SingleGroupBlackoutUpdate")]
		public DateTime EndDate { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SingleGroupBlackoutUpdate")]
		public string Reason { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SingleGroupBlackoutUpdate")]
		public BlackoutInterval[] BlackoutIntervals { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "SingleGroupBlackoutUpdate")]
		public string Group { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "MultiGroupBlackoutUpdate")]
		public PSObject[] GroupBlackouts { get; set; }

		protected override void InternalProcessRecord()
		{
			SetGroupBlackout.<>c__DisplayClass1 CS$<>8__locals1 = new SetGroupBlackout.<>c__DisplayClass1();
			CS$<>8__locals1.toUpdate = null;
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (!(parameterSetName == "SingleGroupBlackoutUpdate"))
				{
					if (parameterSetName == "MultiGroupBlackoutUpdate")
					{
						Dictionary<string, List<BlackoutInterval>> dictionary = new Dictionary<string, List<BlackoutInterval>>();
						foreach (PSObject psobject in this.GroupBlackouts)
						{
							string key = base.GetPropertyValue(psobject.Properties, "GroupName").ToString();
							string reason = base.GetPropertyValue(psobject.Properties, "Reason").ToString();
							DateTime startDate;
							DateTime.TryParse(base.GetPropertyValue(psobject.Properties, "StartDate").ToString(), out startDate);
							DateTime endDate;
							DateTime.TryParse(base.GetPropertyValue(psobject.Properties, "EndDate").ToString(), out endDate);
							if (dictionary.ContainsKey(key))
							{
								if (dictionary[key].Count >= 20)
								{
									base.ThrowTerminatingError(new PSArgumentException("Cannot update more than 20 BlackoutIntervals per group"), ErrorCategory.InvalidArgument, this.GroupBlackouts);
								}
								dictionary[key].Add(new BlackoutInterval(startDate, endDate, reason));
							}
							else
							{
								dictionary.Add(key, new List<BlackoutInterval>
								{
									new BlackoutInterval(startDate, endDate, reason)
								});
							}
						}
						List<GroupBlackout> list = new List<GroupBlackout>();
						foreach (KeyValuePair<string, List<BlackoutInterval>> keyValuePair in dictionary)
						{
							list.Add(new GroupBlackout(keyValuePair.Key, keyValuePair.Value.ToArray()));
						}
						CS$<>8__locals1.toUpdate = list.ToArray();
					}
				}
				else
				{
					BlackoutInterval[] intervals;
					if (this.BlackoutIntervals == null)
					{
						BlackoutInterval blackoutInterval = new BlackoutInterval(this.StartDate, this.EndDate, this.Reason);
						intervals = new BlackoutInterval[]
						{
							blackoutInterval
						};
					}
					else
					{
						intervals = this.BlackoutIntervals;
					}
					GroupBlackout groupBlackout = new GroupBlackout(this.Group, intervals);
					CS$<>8__locals1.toUpdate = new GroupBlackout[]
					{
						groupBlackout
					};
				}
			}
			using (ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints> workloadClient = new ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints>(base.WorkloadUri, base.Certificate))
			{
				workloadClient.CallSymphony(delegate
				{
					workloadClient.Proxy.UpdateBlackout(CS$<>8__locals1.toUpdate);
				}, base.WorkloadUri.ToString());
			}
		}

		private const string SingleGroupBlackoutUpdate = "SingleGroupBlackoutUpdate";

		private const string MultiGroupBlackoutUpdate = "MultiGroupBlackoutUpdate";
	}
}
