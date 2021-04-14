using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class UnifiedPolicyNotificationFactory
	{
		public static UnifiedPolicyNotificationBase Create(WorkItemBase workItem, ADObjectId mailboxOwnerId)
		{
			if (workItem == null)
			{
				throw new ArgumentNullException("workItem");
			}
			if (mailboxOwnerId == null)
			{
				throw new ArgumentNullException("mailboxOwnerId");
			}
			if (workItem is SyncWorkItem)
			{
				return new UnifiedPolicySyncNotification(workItem as SyncWorkItem, mailboxOwnerId);
			}
			if (workItem is SyncStatusUpdateWorkitem)
			{
				return new UnifiedPolicyStatusNotification(workItem as SyncStatusUpdateWorkitem, mailboxOwnerId);
			}
			throw new NotImplementedException("not implemented yet");
		}
	}
}
