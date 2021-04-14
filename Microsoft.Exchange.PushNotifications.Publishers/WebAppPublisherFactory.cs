using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WebAppPublisherFactory : PushNotificationPublisherFactory
	{
		public WebAppPublisherFactory(List<IPushNotificationMapping<WebAppNotification>> mappings = null)
		{
			this.Mappings = mappings;
		}

		public override PushNotificationPlatform Platform
		{
			get
			{
				return PushNotificationPlatform.WebApp;
			}
		}

		private List<IPushNotificationMapping<WebAppNotification>> Mappings { get; set; }

		public override PushNotificationPublisherBase CreatePublisher(PushNotificationPublisherSettings settings)
		{
			WebAppPublisherSettings webAppPublisherSettings = settings as WebAppPublisherSettings;
			if (webAppPublisherSettings == null)
			{
				throw new ArgumentException(string.Format("settings should be an WebAppPublisherSettings instance: {0}", (settings == null) ? "null" : settings.GetType().ToString()));
			}
			return new WebAppPublisher(webAppPublisherSettings, this.Mappings);
		}
	}
}
