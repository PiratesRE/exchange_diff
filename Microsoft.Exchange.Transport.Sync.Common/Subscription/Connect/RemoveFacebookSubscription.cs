using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Net.Facebook;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RemoveFacebookSubscription : IRemoveConnectSubscription
	{
		public RemoveFacebookSubscription(IFacebookClient client)
		{
			SyncUtilities.ThrowIfArgumentNull("client", client);
			this.facebookClient = client;
		}

		public void TryRemovePermissions(IConnectSubscription subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			try
			{
				this.facebookClient.RemoveApplication(subscription.AccessTokenInClearText);
			}
			catch (Exception ex)
			{
				CommonLoggingHelper.SyncLogSession.LogError((TSLID)97UL, RemoveFacebookSubscription.Tracer, "RemoveFacebookSubscription.TryRemovePermissions: {0} hit exception: {1}.", new object[]
				{
					subscription.SubscriptionGuid,
					ex
				});
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.SubscriptionRemoveTracer;

		private readonly IFacebookClient facebookClient;
	}
}
