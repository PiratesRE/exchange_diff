using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ProxyPublisherFactory : PushNotificationPublisherFactory
	{
		public override PushNotificationPlatform Platform
		{
			get
			{
				return PushNotificationPlatform.Proxy;
			}
		}

		public override PushNotificationPublisherBase CreatePublisher(PushNotificationPublisherSettings settings)
		{
			ProxyPublisherSettings proxyPublisherSettings = settings as ProxyPublisherSettings;
			if (proxyPublisherSettings == null)
			{
				throw new ArgumentException(string.Format("settings should be an ProxyPublisherFactory instance: {0}", (settings == null) ? "null" : settings.GetType().ToString()));
			}
			return new ProxyPublisher(proxyPublisherSettings);
		}
	}
}
