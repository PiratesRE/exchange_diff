using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.SendAsVerification;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.SendAs
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SendAsAutoProvision
	{
		public SendAsAutoProvision() : this(new SendAsManager(), new AutoProvisionOverrideProvider())
		{
		}

		public SendAsAutoProvision(SendAsManager sendAsManager, IAutoProvisionOverrideProvider autoProvisionOverrideProvider)
		{
			this.sendAsManager = sendAsManager;
			this.autoProvisionOverrideProvider = autoProvisionOverrideProvider;
		}

		public void SetAppropriateSendAsState(PimAggregationSubscription subscription, IEmailSender emailSender)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("emailSender", emailSender);
			bool meetsEnableCriteria = this.MeetsEnableCriteria(subscription);
			this.sendAsManager.EnableSendAs(subscription, meetsEnableCriteria, emailSender);
		}

		internal bool DoesIncomingServerMatchEmailAddress(PimAggregationSubscription subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			if (!subscription.SendAsNeedsVerification)
			{
				throw new ArgumentException("subscription does not need send as verification.  Type: " + subscription.SubscriptionType.ToString(), "subscription");
			}
			string domain = subscription.Email.Domain;
			string verifiedIncomingServer = subscription.VerifiedIncomingServer;
			return verifiedIncomingServer.Length >= domain.Length && (string.Equals(domain, verifiedIncomingServer, StringComparison.OrdinalIgnoreCase) || verifiedIncomingServer.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase));
		}

		private bool MeetsEnableCriteria(PimAggregationSubscription subscription)
		{
			if (!subscription.SendAsCapable)
			{
				return false;
			}
			if (!subscription.SendAsNeedsVerification)
			{
				return true;
			}
			if (!string.Equals(subscription.Email.ToString(), subscription.VerifiedUserName, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			string[] collection;
			if (this.TryGetSendAsOverrides(subscription, out collection))
			{
				return SyncUtilities.ExistsInCollection<string>(subscription.VerifiedIncomingServer, collection, StringComparer.OrdinalIgnoreCase);
			}
			return this.DoesIncomingServerMatchEmailAddress(subscription);
		}

		private bool TryGetSendAsOverrides(PimAggregationSubscription subscription, out string[] overrideHosts)
		{
			overrideHosts = null;
			string domain = subscription.Email.Domain;
			bool flag;
			return this.autoProvisionOverrideProvider.TryGetOverrides(domain, subscription.SubscriptionType, out overrideHosts, out flag) && flag;
		}

		private SendAsManager sendAsManager;

		private IAutoProvisionOverrideProvider autoProvisionOverrideProvider;
	}
}
