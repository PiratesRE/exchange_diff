using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Worker.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SubscriptionWorkItemCompletedEventArgs : SubscriptionWorkItemEventArgs<SubscriptionWorkItemCompletedEventResult>
	{
		public SubscriptionWorkItemCompletedEventArgs(SyncLogSession syncLogSession, Guid subscriptionId, ISyncWorkerData subscription, bool isSyncNow, Exception workItemResultException, StoreObjectId subscriptionMessageId, Guid mailboxGuid, string userLegacyDn, Guid tenantGuid, OrganizationId organizationId, MailboxSession mailboxSession) : base(new SubscriptionWorkItemCompletedEventResult(), syncLogSession, subscriptionId, workItemResultException, subscriptionMessageId, mailboxGuid, userLegacyDn, tenantGuid, organizationId)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			this.subscription = subscription;
			this.mailboxSession = mailboxSession;
			this.isSyncNow = isSyncNow;
		}

		public bool IsSyncNow
		{
			get
			{
				return this.isSyncNow;
			}
		}

		public ISyncWorkerData Subscription
		{
			get
			{
				return this.subscription;
			}
		}

		public MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		private readonly ISyncWorkerData subscription;

		private readonly MailboxSession mailboxSession;

		private readonly bool isSyncNow;
	}
}
