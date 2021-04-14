using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class UnifiedPolicySyncNotification : UnifiedPolicyNotificationBase
	{
		public UnifiedPolicySyncNotification()
		{
		}

		public UnifiedPolicySyncNotification(SyncWorkItem workItem, ADObjectId mailboxOwnerId) : base(workItem, mailboxOwnerId)
		{
			if (workItem.WorkItemInfo != null && workItem.WorkItemInfo.Count > 0)
			{
				this.syncChangeInfos = new MultiValuedProperty<string>();
				foreach (List<SyncChangeInfo> list in workItem.WorkItemInfo.Values)
				{
					foreach (SyncChangeInfo syncChangeInfo in list)
					{
						this.syncChangeInfos.Add(syncChangeInfo.ToString());
					}
				}
			}
		}

		[Parameter]
		public string SyncSvcUrl
		{
			get
			{
				return ((SyncWorkItem)this.workItem).SyncSvcUrl;
			}
		}

		[Parameter]
		public bool FullSync
		{
			get
			{
				return ((SyncWorkItem)this.workItem).FullSyncForTenant;
			}
		}

		[Parameter]
		public bool SyncNow
		{
			get
			{
				return ((SyncWorkItem)this.workItem).ProcessNow;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> SyncChangeInfos
		{
			get
			{
				return this.syncChangeInfos;
			}
		}

		private readonly MultiValuedProperty<string> syncChangeInfos;
	}
}
