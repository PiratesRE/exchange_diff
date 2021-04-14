using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	internal class PublishOnPremNotifications : PublishNotificationsBase<MailboxNotificationBatch>
	{
		public PublishOnPremNotifications(MailboxNotificationBatch notifications, PushNotificationPublisherManager publisherManager, AsyncCallback asyncCallback, object asyncState) : base(notifications, publisherManager, asyncCallback, asyncState)
		{
		}

		protected override void Publish()
		{
			base.PublisherManager.Publish(new ProxyNotification(PushNotificationCannedApp.OnPremProxy.Name, null, base.RequestInstance));
		}
	}
}
