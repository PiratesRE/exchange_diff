using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Worker.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SubscriptionWorkItemEventArgs<R> : SubscriptionEventArgs<R> where R : SubscriptionEventResult
	{
		public SubscriptionWorkItemEventArgs(R result, SyncLogSession syncLogSession, Guid subscriptionId, Exception workItemResultException, StoreObjectId subscriptionMessageId, Guid mailboxGuid, string userLegacyDn, Guid tenantGuid, OrganizationId organizationId) : base(syncLogSession, result)
		{
			SyncUtilities.ThrowIfArgumentNull("workItem", syncLogSession);
			SyncUtilities.ThrowIfGuidEmpty("subscriptionId", subscriptionId);
			SyncUtilities.ThrowIfArgumentNull("subscriptionMessageId", subscriptionMessageId);
			SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("userLegacyDn", userLegacyDn);
			this.subscriptionId = subscriptionId;
			this.workItemResultException = workItemResultException;
			this.subscriptionMessageId = subscriptionMessageId;
			this.mailboxGuid = mailboxGuid;
			this.userLegacyDn = userLegacyDn;
			this.tenantGuid = tenantGuid;
			this.organizationId = organizationId;
		}

		public Guid SubscriptionId
		{
			get
			{
				return this.subscriptionId;
			}
		}

		public Exception WorkItemResultException
		{
			get
			{
				return this.workItemResultException;
			}
		}

		public StoreObjectId SubscriptionMessageId
		{
			get
			{
				return this.subscriptionMessageId;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public string UserLegacyDn
		{
			get
			{
				return this.userLegacyDn;
			}
		}

		public Guid TenantGuid
		{
			get
			{
				return this.tenantGuid;
			}
		}

		internal OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		private readonly Guid subscriptionId;

		private readonly Exception workItemResultException;

		private readonly StoreObjectId subscriptionMessageId;

		private readonly Guid mailboxGuid;

		private readonly string userLegacyDn;

		private readonly Guid tenantGuid;

		private readonly OrganizationId organizationId;
	}
}
