using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PendingGetPublisherFactory : PushNotificationPublisherFactory
	{
		public PendingGetPublisherFactory(IPendingGetConnectionCache connectionCache, List<IPushNotificationMapping<PendingGetNotification>> mappings = null)
		{
			this.connectionCache = connectionCache;
			this.Mappings = mappings;
		}

		public override PushNotificationPlatform Platform
		{
			get
			{
				return PushNotificationPlatform.PendingGet;
			}
		}

		private List<IPushNotificationMapping<PendingGetNotification>> Mappings { get; set; }

		public override PushNotificationPublisherBase CreatePublisher(PushNotificationPublisherSettings settings)
		{
			PendingGetPublisherSettings pendingGetPublisherSettings = settings as PendingGetPublisherSettings;
			if (pendingGetPublisherSettings == null)
			{
				throw new ArgumentException(string.Format("settings should be an PendingGetPublisherSettings instance: {0}", (settings == null) ? "null" : settings.GetType().ToString()));
			}
			return new PendingGetPublisher(pendingGetPublisherSettings, this.connectionCache, null, null, this.Mappings);
		}

		private IPendingGetConnectionCache connectionCache;
	}
}
