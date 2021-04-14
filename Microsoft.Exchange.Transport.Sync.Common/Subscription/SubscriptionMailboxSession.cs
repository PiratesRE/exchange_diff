using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SubscriptionMailboxSession
	{
		public SubscriptionMailboxSession(MailboxSession mailboxSession)
		{
			SyncUtilities.ThrowIfArgumentNull("mailboxSession", mailboxSession);
			this.mailboxSession = mailboxSession;
		}

		protected SubscriptionMailboxSession()
		{
		}

		internal MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		internal virtual void SetPropertiesOfSubscription(ISyncWorkerData subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			subscription.SubscriptionIdentity.LegacyDN = this.mailboxSession.MailboxOwnerLegacyDN;
			subscription.SubscriptionIdentity.PrimaryMailboxLegacyDN = this.mailboxSession.MailboxOwner.LegacyDn;
			subscription.SubscriptionIdentity.AdUserId = this.mailboxSession.MailboxOwner.ObjectId;
			subscription.UserExchangeMailboxSmtpAddress = this.mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			subscription.UserExchangeMailboxDisplayName = this.mailboxSession.MailboxOwner.MailboxInfo.DisplayName;
		}

		internal virtual string GetMailboxServerName()
		{
			return this.mailboxSession.ServerFullyQualifiedDomainName;
		}

		private MailboxSession mailboxSession;
	}
}
