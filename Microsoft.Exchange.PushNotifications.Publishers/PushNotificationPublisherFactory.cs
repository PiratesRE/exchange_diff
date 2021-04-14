using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class PushNotificationPublisherFactory
	{
		public abstract PushNotificationPlatform Platform { get; }

		public abstract PushNotificationPublisherBase CreatePublisher(PushNotificationPublisherSettings settings);
	}
}
