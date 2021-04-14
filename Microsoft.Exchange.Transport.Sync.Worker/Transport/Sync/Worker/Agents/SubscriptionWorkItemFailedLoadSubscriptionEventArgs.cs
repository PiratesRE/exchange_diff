using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Worker.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SubscriptionWorkItemFailedLoadSubscriptionEventArgs : SubscriptionWorkItemEventArgs<SubscriptionWorkItemFailedLoadSubscriptionEventResult>
	{
		public SubscriptionWorkItemFailedLoadSubscriptionEventArgs(SyncLogSession syncLogSession, Guid subscriptionId, Exception workItemResultException, StoreObjectId subscriptionMessageId, Guid mailboxGuid, string userLegacyDn, Guid tenantGuid, OrganizationId organizationId) : base(new SubscriptionWorkItemFailedLoadSubscriptionEventResult(), syncLogSession, subscriptionId, workItemResultException, subscriptionMessageId, mailboxGuid, userLegacyDn, tenantGuid, organizationId)
		{
		}
	}
}
