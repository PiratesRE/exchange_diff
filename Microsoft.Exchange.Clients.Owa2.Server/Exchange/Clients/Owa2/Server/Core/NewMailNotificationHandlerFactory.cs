using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class NewMailNotificationHandlerFactory
	{
		public static NewMailNotificationHandler Create(string subscriptionId, IMailboxContext userContext, SubscriptionParameters parameters)
		{
			if (parameters.InferenceEnabled)
			{
				return new InferenceNewMailNotificationHandler(subscriptionId, userContext);
			}
			return new NewMailNotificationHandler(subscriptionId, userContext);
		}
	}
}
