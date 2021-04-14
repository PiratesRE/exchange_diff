using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsPublisherFactory : PushNotificationPublisherFactory
	{
		public ApnsPublisherFactory(IApnsFeedbackProvider feedbackProvider, IThrottlingManager throttlingManager, List<IPushNotificationMapping<ApnsNotification>> mappings = null)
		{
			ArgumentValidator.ThrowIfNull("feedbackProvider", feedbackProvider);
			ArgumentValidator.ThrowIfNull("throttlingManager", throttlingManager);
			this.FeedbackProvider = feedbackProvider;
			this.ThrottlingManager = throttlingManager;
			this.Mappings = mappings;
		}

		public override PushNotificationPlatform Platform
		{
			get
			{
				return PushNotificationPlatform.APNS;
			}
		}

		private IApnsFeedbackProvider FeedbackProvider { get; set; }

		private IThrottlingManager ThrottlingManager { get; set; }

		private List<IPushNotificationMapping<ApnsNotification>> Mappings { get; set; }

		public override PushNotificationPublisherBase CreatePublisher(PushNotificationPublisherSettings settings)
		{
			ApnsPublisherSettings apnsPublisherSettings = settings as ApnsPublisherSettings;
			if (apnsPublisherSettings == null)
			{
				throw new ArgumentException(string.Format("settings should be an ApnsPublisherSettings instance: {0}", (settings == null) ? "null" : settings.GetType().ToString()));
			}
			return new ApnsPublisher(apnsPublisherSettings, this.FeedbackProvider, this.ThrottlingManager, this.Mappings);
		}
	}
}
