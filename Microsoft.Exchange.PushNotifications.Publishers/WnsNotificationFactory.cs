using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsNotificationFactory : PushNotificationFactory<WnsNotification>
	{
		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, int recipientId)
		{
			return new MailboxNotificationRecipient(appId, string.Format("http://127.0.0.1:0/send?id={0}", recipientId), DateTime.UtcNow, null);
		}

		public override MailboxNotificationRecipient CreateMonitoringRecipient(string appId, string recipientId)
		{
			throw new NotImplementedException("RecipientId is required as int for creating WNS MonitoringRecipient");
		}

		protected override bool TryCreateUnseenEmailNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out WnsNotification notification)
		{
			notification = new WnsBadgeNotification(recipient.AppId, context.OrgId, recipient.DeviceId, payload.UnseenEmailCount.Value, null, new WnsCachePolicy?(WnsCachePolicy.Cache));
			return true;
		}

		protected override bool TryCreateMonitoringNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, PushNotificationPublishingContext context, out WnsNotification notification)
		{
			notification = new WnsMonitoringNotification(recipient.AppId, recipient.DeviceId);
			return true;
		}

		private bool TryCreateWnsTileNotification(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient, OrganizationId tenantId, string text, out WnsNotification notification)
		{
			WnsText[] texts = new WnsText[]
			{
				new WnsText(text, null)
			};
			WnsTileBinding binding = new WnsTileBinding(WnsTile.SquareText04, null, null, null, null, null, texts, null);
			WnsTileBinding[] extraBindings = new WnsTileBinding[]
			{
				new WnsTileBinding(WnsTile.WideText04, null, null, null, null, null, texts, null)
			};
			notification = new WnsTileNotification(recipient.AppId, tenantId, recipient.DeviceId, new WnsTileVisual(binding, extraBindings, null, null, null, null), null, null, null);
			return true;
		}

		public static readonly WnsNotificationFactory Default = new WnsNotificationFactory();
	}
}
