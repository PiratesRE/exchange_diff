using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class HierachyNotificationHandlerFactory
	{
		public static HierarchyNotificationHandler CreateHandler(string subscriptionId, UserContext userContext)
		{
			if (userContext.FeaturesManager.ServerSettings.InferenceUI.Enabled && userContext.MailboxSession.Mailbox != null && userContext.MailboxSession.Mailbox.GetValueOrDefault<bool>(MailboxSchema.InferenceUserUIReady, false))
			{
				return new InferenceHierarchyNotificationHandler(subscriptionId, userContext, userContext.MailboxSession.MailboxGuid);
			}
			return new HierarchyNotificationHandler(subscriptionId, userContext, userContext.MailboxSession.MailboxGuid);
		}
	}
}
