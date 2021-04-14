using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class UnifiedPolicyStatusNotification : UnifiedPolicyNotificationBase
	{
		public UnifiedPolicyStatusNotification(SyncStatusUpdateWorkitem workItem, ADObjectId mailboxOwnerId) : base(workItem, mailboxOwnerId)
		{
		}

		[Parameter]
		public string StatusUpdateSvcUrl
		{
			get
			{
				return ((SyncStatusUpdateWorkitem)this.workItem).StatusUpdateSvcUrl;
			}
		}

		[Parameter]
		public IEnumerable<UnifiedPolicyStatus> StatusUpdates
		{
			get
			{
				return ((SyncStatusUpdateWorkitem)this.workItem).StatusUpdates;
			}
		}

		[Parameter]
		public bool SyncNow
		{
			get
			{
				return ((SyncStatusUpdateWorkitem)this.workItem).ProcessNow;
			}
		}
	}
}
