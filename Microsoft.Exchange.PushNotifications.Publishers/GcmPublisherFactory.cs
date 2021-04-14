using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class GcmPublisherFactory : PushNotificationPublisherFactory
	{
		public GcmPublisherFactory(IThrottlingManager throttlingManager, List<IPushNotificationMapping<GcmNotification>> mappings = null)
		{
			this.ThrottlingManager = throttlingManager;
			this.Mappings = mappings;
		}

		public override PushNotificationPlatform Platform
		{
			get
			{
				return PushNotificationPlatform.GCM;
			}
		}

		private IThrottlingManager ThrottlingManager { get; set; }

		private List<IPushNotificationMapping<GcmNotification>> Mappings { get; set; }

		public override PushNotificationPublisherBase CreatePublisher(PushNotificationPublisherSettings settings)
		{
			GcmPublisherSettings gcmPublisherSettings = settings as GcmPublisherSettings;
			if (gcmPublisherSettings == null)
			{
				throw new ArgumentException(string.Format("settings should be an GcmPublisherSettings instance: {0}", (settings == null) ? "null" : settings.GetType().ToString()));
			}
			return new GcmPublisher(gcmPublisherSettings, this.ThrottlingManager, this.Mappings);
		}
	}
}
