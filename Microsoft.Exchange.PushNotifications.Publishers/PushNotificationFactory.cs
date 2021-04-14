using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class PushNotificationFactory<T> : IMonitoringMailboxNotificationRecipientFactory, IPushNotificationMapping<T> where T : PushNotification
	{
		public PushNotificationFactory()
		{
			this.InputType = typeof(MailboxNotificationFragment);
		}

		public Type InputType { get; private set; }

		public bool TryMap(Notification notification, PushNotificationPublishingContext context, out T pushNotification)
		{
			MailboxNotificationFragment mailboxNotificationFragment = notification as MailboxNotificationFragment;
			if (mailboxNotificationFragment == null)
			{
				throw new InvalidOperationException("Notification type not supported: " + notification.ToFullString());
			}
			return this.TryCreate(mailboxNotificationFragment.Payload, mailboxNotificationFragment.Recipient, context, out pushNotification);
		}

		public virtual bool TryCreate(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out T notification)
		{
			if (payload.IsMonitoring)
			{
				return this.TryCreateMonitoringNotification(payload, recipient, context, out notification);
			}
			return this.TryCreateUnseenEmailNotification(payload, recipient, context, out notification);
		}

		public abstract MailboxNotificationRecipient CreateMonitoringRecipient(string appId, int recipientId);

		public abstract MailboxNotificationRecipient CreateMonitoringRecipient(string appId, string recipientId);

		protected abstract bool TryCreateUnseenEmailNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out T notification);

		protected abstract bool TryCreateMonitoringNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out T notification);

		protected string GetBackgroundSyncTypeString(BackgroundSyncType backgroundSyncType)
		{
			if (backgroundSyncType == BackgroundSyncType.Email)
			{
				return "e";
			}
			return null;
		}

		internal const string EmailBackgroundSyncType = "e";
	}
}
