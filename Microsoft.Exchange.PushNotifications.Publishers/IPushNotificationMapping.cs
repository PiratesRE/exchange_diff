using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal interface IPushNotificationMapping<T> where T : PushNotification
	{
		Type InputType { get; }

		bool TryMap(Notification notification, PushNotificationPublishingContext context, out T pushNotification);
	}
}
