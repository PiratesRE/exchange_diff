using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;

namespace Microsoft.Exchange.HttpProxy
{
	internal class PushNotificationsProxyRequestHandler : ProxyRequestHandler
	{
		protected override MailboxServerLocator CreateMailboxServerLocator(Guid databaseGuid, string domainName, string resourceForest)
		{
			return base.CreateMailboxServerLocator(databaseGuid, domainName, resourceForest);
		}
	}
}
