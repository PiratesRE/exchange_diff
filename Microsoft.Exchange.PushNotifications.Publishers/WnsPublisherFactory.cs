using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsPublisherFactory : PushNotificationPublisherFactory
	{
		public WnsPublisherFactory(IThrottlingManager throttlingManager, List<IPushNotificationMapping<WnsNotification>> mappings = null)
		{
			ArgumentValidator.ThrowIfNull("throttlingManager", throttlingManager);
			this.ThrottlingManager = throttlingManager;
			this.Mappings = mappings;
		}

		public override PushNotificationPlatform Platform
		{
			get
			{
				return PushNotificationPlatform.WNS;
			}
		}

		private List<IPushNotificationMapping<WnsNotification>> Mappings { get; set; }

		private IThrottlingManager ThrottlingManager { get; set; }

		public override PushNotificationPublisherBase CreatePublisher(PushNotificationPublisherSettings settings)
		{
			WnsPublisherSettings wnsPublisherSettings = settings as WnsPublisherSettings;
			if (wnsPublisherSettings == null)
			{
				throw new ArgumentException(string.Format("settings should be an WnsPublisherSettings instance: {0}", (settings == null) ? "null" : settings.GetType().ToString()));
			}
			return new WnsPublisher(wnsPublisherSettings, this.ThrottlingManager, this.Mappings);
		}
	}
}
