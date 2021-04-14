using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Net.LinkedIn;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RemoveLinkedInSubscription : IRemoveConnectSubscription
	{
		public RemoveLinkedInSubscription(ILinkedInWebClient client)
		{
			SyncUtilities.ThrowIfArgumentNull("client", client);
			this.linkedInClient = client;
		}

		public void TryRemovePermissions(IConnectSubscription subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			try
			{
				this.linkedInClient.RemoveApplicationPermissions(subscription.AccessTokenInClearText, subscription.AccessTokenSecretInClearText);
			}
			catch (Exception ex)
			{
				CommonLoggingHelper.SyncLogSession.LogError((TSLID)177UL, RemoveLinkedInSubscription.Tracer, "RemoveLinkedInSubscription.TryRemovePermissions: {0} hit exception: {1}.", new object[]
				{
					subscription.SubscriptionGuid,
					ex
				});
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.SubscriptionRemoveTracer;

		private readonly ILinkedInWebClient linkedInClient;
	}
}
