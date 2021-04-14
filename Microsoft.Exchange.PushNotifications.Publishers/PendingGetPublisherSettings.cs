using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PendingGetPublisherSettings : PushNotificationPublisherSettings
	{
		public PendingGetPublisherSettings(string appId, bool enabled, Version minimumVersion, Version maximumVersion, int queueSize, int numberOfChannels, int addTimeout) : base(appId, enabled, minimumVersion, maximumVersion, queueSize, numberOfChannels, addTimeout)
		{
		}

		public const bool DefaultEnabled = true;

		public const Version DefaultMinimumVersion = null;

		public const Version DefaultMaximumVersion = null;

		public const int DefaultQueueSize = 10000;

		public const int DefaultNumberOfChannels = 1;

		public const int DefaultAddTimeout = 15;
	}
}
