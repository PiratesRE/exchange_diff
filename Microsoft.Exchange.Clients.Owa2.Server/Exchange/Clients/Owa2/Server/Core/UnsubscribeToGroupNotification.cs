using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class UnsubscribeToGroupNotification : UnsubscribeToNotificationBase
	{
		public UnsubscribeToGroupNotification(CallContext callContext, SubscriptionData[] subscriptionData) : base(callContext, subscriptionData)
		{
		}

		protected override void InternalUnsubscribeNotification(IMailboxContext userContext, SubscriptionData subscription)
		{
			if (subscription.Parameters == null)
			{
				throw new ArgumentNullException("Subscription data parameters cannot be null");
			}
			base.InternalUnsubscribeNotification(userContext, subscription);
		}
	}
}
