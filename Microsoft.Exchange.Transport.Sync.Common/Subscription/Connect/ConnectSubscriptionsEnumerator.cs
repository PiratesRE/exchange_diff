using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConnectSubscriptionsEnumerator : IEnumerable<ConnectSubscription>, IEnumerable
	{
		public ConnectSubscriptionsEnumerator(MailboxSession session, string provider)
		{
			SyncUtilities.ThrowIfArgumentNull("session", session);
			this.session = session;
			this.provider = this.NormalizeProvider(provider);
		}

		public IEnumerator<ConnectSubscription> GetEnumerator()
		{
			if (string.IsNullOrEmpty(this.provider))
			{
				return Enumerable.Empty<ConnectSubscription>().GetEnumerator();
			}
			AggregationSubscriptionType providerFilter = this.GetProviderFilter();
			if (providerFilter == AggregationSubscriptionType.Unknown)
			{
				return Enumerable.Empty<ConnectSubscription>().GetEnumerator();
			}
			return SubscriptionManager.GetAllSubscriptions(this.session, providerFilter).Cast<ConnectSubscription>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generic version of GetEnumerator.");
		}

		private AggregationSubscriptionType GetProviderFilter()
		{
			if (WellKnownNetworkNames.Facebook.Equals(this.provider, StringComparison.OrdinalIgnoreCase))
			{
				return AggregationSubscriptionType.Facebook;
			}
			if (WellKnownNetworkNames.LinkedIn.Equals(this.provider, StringComparison.OrdinalIgnoreCase))
			{
				return AggregationSubscriptionType.LinkedIn;
			}
			return AggregationSubscriptionType.Unknown;
		}

		private string NormalizeProvider(string provider)
		{
			if (string.IsNullOrEmpty(provider))
			{
				return string.Empty;
			}
			return provider.Trim();
		}

		private readonly MailboxSession session;

		private readonly string provider;
	}
}
