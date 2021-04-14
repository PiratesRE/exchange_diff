using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "UpgradeWorkItem", DefaultParameterSetName = "regularQuery")]
	public class GetUpgradeWorkItem : SymphonyTaskBase
	{
		[Parameter(Mandatory = false, ParameterSetName = "regularQuery")]
		public string ForestName { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "regularQuery")]
		public Unlimited<int> ResultSize
		{
			get
			{
				return this.resultSizeField;
			}
			set
			{
				this.resultSizeField = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "regularQuery")]
		public WorkItemStatus[] Status { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "regularQuery")]
		public string TenantTier { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "regularQuery")]
		public string Type { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "tenantQuery")]
		[Parameter(Mandatory = false, ParameterSetName = "WorkItemIDQUery")]
		public Guid Tenant { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "WorkItemIDQUery")]
		public string WorkItemID { get; set; }

		protected override void InternalProcessRecord()
		{
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (!(parameterSetName == "regularQuery"))
				{
					if (!(parameterSetName == "tenantQuery"))
					{
						if (!(parameterSetName == "WorkItemIDQUery"))
						{
							goto IL_247;
						}
						goto IL_208;
					}
				}
				else
				{
					WorkItemStatus[] array = base.UserSpecifiedParameters.Contains("Status") ? this.Status : ((WorkItemStatus[])Enum.GetValues(typeof(WorkItemStatus)));
					using (ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler> workloadClient = new ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler>(base.WorkloadUri, base.Certificate))
					{
						WorkItemStatus[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							GetUpgradeWorkItem.<>c__DisplayClass4 CS$<>8__locals2 = new GetUpgradeWorkItem.<>c__DisplayClass4();
							CS$<>8__locals2.status = array2[i];
							WorkItemQueryResult queryResult = new WorkItemQueryResult();
							bool flag;
							do
							{
								workloadClient.CallSymphony(delegate
								{
									queryResult = workloadClient.Proxy.QueryWorkItems(this.ForestName, this.TenantTier, this.Type, CS$<>8__locals2.status, 1000, queryResult.Bookmark);
								}, base.WorkloadUri.ToString());
								flag = this.WriteWorkitems(queryResult.WorkItems);
							}
							while (queryResult.HasMoreResults && !flag);
							if (flag)
							{
								break;
							}
						}
						return;
					}
				}
				using (ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler> workloadClient = new ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler>(base.WorkloadUri, base.Certificate))
				{
					if (this.Tenant == Guid.Empty)
					{
						throw new InvalidTenantGuidException(this.Tenant.ToString());
					}
					WorkItemInfo[] retrieved = null;
					workloadClient.CallSymphony(delegate
					{
						retrieved = workloadClient.Proxy.QueryTenantWorkItems(this.Tenant);
					}, base.WorkloadUri.ToString());
					this.WriteWorkitems(retrieved);
					return;
				}
				IL_208:
				WorkItemInfo workItem;
				if (this.Tenant != Guid.Empty)
				{
					workItem = base.GetWorkitemByIdAndTenantId(this.WorkItemID, this.Tenant);
				}
				else
				{
					workItem = base.GetWorkItemById(this.WorkItemID);
				}
				this.WriteWorkItem(workItem);
				return;
			}
			IL_247:
			throw new ArgumentException("Invalid parameter set.");
		}

		private bool WriteWorkitems(IEnumerable<WorkItemInfo> workItems)
		{
			foreach (WorkItemInfo workItem in workItems)
			{
				if (!this.ResultSize.IsUnlimited && this.writtenCount >= this.ResultSize.Value)
				{
					this.WriteWarning(Strings.WarningMoreResultsAvailable);
					return true;
				}
				this.WriteWorkItem(workItem);
				this.writtenCount++;
			}
			return false;
		}

		private void WriteWorkItem(WorkItemInfo workItem)
		{
			UpgradeWorkItem sendToPipeline = new UpgradeWorkItem(workItem);
			base.WriteObject(sendToPipeline);
		}

		private const int PageSize = 1000;

		private const string RegularQuery = "regularQuery";

		private const string TenantQuery = "tenantQuery";

		private const string WorkItemIDQuery = "WorkItemIDQUery";

		private int writtenCount;

		private Unlimited<int> resultSizeField = 100;
	}
}
